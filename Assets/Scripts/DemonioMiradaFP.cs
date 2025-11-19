using UnityEngine;
using UnityEngine.AI;

public class DemonioMiradaFP : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;                // Transform del objeto Player (para destino y posición)
    public Camera camaraJugador;             // Cámara en primera persona (FOV y forward reales)
    public GameOverUITMP interfazGameOver;

    [Header("Movimiento")]
    public float velocidadPersecucion = 2f;  // Movimiento lento
    public float distanciaMatar = 1.2f;      // Distancia a la que consideras contacto si no usas trigger

    [Header("Detección de mirada")]
    public float umbralDot = 0.7f;           // Sensibilidad del "me está mirando" (0.7 ~ 45° aprox)
    public float maxDistanciaVista = 30f;    // Hasta dónde puede ver el jugador
    public LayerMask mascaraObstaculos;      // Capas que bloquean la visión (paredes, muebles, etc.)

    private NavMeshAgent agente;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidadPersecucion;
        agente.stoppingDistance = 0f;

        if (camaraJugador == null && Camera.main != null)
            camaraJugador = Camera.main;
    }

    void Update()
    {
        if (jugador == null || camaraJugador == null) return;

        bool meMira = JugadorMeMiraConLineaDeVista();

        if (meMira)
        {
            agente.isStopped = true;
        }
        else
        {
            agente.isStopped = false;
            agente.SetDestination(jugador.position);
        }

        // Opción: matar por distancia si no usas trigger
        if (!meMira && Vector3.Distance(transform.position, jugador.position) <= distanciaMatar)
        {
            ActivarGameOver();
        }
    }

    bool JugadorMeMiraConLineaDeVista()
    {
        // Vector desde la cámara del jugador hacia el demonio
        Vector3 dirHaciaDemonio = (transform.position - camaraJugador.transform.position).normalized;

        // ¿La mirada de la cámara apunta aproximadamente hacia el demonio?
        float dot = Vector3.Dot(camaraJugador.transform.forward, dirHaciaDemonio);
        if (dot < umbralDot) return false; // No lo está mirando dentro del FOV deseado

        // Comprobar línea de vista con raycast: que no haya obstáculos entre cámara y demonio
        float distancia = Vector3.Distance(camaraJugador.transform.position, transform.position);
        if (distancia > maxDistanciaVista) return false;

        if (Physics.Raycast(camaraJugador.transform.position, dirHaciaDemonio, out RaycastHit hit, distancia, mascaraObstaculos, QueryTriggerInteraction.Ignore))
        {
            // Si el primer impacto NO es el demonio, la visión está bloqueada
            if (hit.transform != transform) return false;
        }

        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivarGameOver();
        }
    }

    void ActivarGameOver()
    {
        if (interfazGameOver != null)
        {
            interfazGameOver.ShowGameOverMessage();
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("Interfaz Game Over no asignada.");
        }
    }
}

