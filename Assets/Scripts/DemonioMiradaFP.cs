using UnityEngine;
using UnityEngine.AI;
using System.Linq;

// Demonio que se mueve solo cuando el jugador NO lo está mirando
public class DemonioMiradaFP : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;          // Transform del jugador (posición a seguir)
    public Camera camaraJugador;       // Cámara en primera persona usada para detectar la mirada
    public GameOverUITMP interfazGameOver;

    [Header("Movimiento")]
    public float velocidadPersecucion = 2f;  // Velocidad lenta al perseguir
    public float distanciaMatar = 1.2f;      // Distancia de muerte si no se usa trigger

    [Header("Detección de mirada")]
    public float umbralDot = 0.7f;           // Sensibilidad del ángulo de visión (0.7 ≈ 45°)
    public float maxDistanciaVista = 100f;    // Máxima distancia en la que el jugador puede verlo
    public LayerMask mascaraObstaculos;      // Capas que bloquean la visión


    public Transform[] puntosMirada;
    private NavMeshAgent agente;

    void Start()
    {
        // Obtener NavMeshAgent y configurar movimiento inicial
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidadPersecucion;
        agente.stoppingDistance = 0f;

        // Asigna cámara principal si no se asignó manualmente
        if (camaraJugador == null && Camera.main != null)
            camaraJugador = Camera.main;
    }

    void Update()
    {
        // Si falta referencia, el script no hace nada
        if (jugador == null || camaraJugador == null) return;

        // Comprobar si el jugador está mirando al demonio
        bool meMira = JugadorMeMiraConLineaDeVista();

        // Si el jugador lo mira, el demonio se queda quieto
        if (meMira)
        {
            agente.isStopped = true;
        }
        else
        {
            // Si no lo mira, persigue al jugador
            agente.isStopped = false;
            agente.SetDestination(jugador.position);
        }

        // Matar por distancia si no se usa trigger
        if (!meMira && Vector3.Distance(transform.position, jugador.position) <= distanciaMatar)
        {
            ActivarGameOver();
        }
    }

    // Detecta si el jugador realmente está mirando al demonio con FOV + línea de visión
    bool JugadorMeMiraConLineaDeVista()
{
    foreach (Transform punto in puntosMirada)
    {
        Vector3 dirHaciaPunto = (punto.position - camaraJugador.transform.position).normalized;
        float dot = Vector3.Dot(camaraJugador.transform.forward, dirHaciaPunto);

        if (dot < umbralDot) continue; // fuera del ángulo

        float distancia = Vector3.Distance(camaraJugador.transform.position, punto.position);
        // if (distancia > maxDistanciaVista) continue; // demasiado lejos

        if (Physics.Raycast(
        camaraJugador.transform.position,
        dirHaciaPunto,
        out RaycastHit hit,
        distancia,
        ~0, // ← esto significa "todas las capas"
        QueryTriggerInteraction.Ignore))
{
    // Si lo primero que golpea NO es el demonio, la vista está bloqueada
        if (hit.transform != transform && !puntosMirada.Contains(hit.transform))
            continue;
}

        // Si pasa todas las pruebas para este punto, el jugador lo está mirando
        return true;
    }

    // Si ningún punto cumple las condiciones, no lo está mirando
    return false;
}

    // Si usa colisionador como detección de muerte
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivarGameOver();
        }
    }

    // Llama a la interfaz para mostrar Game Over
    void ActivarGameOver()
    {
        if (interfazGameOver != null)
        {
            interfazGameOver.ShowGameOverMessage();
            Time.timeScale = 0f;  // Pausa el juego
        }
        else
        {
            Debug.LogWarning("Interfaz Game Over no asignada.");
        }
    }
}
