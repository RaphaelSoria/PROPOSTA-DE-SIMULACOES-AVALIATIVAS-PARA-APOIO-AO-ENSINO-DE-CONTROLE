using UnityEngine;
using System.Runtime.InteropServices;

public class ScormController : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern bool JS_ScormInitialize();

    [DllImport("__Internal")]
    private static extern void JS_ScormFinish();

    [DllImport("__Internal")]
    private static extern void JS_ScormCommit();

    [DllImport("__Internal")]
    private static extern void JS_ScormSetValue(string element, string value);

    void Start()
    {
    #if !UNITY_EDITOR && UNITY_WEBGL
                JS_ScormInitialize();
    #endif
        }

    public void SendResults(float kp, float ki, float kd, float tempoEstabilizacao, bool flag)
    {
    #if !UNITY_EDITOR && UNITY_WEBGL
            bool success=flag;
            if (success) {
                JS_ScormSetValue("cmi.core.lesson_status", "passed");
                JS_ScormSetValue("cmi.core.score.raw", "100"); // Nota 100 para sucesso
            } else {
                JS_ScormSetValue("cmi.core.lesson_status", "failed");
                JS_ScormSetValue("cmi.core.score.raw", "0"); // Nota 0 para falha
            }

            string suspendData = $"{{ \"KP\":{kp}, \"KI\":{ki}, \"KD\":{kd}, \"tempo\":{tempoEstabilizacao} }}";
            JS_ScormSetValue("cmi.suspend_data", suspendData);

            JS_ScormSetValue("cmi.core.score.max", "100");
            JS_ScormSetValue("cmi.core.score.min", "0");

            JS_ScormCommit();

            JS_ScormFinish();
    #else
        Debug.Log($"Resultados (Modo Editor): Sucesso={flag}, KP={kp}, KI={ki}, KD={kd}, Tempo={tempoEstabilizacao}");
    #endif
    }
}