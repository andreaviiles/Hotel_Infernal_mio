using UnityEngine;

public class InteractionDemon : MonoBehaviour
{
    public float tiempoLimite = 60f;
    public float tiempoFinal = 90f;
    public DemonBehaviour demonio;
    public Renderer cuboRenderer;
    public Camera camaraJugador;
    public float distanciaInteraccion = 2f;

    private float tiempoActual = 0f;
    private bool faseFinal = false;
    private bool jugadorCerca = false;
    private bool cronometroDetenido = false;

    void Start()
    {
        tiempoActual = 0f;
        cuboRenderer.material.color = Color.white;

        if (camaraJugador == null)
            Debug.LogError("Asigna la cámara del jugador en el inspector.");
    }

    void Update()
    {
        RaycastHit hit;
        jugadorCerca = false;

        if (Physics.Raycast(camaraJugador.transform.position, camaraJugador.transform.forward, out hit, distanciaInteraccion))
        {
            if (hit.collider.gameObject == gameObject)
            {
                jugadorCerca = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    cuboRenderer.material.color = Color.green;

                    if (!faseFinal)
                    {
                        cronometroDetenido = true;
                        demonio.Calmar();
                        Debug.Log("Cubo tocado: demonio calmado y cronómetro detenido.");
                    }
                    else
                    {
                        Debug.Log("Cubo tocado, pero el demonio ya no se calma.");
                    }
                }
            }
        }

        // Temporizador demonio
        if (!cronometroDetenido && !faseFinal)
        {
            tiempoActual += Time.deltaTime;

            if (tiempoActual >= tiempoLimite)
            {
                demonio.Enfadar();
            }

            if (tiempoActual >= tiempoFinal)
            {
                faseFinal = true;
                demonio.Teletransportarse();
            }
        }
    }

    public bool EstaEnFaseFinal()
    {
        return faseFinal;
    }

    public bool IsPlayerNear() => jugadorCerca;
    public float GetRemainingTime() => Mathf.Max(0f, tiempoFinal - tiempoActual);

}



