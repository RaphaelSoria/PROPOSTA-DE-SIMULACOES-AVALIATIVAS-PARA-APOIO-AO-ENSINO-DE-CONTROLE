using System.Collections;
using UnityEngine;
using TMPro;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class ConfiguracaoPID
{
    public float Posicao_Inicial;
    public float Posicao;
    public float Tolerancia;
    public float tempoMaximoEstabilizacao;
}

public class PID : MonoBehaviour
{
    public GerenciadorSimulacao gerenciador;
    public Transform Bola;
    public Transform Barra;

    public TMP_InputField inputKp;
    public TMP_InputField inputKi;
    public TMP_InputField inputKd;

    float Kp;
    float Ki;
    float Kd;

    int Tentativa;

    public float Posicao;
    public float Tolerancia;
    public float tempoMaximoEstabilizacao;

    public float Posicao_Final;
    public float Tempo_Final;

    float erro_Kp = 0;
    float erro_Ki = 0;
    float erro_Kd = 0;
    float Rotacao = 0;
    float tempoDecorrido = 0f;

    float velocidadeMinima = 0.01f;
    float anguloBarraMinimo = 0.5f;


    private List<string> logErros = new List<string>();

    private Rigidbody bolaRigidbody;
    private Rigidbody barraRigidbody;

    void Awake()
    {
        StartCoroutine(CarregarConfiguracoes());
    }

    void Update()
    {
        if (gerenciador.simBloqueada) return;
        float posY = Bola.position.y;

        if (posY < -3)
        {
            gerenciador.SimulacaoFinalizada(false);
            SalvarLogCSV();
            return;
        }

        tempoDecorrido += Time.deltaTime;

        if (tempoDecorrido > tempoMaximoEstabilizacao)
        {
            gerenciador.SimulacaoFinalizada(false);
            SalvarLogCSV();
            return;
        }

        float toleranciaAbsoluta = 0f;

        if (Posicao != 0)
        {
            toleranciaAbsoluta  = Mathf.Abs(Posicao) * (Tolerancia / 100.0f);
        }
        else
        {
            toleranciaAbsoluta = 1f * (Tolerancia / 100.0f);
        }
        float Posicao_Bola = Bola.position.x;
        erro_Kp = Posicao - Posicao_Bola;
        erro_Ki += (erro_Kp * Time.deltaTime);
        erro_Kd = bolaRigidbody.linearVelocity.x;

        bool dentroDaTolerancia = Mathf.Abs(erro_Kp) < toleranciaAbsoluta;
        bool velocidadeQuaseZero = Mathf.Abs(bolaRigidbody.linearVelocity.x) < velocidadeMinima;
        bool barraReta = barraRigidbody.rotation.eulerAngles.z < anguloBarraMinimo;

        if (dentroDaTolerancia && velocidadeQuaseZero && barraReta)
        {
            SalvarLogCSV();
            Posicao_Final = Posicao_Bola;
            Tempo_Final = tempoDecorrido;
            gerenciador.SimulacaoFinalizada(true);
            return;
        }

        Rotacao = ((erro_Kp * Kp) + (erro_Ki * Ki) - (erro_Kd * Kd)) * Time.deltaTime;

        if (Rotacao > 0.1f)
        {
            Rotacao = 0.1f;
        }
        if (Rotacao < -0.1f)
        {
            Rotacao = -0.1f;
        }
        
        transform.Rotate(new Vector3(0, 0, -Rotacao), Space.Self);

        string linha =
            (tempoDecorrido * 1000).ToString("F0") + "," +
            (erro_Kp * 1000).ToString("F0") + "," +
            (erro_Ki * 1000).ToString("F0") + "," +
            (erro_Kd * 1000).ToString("F0") + "," +
            (Posicao_Bola * 1000).ToString("F0");
        logErros.Add(linha);
    }

    public void ResetarEstado()
    {
        tempoDecorrido = 0f;
        erro_Kp = 0f;
        erro_Ki = 0f;
        erro_Kd = 0f;
        logErros.Clear();
    }

    public void SetGanhos(int Try)
    {
        Tentativa = Try;
        Kp = float.Parse(inputKp.text);
        Ki = float.Parse(inputKi.text);
        Kd = float.Parse(inputKd.text);
        logErros.Add("Erros para os valores,KP,KI,KD");
        logErros.Add($" ,{Kp.ToString().Replace(',', '.')},{Ki.ToString().Replace(',', '.')},{Kd.ToString().Replace(',', '.')}");
        logErros.Add("Tempo (ms),Erro_Kp (*1000),Erro_Ki (*1000),Erro_Kd (*1000),Posição da bola (*1000)");
    }

    void SalvarLogCSV()
    {
        string pastaDownloads = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Downloads";
        string caminho = Path.Combine(pastaDownloads, $"log_erros_tentativa_{Tentativa}.csv");
        File.WriteAllLines(caminho, logErros, Encoding.UTF8);
        System.Diagnostics.Process.Start("explorer.exe", "/select," + caminho.Replace("/", "\\"));
    }

    IEnumerator CarregarConfiguracoes()
    {
    #if UNITY_EDITOR
         string pastaDoExe = Application.dataPath;
    #else
         string pastaDoExe = Directory.GetParent(Application.dataPath).FullName;
    #endif
        string caminho = Path.Combine(pastaDoExe, "config.json");

        if (!File.Exists(caminho))
        {
            Debug.LogError("Arquivo config.json não encontrado em: " + caminho);
            yield break;
        }

        bolaRigidbody = Bola.GetComponent<Rigidbody>();
        barraRigidbody = Barra.GetComponent<Rigidbody>();

        string json = File.ReadAllText(caminho);
        ConfiguracaoPID config = JsonUtility.FromJson<ConfiguracaoPID>(json);

        Vector3 pos = bolaRigidbody.position;
        pos.x = config.Posicao_Inicial;
        bolaRigidbody.MovePosition(pos);

        Posicao = config.Posicao;
        Tolerancia = config.Tolerancia;
        tempoMaximoEstabilizacao = config.tempoMaximoEstabilizacao;

        yield return null;
    }
}