using UnityEngine;

public class FrameTaskManager : MonoBehaviour
{
    public Camera playerCamera;
    public FrameBehavior[] cuadros;

    private FrameBehavior cuadroActual;
    private bool cerca = false;
    private bool tareaCompletada = false;

    void Update()
    {
        if (tareaCompletada) return;

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f))
        {
            FrameBehavior frame = hit.collider.GetComponent<FrameBehavior>();
            if (frame != null && !frame.EstaRecto)
            {
                cuadroActual = frame;
                cerca = true;
            }
            else
            {
                cerca = false;
                cuadroActual = null;
            }
        }
        else
        {
            cerca = false;
            cuadroActual = null;
        }

        if (cerca && Input.GetKeyDown(KeyCode.E) && cuadroActual != null)
        {
            cuadroActual.Enderezar();
        }

        if (TodosRectos())
        {
            tareaCompletada = true;
            Debug.Log("✅ ¡Todos los cuadros están rectos! Tarea completada.");
        }
    }

    bool TodosRectos()
    {
        foreach (FrameBehavior frame in cuadros)
        {
            if (!frame.EstaRecto) return false;
        }
        return true;
    }

    void OnGUI()
{
    if (cerca && cuadroActual != null && !cuadroActual.EstaRecto)
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
