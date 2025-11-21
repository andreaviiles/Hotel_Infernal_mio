using UnityEngine;

public class LampTaskManager : MonoBehaviour
{
    public Camera playerCamera;
    public LampBehavior[] lamparas;

    private LampBehavior lamparaActual;
    private bool cerca = false;
    private bool tareaCompletada = false;

    void Update()
    {
        if (tareaCompletada) return;

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f))
        {
            LampBehavior lamp = hit.collider.GetComponent<LampBehavior>();
            if (lamp != null && !lamp.EstaEncendida)
            {
                lamparaActual = lamp;
                cerca = true;
            }
            else
            {
                cerca = false;
                lamparaActual = null;
            }
        }
        else
        {
            cerca = false;
            lamparaActual = null;
        }

        if (cerca && Input.GetKeyDown(KeyCode.E) && lamparaActual != null)
        {
            lamparaActual.Encender();
            Debug.Log($"Lámpara {lamparaActual.name} encendida.");

            if (TodasEncendidas())
            {
                tareaCompletada = true;
                Debug.Log("✅ ¡Todas las lámparas encendidas! Tarea completada.");
            }
        }
    }

    bool TodasEncendidas()
    {
        foreach (LampBehavior lamp in lamparas)
        {
            if (!lamp.EstaEncendida) return false;
        }
        return true;
    }

    void OnGUI()
{
    if (cerca && lamparaActual != null && !lamparaActual.EstaEncendida)
    {
        GUIStyle estilo = new GUIStyle(GUI.skin.label);
        estilo.fontSize = 40; // ← tamaño más grande
        estilo.normal.textColor = Color.white;
        estilo.alignment = TextAnchor.MiddleCenter;

        // Rect más ancho y alto para acomodar el texto grande
        Rect mensaje = new Rect(Screen.width / 2 - 200, Screen.height - 120, 400, 80);
        GUI.Label(mensaje, "E para interactuar", estilo);
    }
}
}
