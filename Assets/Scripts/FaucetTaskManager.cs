using UnityEngine;

public class FaucetTaskManager : MonoBehaviour
{
    public Camera playerCamera;
    public FaucetBehavior[] grifos;

    private FaucetBehavior grifoActual;
    private bool cerca = false;
    private bool tareaCompletada = false;

    void Update()
    {
        if (tareaCompletada) return;

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
            estilo.fontSize = 20;
            estilo.normal.textColor = Color.cyan;
            estilo.alignment = TextAnchor.MiddleCenter;

            Rect mensaje = new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 50);
            GUI.Label(mensaje, "E para cerrar grifo", estilo);
        }
    }
}