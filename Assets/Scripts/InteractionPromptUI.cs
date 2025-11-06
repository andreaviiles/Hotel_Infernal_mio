using UnityEngine;

public class InteractionPromptUI : MonoBehaviour
{
    public Camera playerCamera;
    public float distanciaInteraccion = 2f;
    public PlayerMovement playerMovement; 

    private bool mostrarMensaje = false;

    void Update()
    {
        mostrarMensaje = false;

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, distanciaInteraccion))
        {
            string tag = hit.collider.tag;

            if (playerMovement != null && playerMovement.EstaLlevandoObjeto)
            {
                if (tag == "EntregaToalla")
                {
                    mostrarMensaje = true;
                }
            }
            else
            {
                if (tag == "Cleanable" || tag == "Toalla" || tag == "EntregaToalla")
                {
                    mostrarMensaje = true;
                }
            }
        }
    }

    void OnGUI()
    {
        if (!mostrarMensaje || Time.timeScale == 0f) return;

        GUIStyle estilo = new GUIStyle(GUI.skin.label);
        estilo.fontSize = 20;
        estilo.normal.textColor = Color.white;
        estilo.alignment = TextAnchor.MiddleCenter;

        Rect rect = new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 50);
        GUI.Label(rect, "Presiona E para interactuar", estilo);
    }
}

