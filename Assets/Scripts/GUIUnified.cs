using UnityEngine;

public class UnifiedInteractionUI : MonoBehaviour
{
    public InteractionScript interactionScript;
    public InteractionDemon interactionDemon;

    void OnGUI()
    {
        bool mostrarInteraccion = false;

        if (interactionScript != null && interactionScript.IsPlayerNear())
            mostrarInteraccion = true;

        if (interactionDemon != null && interactionDemon.IsPlayerNear())
            mostrarInteraccion = true;

        if (mostrarInteraccion)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            Rect rect = new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 50);
            GUI.Label(rect, "Presiona E para interactuar", style);
        }

        float tiempoCubos = interactionScript != null ? interactionScript.GetRemainingTime() : 0f;
        float tiempoDemonio = interactionDemon != null ? interactionDemon.GetRemainingTime() : 0f;

        float tiempoFusionado = Mathf.Max(tiempoCubos, tiempoDemonio);

        GUIStyle timerStyle = new GUIStyle();
        timerStyle.fontSize = 16;
        timerStyle.normal.textColor = (tiempoFusionado > 10f) ? Color.white : Color.red;

        GUI.Label(new Rect(10, 10, 250, 30), "Tiempo restante: " + Mathf.Ceil(tiempoFusionado).ToString() + "s", timerStyle);
    }
}
