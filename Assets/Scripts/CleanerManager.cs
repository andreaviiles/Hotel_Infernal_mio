using UnityEngine;
using System.Collections.Generic;

public class CleanerManager : MonoBehaviour
{
    public float tiempoLimite = 80f;
    public float tiempoReinicio = 60f;
    public Camera playerCamera;
    public GameObject[] cubosLimpiables;

    private Dictionary<GameObject, int> interacciones = new Dictionary<GameObject, int>();
    private float temporizador;
    private float tiempoEspera;
    private bool limpiezaActiva = true;
    private GameObject cuboActual;
    private bool cerca = false;

    void Start()
    {
        temporizador = tiempoLimite;

        foreach (GameObject cubo in cubosLimpiables)
        {
            cubo.SetActive(true);
            interacciones[cubo] = 0;

            if (!cubo.TryGetComponent(out CleanerBehavior behavior))
                cubo.AddComponent<CleanerBehavior>();
        }
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f))
        {
            if (hit.collider.CompareTag("Cleanable"))
            {
                cuboActual = hit.collider.gameObject;
                cerca = true;
            }
            else
            {
                cerca = false;
            }
        }
        else
        {
            cerca = false;
        }

        if (limpiezaActiva)
        {
            temporizador -= Time.deltaTime;

            if (cerca && Input.GetKeyDown(KeyCode.E) && cuboActual != null)
            {
                if (interacciones.ContainsKey(cuboActual))
                {
                    interacciones[cuboActual]++;
                    Debug.Log($"Cubo {cuboActual.name}: {interacciones[cuboActual]} interacciones.");

                    if (interacciones[cuboActual] >= 3)
                    {
                        cuboActual.SetActive(false);
                        interacciones[cuboActual] = 0;
                    }

                    if (TodosCubosDesactivados())
                    {
                        limpiezaActiva = false;
                        tiempoEspera = tiempoReinicio;
                        Debug.Log("¡Todos los cubos han sido limpiados!");
                    }
                }
            }

            if (temporizador <= 0f)
            {
                limpiezaActiva = false;
                tiempoEspera = tiempoReinicio;
                Debug.Log("¡Tiempo agotado! Los cubos reaparecerán en 1 minuto.");
            }
        }
        else
        {
            tiempoEspera -= Time.deltaTime;

            if (tiempoEspera <= 0f)
            {
                ReiniciarCiclo();
            }
        }
    }

    bool TodosCubosDesactivados()
    {
        foreach (GameObject cubo in cubosLimpiables)
        {
            if (cubo.activeSelf)
                return false;
        }
        return true;
    }

    void ReiniciarCiclo()
    {
        foreach (GameObject cubo in cubosLimpiables)
        {
            cubo.SetActive(true);
            interacciones[cubo] = 0;
        }

        temporizador = tiempoLimite;
        limpiezaActiva = true;
        Debug.Log("¡Los cubos han reaparecido! Comienza un nuevo ciclo.");
    }

    void OnGUI()
    {
        GUIStyle estilo = new GUIStyle(GUI.skin.label);
        estilo.fontSize = 20;
        estilo.normal.textColor = Color.white;
        estilo.alignment = TextAnchor.UpperCenter;

        Rect rect = new Rect(Screen.width / 2 - 100, 20, 200, 30);
        if (limpiezaActiva)
            GUI.Label(rect, $"Tiempo restante: {temporizador:F1}s", estilo);
        else
            GUI.Label(rect, $"Reinicio en: {tiempoEspera:F1}s", estilo);

        if (cerca && limpiezaActiva)
        {
            Rect mensaje = new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 50);
            GUI.Label(mensaje, "Presiona E para limpiar cubo", estilo);
        }
    }
}

