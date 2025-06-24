using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using System.Runtime.InteropServices;

public class GerenciadorSimulacao : MonoBehaviour
{
    public PID pidController;
    public ScormController scormController;
    public Rigidbody bolaRigidbody;
    public Rigidbody barraRigidbody;
    public GameObject botaoPrincipal;
    public TextMeshProUGUI textoDoBotao;
    public GameObject painelResultado;
    public TextMeshProUGUI textoResultado;
    public TMP_InputField inputKp;
    public TMP_InputField inputKi;
    public TMP_InputField inputKd;
    public GameObject painelGanhos;

    private Vector3 posInicialBola;
    private Quaternion rotInicialBola;
    private Vector3 posInicialBarra;
    private Quaternion rotInicialBarra;

    int Tentativa = 1;
    public float Max_Tentativas;

    private enum EstadoSimulacao { Pronta, Rodando, Finalizada }
    private EstadoSimulacao estadoAtual;

    private string caminhoArquivoTrava;
    public bool simBloqueada = false;

    void Awake()
    {
    #if !UNITY_WEBGL
        string pastaAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string nomeArquivoTrava = "simulacao_pid.lock";
        caminhoArquivoTrava = Path.Combine(pastaAppData, nomeArquivoTrava);

        if (File.Exists(caminhoArquivoTrava))
        {
            if (painelResultado != null && textoResultado != null)
            {
                painelResultado.SetActive(true);
                textoResultado.text = "Esta simulação já foi executada.";
                textoResultado.color = Color.yellow;
            }

            simBloqueada = true;
            if (botaoPrincipal != null) botaoPrincipal.SetActive(false);
            if (painelGanhos != null) painelGanhos.SetActive(false);

            enabled = false;
            return;
        }
        else
        {
            File.WriteAllText(caminhoArquivoTrava, $"Executado em: {DateTime.Now}");
        }
    #endif
    }

    void Start()
    {
        if (simBloqueada) return;
        posInicialBola = bolaRigidbody.position;
        rotInicialBola = bolaRigidbody.rotation;
        posInicialBarra = barraRigidbody.position;
        rotInicialBarra = barraRigidbody.rotation;

        PrepararParaInicio();
    }

    void PrepararParaInicio()
    {
        pidController.enabled = false;
        bolaRigidbody.isKinematic = true;
        barraRigidbody.isKinematic = true;

        bolaRigidbody.position = posInicialBola;
        bolaRigidbody.rotation = rotInicialBola;
        barraRigidbody.position = posInicialBarra;
        barraRigidbody.rotation = rotInicialBarra;

        pidController.ResetarEstado();

        textoDoBotao.text = "Iniciar Tentativa";
        textoDoBotao.color = Color.black;
        botaoPrincipal.SetActive(true);
        painelGanhos.SetActive(true);
        estadoAtual = EstadoSimulacao.Pronta;
    }

    public void BotaoPrincipalClicado()
    {
        if (inputKd.text == "" || inputKp.text == "" || inputKi.text == "")
        {
            textoDoBotao.text = "Ajustar valores dos ganhos.";
            textoDoBotao.color = Color.red;
            return;
        }
        if (estadoAtual == EstadoSimulacao.Pronta)
        {
            pidController.SetGanhos(Tentativa);
            pidController.enabled = true;
            bolaRigidbody.isKinematic = false;
            estadoAtual = EstadoSimulacao.Rodando;
            botaoPrincipal.SetActive(false);
            painelGanhos.SetActive(false);
        }
        else if (estadoAtual == EstadoSimulacao.Finalizada)
        {
            PrepararParaInicio();
        }
    }

    public void SimulacaoFinalizada(bool sucesso)
    {
        bolaRigidbody.isKinematic = true;
        barraRigidbody.isKinematic = true;
        estadoAtual = EstadoSimulacao.Finalizada;

        if (sucesso)
        {
            painelResultado.SetActive(true);
            textoResultado.text = "SUCESSO!";
            textoResultado.color = Color.green;
            pidController.enabled = false;
            SalvarResumoSucesso();
        }
        else
        {
            float kp = float.Parse(inputKp.text);
            float ki = float.Parse(inputKi.text);
            float kd = float.Parse(inputKd.text);
            float tempo = pidController.tempoMaximoEstabilizacao;
            scormController.SendResults(kp, ki, kd, tempo, false);

            Tentativa++;
            if (Tentativa <= Max_Tentativas)
            {
                textoDoBotao.text = "Falhou! Tentar Novamente?";
                botaoPrincipal.SetActive(true);
                pidController.enabled = false;
            }
            else
            {
                painelResultado.SetActive(true);
                textoResultado.text = "FALHOU!";
                textoResultado.color = Color.red;
                pidController.enabled = false;
            }
        }
        
    }

    void SalvarResumoSucesso()
    {
        float kp = float.Parse(inputKp.text);
        float ki = float.Parse(inputKi.text);
        float kd = float.Parse(inputKd.text);
        float pos_fim = Mathf.Round(pidController.Posicao_Final * 100f) / 100f;
        float tempo_fim = Mathf.Round(pidController.Tempo_Final * 100f) / 100f;
        float pos = pidController.Posicao;
        float tol = pidController.Tolerancia;
        float tempo = pidController.tempoMaximoEstabilizacao;

        scormController.SendResults(kp, ki, kd, tempo_fim, true);
        float produto = kp * ki * kd * pos_fim * tempo_fim * pos * tol * tempo;

        string nomeArquivo = $"PID_{produto:F2}.txt";

        string pastaDownloads = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Downloads";
        string caminho = Path.Combine(pastaDownloads, nomeArquivo); 

        List<string> linhas = new List<string>
        {
        $"Tentativa {Tentativa}",
        $"Kp = {kp}",
        $"Ki = {ki}",
        $"Kd = {kd}",
        $"Posição final = {pos_fim:F2}",
        $"Tempo de estabilização = {tempo_fim:F2} segundos",
        $"Posição desejada = {pos}",
        $"Tolerância = {tol} %",
        $"Tempo máximo de estabilização = {tempo} segundos"
        };

        File.WriteAllLines(caminho, linhas, Encoding.UTF8);

    }

}