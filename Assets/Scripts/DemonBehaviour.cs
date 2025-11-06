using UnityEngine;
using UnityEngine.AI;

public class DemonBehaviour : MonoBehaviour
{
    public Transform jugador;
    public GameOverUITMP interfazGameOver;

    public Transform[] puntosPatrulla;

    public float velocidadNormal = 5.5f;
    public float velocidadMatar = 8f;
    public float distanciaMinima = 6f;

    private NavMeshAgent agente;
    private bool enfadado = false;
    private bool faseFinal = false;
    private int puntoActual = -1;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        agente.speed = velocidadNormal;
        agente.stoppingDistance = distanciaMinima;
        MoverAPuntoDePatrulla();
    }

    void Update()
    {
        if (!enfadado && !faseFinal)
        {
            if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
            {
                MoverAPuntoDePatrulla();
            }
        }

        if ((enfadado || faseFinal) && jugador != null)
        {
            if (Vector3.Distance(agente.destination, jugador.position) > 1f)
            {
                agente.SetDestination(jugador.position);
            }
        }
    }

    void MoverAPuntoDePatrulla()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length == 0) return;

        int nuevoIndice;
        do
        {
            nuevoIndice = Random.Range(0, puntosPatrulla.Length);
        } while (nuevoIndice == puntoActual && puntosPatrulla.Length > 1);

        puntoActual = nuevoIndice;
        agente.SetDestination(puntosPatrulla[puntoActual].position);
        Debug.Log($"Demonio patrullando hacia: {puntosPatrulla[puntoActual].name}");
    }

    public void ActivarPersecucionSuave()
    {
        enfadado = true;
        faseFinal = false;
        agente.speed = velocidadNormal;
        agente.stoppingDistance = distanciaMinima;
        Debug.Log("Demonio en modo persecución suave.");
    }

    public void ActivarModoMatar()
    {
        faseFinal = true;
        agente.speed = velocidadMatar;
        agente.stoppingDistance = 0f;
        Teletransportarse();
        Debug.Log("Demonio en modo matar.");
    }

    public void Calmar()
    {
        if (faseFinal)
        {
            Debug.Log("No se puede calmar al demonio en modo matar.");
            return;
        }

        if (!enfadado)
        {
            Debug.Log("Demonio ya está calmado.");
            return;
        }

        enfadado = false;
        agente.speed = velocidadNormal;
        agente.stoppingDistance = distanciaMinima;
        MoverAPuntoDePatrulla();
        Debug.Log("Demonio calmado: vuelve a patrullar.");
    }

    public void Teletransportarse()
    {
        if (jugador != null)
        {
            Vector3 destino = jugador.position + jugador.forward * 1.5f;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(destino, out hit, 5f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                transform.LookAt(jugador.position);
                agente.SetDestination(jugador.position);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (faseFinal && other.CompareTag("Player"))
        {
            Debug.Log("Trigger detectado con el jugador.");

            if (interfazGameOver != null)
            {
                interfazGameOver.ShowGameOverMessage();
                Time.timeScale = 0f;
                Debug.Log("El demonio ha atrapado al jugador.");
            }
            else
            {
                Debug.LogWarning("Interfaz Game Over no asignada.");
            }
        }
    }
}






