using UnityEngine;
using UnityEngine.AI;
using System.Collections;

// Demonio que rota hacia el jugador y persigue con precision cuando esta enfadado
public class DemonBehaviour2 : MonoBehaviour
{
    // Referencias
    public Transform jugador;
    public GameOverUITMP interfazGameOver;
    public TelefonoInteract telefono;

    // Rotacion
    public float velocidadRotacion = 6f; // rotacion mas rapida para apuntar al jugador

    // NavMesh
    private NavMeshAgent agente;

    // Estado
    private bool enfadado = false;
    private Vector3 puntoOrigen;

    // Parametros de captura
    public float distanciaMatar = 0.6f; // distancia exacta para matar

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        if (agente != null)
        {
            // Configuracion para parada precisa en el objetivo
            agente.isStopped = true;           // inicia quieto
            agente.stoppingDistance = 0.5f;    // margen para no atravesar al jugador
            agente.autoBraking = true;         // imprescindible para frenar en el destino
            agente.updateRotation = false;     // rotacion manual para mayor control
            agente.updatePosition = true;      // que el agente actualice su posicion
            agente.acceleration = 200f;        // respuesta inmediata
            agente.angularSpeed = 720f;        // giros rapidos
            agente.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        }

        puntoOrigen = transform.position;
    }

    void Update()
    {
        if (jugador == null || agente == null) return;

        // Rotar siempre hacia el jugador
        Vector3 dir = jugador.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion rotObjetivo = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotObjetivo, velocidadRotacion * 90f * Time.deltaTime);
        }

        // Perseguir con recalculo continuo del destino
        if (enfadado)
        {
            agente.isStopped = false;
            agente.speed = 25f;                    // velocidad alta solicitada
            agente.SetDestination(jugador.position); // actualiza destino cada frame

            // Si esta dentro de la distancia de matar, activa GameOver
            float dist = Vector3.Distance(transform.position, jugador.position);
            if (dist <= distanciaMatar)
            {
                ActivarGameOver();
                return;
            }

            // Si ya alcanzo el stoppingDistance, frena para no pasarse
            if (dist <= agente.stoppingDistance + 0.05f)
            {
                agente.isStopped = true; // detener en el borde
            }
        }
    }

    // Activa persecucion rapida durante un tiempo limitado
    public void ActivarPersecucionRapida()
    {
        if (agente != null && jugador != null)
        {
            enfadado = true;
            agente.isStopped = false;
            agente.speed = 200f;

            // Opcional: calma despues de 10 segundos
            StartCoroutine(CalmarDespuesDeTiempo(15f));
        }
    }

    private IEnumerator CalmarDespuesDeTiempo(float segundos)
    {
        yield return new WaitForSeconds(segundos);

        enfadado = false;

        if (agente != null)
        {
            agente.speed = 5f;              // velocidad normal
            agente.isStopped = false;
            agente.autoBraking = true;      // mantener frenado en destino
            agente.SetDestination(puntoOrigen);
        }
    }

    // Kill adicional por trigger si tienes collider con isTrigger
    void OnTriggerEnter(Collider other)
    {
        if (enfadado && other.CompareTag("Player"))
        {
            ActivarGameOver();
        }
    }

    // Lanza GameOver y cierra el telefono si estaba abierto
    void ActivarGameOver()
    {
        // Cerrar canvas del telefono si estaba abierto
        if (telefono != null)
        {
            telefono.CerrarCanvas();
        }

        if (interfazGameOver != null)
        {
            interfazGameOver.ShowGameOverMessage();
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("Interfaz Game Over no asignada en DemonBehaviour2.");
        }
    }
}

