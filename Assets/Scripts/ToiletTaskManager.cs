using UnityEngine;

public class ToiletTaskManager : MonoBehaviour
{
    public Camera playerCamera;
    public ToiletBehavior[] vateres;

    private ToiletBehavior vaterActual;
    private bool cerca = false;
    private bool tareaCompletada = false;

    private float tiempoMantener = 3f; // Tiempo necesario para limpiar
    private float contadorMantener = 0f;
    private bool manteniendo = false;

    void Update()
    {
        if (tareaCompletada) return;

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f))
        {
            ToiletBehavior toilet = hit.collider.GetComponent<ToiletBehavior>();
            if (toilet != null && !toilet.EstaLimpio)
            {
                vaterActual = toilet;
                cerca = true;
            }
            else
            {
                cerca = false;
                vaterActual = null;
            }
        }
        else
        {
            cerca = false;
            vaterActual = null;
        }

        if (cerca && vaterActual != null)
        {
            if (Input.GetKey(KeyCode.E))
            {
                manteniendo = true;
                contadorMantener += Time.deltaTime;

                if (contadorMantener >= tiempoMantener)
                {
                    vaterActual.Limpiar();
                    contadorMantener = 0f;
                    manteniendo = false;

                    if (TodosLimpios())
                    {
                        tareaCompletada = true;
                        Debug.Log("✅ ¡Todos los váteres han sido limpiados! Tarea completada.");
                    }
                }
            }
            else
            {
                manteniendo = false;
                contadorMantener = 0f;
            }
        }
    }

    bool TodosLimpios()
    {
        foreach (ToiletBehavior toilet in vateres)
        {
            if (!toilet.EstaLimpio) return false;
        }
        return true;
    }

    void OnGUI()
    {
        if (cerca && vaterActual != null && !vaterActual.EstaLimpio)
        {
            GUIStyle estilo = new GUIStyle(GUI.skin.label);
            estilo.fontSize = 20;
            estilo.normal.textColor = Color.white;
            estilo.alignment = TextAnchor.MiddleCenter;

            Rect mensaje = new Rect(Screen.width / 2 - 200, Screen.height - 100, 400, 50);

            if (manteniendo)
            {
                float progreso = contadorMantener / tiempoMantener;
                GUI.Label(mensaje, $"Limpiando váter... {progreso * 100:F0}%", estilo);
            }
            else
            {
                GUI.Label(mensaje, "Mantén E para limpiar váter", estilo);
            }
        }
    }
}
