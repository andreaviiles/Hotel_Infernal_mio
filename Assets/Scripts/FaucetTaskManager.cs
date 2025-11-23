using UnityEngine;

public class FaucetTaskManager : MonoBehaviour
{
    public Camera playerCamera;
    public FaucetBehavior[] grifos;

    private FaucetBehavior grifoActual;
    private bool cerca = false;
    private bool tareaCompletada = false;

    public PlayerMovement playerMovement;     // Referencia al jugador


    void Update()
    {
        if (tareaCompletada) return;

        if (playerMovement != null && playerMovement.EstaLlevandoObjeto)
        {
            cerca = false;
            grifoActual = null; // o grifoActual, cuadroActual, etc.
            return; // Bloquea la interacción si lleva objeto
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f))
        {
            FaucetBehavior faucet = hit.collider.GetComponent<FaucetBehavior>();
            if (faucet != null && !faucet.EstaCerrado)
            {
                grifoActual = faucet;
                cerca = true;
            }
            else
            {
                cerca = false;
                grifoActual = null;
            }
        }
        else
        {
            cerca = false;
            grifoActual = null;
        }

        // Interacción simple con una pulsación de E
        if (cerca && grifoActual != null && Input.GetKeyDown(KeyCode.E))
        {
            grifoActual.Cerrar();

            if (TodosCerrados())
            {
                tareaCompletada = true;
                Debug.Log("✅ ¡Todos los grifos cerrados! Tarea completada.");
            }
        }
    }

    bool TodosCerrados()
    {
        foreach (FaucetBehavior faucet in grifos)
        {
            if (!faucet.EstaCerrado) return false;
        }
        return true;
    }

    void OnGUI()
    {
        if (cerca && grifoActual != null && !grifoActual.EstaCerrado)
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