using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DemonBehaviour2 : MonoBehaviour
{
    public Transform jugador;
    public float velocidadRotacion = 3f;
    public GameOverUITMP interfazGameOver;

    private NavMeshAgent agente;
    private bool enfadado = false;
    private Vector3 puntoOrigen;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        if (agente != null)
        {
            agente.isStopped = true;
            agente.stoppingDistance = 0f;
            agente.autoBraking = false;
            agente.updateRotation = true;
        }
        puntoOrigen = transform.position;
    }

    void Update()
    {
        if (jugador != null)
        {
            Vector3 direccion = jugador.position - transform.position;
            direccion.y = 0;
            if (direccion != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
            }

            if (enfadado && agente != null)
            {
                agente.isStopped = false;
                agente.SetDestination(jugador.position);
            }
        }
    }

    public void ActivarPersecucionRapida()
    {
        if (agente != null && jugador != null)
        {
            agente.speed = 25f;
            enfadado = true;
            StartCoroutine(CalmarDespuesDeTiempo(10f));
        }
    }

    private IEnumerator CalmarDespuesDeTiempo(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        enfadado = false;
        if (agente != null)
        {
            agente.speed = 5f; 
            agente.isStopped = false;
            agente.SetDestination(puntoOrigen); 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (enfadado && other.CompareTag("Player"))
        {
            if (interfazGameOver != null)
            {
                interfazGameOver.ShowGameOverMessage();
                Time.timeScale = 0f;
            }
        }
    }
}

