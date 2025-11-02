using UnityEngine;
using UnityEngine.AI;

public class DemonBehaviour : MonoBehaviour
{
    public Transform jugador;
    public Transform[] puntosPatrulla;
    public GameOverUITMP interfazGameOver;
    public InteractionDemon scriptInteraccion;

    private NavMeshAgent agente;
    private bool enfadado = false;
    private bool faseFinal = false;

    public float distanciaMinima = 3f;
    private float velocidadNormal;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        velocidadNormal = agente.speed;
        IrAlSiguientePunto();
    }

    void Update()
    {
        if (!enfadado && !faseFinal)
        {
            Patrullar();
        }
        else
        {
            PerseguirJugador();
        }
    }

    public void Enfadar()
    {
        enfadado = true;
        Debug.Log("El demonio se ha enfadado y comienza la persecución.");
    }

    public void Calmar()
    {
        enfadado = false;
        faseFinal = false;
        agente.speed = velocidadNormal;
        agente.stoppingDistance = 0f;

        Vector3 destinoAleatorio = GenerarDestinoAleatorio();
        agente.SetDestination(destinoAleatorio);

        Debug.Log("El demonio se ha calmado y vuelve a patrullar aleatoriamente.");
    }

    public void Teletransportarse()
    {
        Vector3 posicionJugador = jugador.position;
        Vector3 direccionFrontal = jugador.forward;

        Vector3 nuevaPosicion = posicionJugador + direccionFrontal * 1.5f;
        transform.position = nuevaPosicion;
        transform.LookAt(posicionJugador);

        faseFinal = true;
        agente.stoppingDistance = 0f;
        agente.speed = 8f;
        agente.SetDestination(jugador.position);
    }


    void Patrullar()
    {
        if (!agente.hasPath || agente.remainingDistance < 0.5f)
        {
            Vector3 destinoAleatorio = GenerarDestinoAleatorio();
            agente.SetDestination(destinoAleatorio);
        }
    }

    Vector3 GenerarDestinoAleatorio()
    {
        float rango = 10f;
        Vector3 centro = transform.position;

        Vector3 puntoAleatorio = centro + new Vector3(
            Random.Range(-rango, rango),
            0,
            Random.Range(-rango, rango)
        );

        NavMeshHit hit;
        if (NavMesh.SamplePosition(puntoAleatorio, out hit, 2f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }

    void IrAlSiguientePunto()
    {
        if (puntosPatrulla.Length > 0)
        {
            agente.SetDestination(puntosPatrulla[0].position);
        }
    }

    void PerseguirJugador()
    {
        if (jugador == null) return;

        if (faseFinal)
        {
            agente.stoppingDistance = 0f;
            agente.speed = 8f;
        }
        else
        {
            agente.stoppingDistance = distanciaMinima;
            agente.speed = velocidadNormal;
        }

        agente.SetDestination(jugador.position);
    }


    void OnTriggerEnter(Collider otro)
    {
        if (otro.CompareTag("Player") && scriptInteraccion.EstaEnFaseFinal())
        {
            interfazGameOver.ShowGameOverMessage();
            Debug.Log("El demonio ha matado al jugador.");
        }
    }
}
