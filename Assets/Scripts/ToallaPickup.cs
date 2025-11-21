//ESTE SCRIPT ES PARA PRUEBAS, SE TIENE QUE BORRAR MAS ADELANTE

//ESTE SCRIPT ES PARA PRUEBAS, SE TIENE QUE BORRAR MAS ADELANTE

using UnityEngine;

public class ToallaPickup : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Transform toallaAnchor;
    public GameObject toallaVisualPrefab;
    public Transform puntoColocacion;
    public DemonBehaviour2 demonio2;

    public Camera camaraJugador;
    public float radioInteraccion = 0.5f;
    public float distanciaInteraccion = 3.5f;

    private bool recogida = false;
    private bool entregada = false;

    void Update()
    {
        // NO SE PUEDE RECOGER SI YA LLEVA OTRO OBJETO
        if (!recogida && !playerMovement.EstaLlevandoObjeto && DetectarToalla() && Input.GetKeyDown(KeyCode.E))
        {
            recogida = true;

            // La toalla SÍ reduce velocidad → reduceVelocidad = true
            playerMovement.LlevarObjeto(true, true);

            // Colocar en mano
            transform.SetParent(toallaAnchor);
            transform.localPosition = new Vector3(0, -0.4f, 0);
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * 0.5f;

            GetComponent<Collider>().enabled = false;

            Debug.Log("Toalla recogida");
        }

        // Entregar toalla
        if (recogida && !entregada)
        {
            Collider[] hits = Physics.OverlapSphere(playerMovement.transform.position, 2f);

            foreach (Collider hit in hits)
            {
                // Correcto
                if (hit.CompareTag("EntregaToalla") && Input.GetKeyDown(KeyCode.E))
                {
                    entregada = true;

                    // Dejar objeto
                    playerMovement.SoltarObjeto();

                    gameObject.SetActive(false);

                    // Colocar toalla visual
                    if (toallaVisualPrefab != null && puntoColocacion != null)
                    {
                        Vector3 offset = new Vector3(0, 0.5f, 0);
                        Instantiate(toallaVisualPrefab, puntoColocacion.position + offset, puntoColocacion.rotation);
                    }

                    Debug.Log("Toalla entregada correctamente");
                }

                // Incorrecto
                if (hit.CompareTag("EntregaToallaWrong") && Input.GetKeyDown(KeyCode.E))
                {
                    entregada = true;
                    playerMovement.SoltarObjeto();
                    gameObject.SetActive(false);

                    Debug.Log("Toalla entregada en el sitio equivocado, Demonio enfadado!");

                    if (demonio2 != null)
                        demonio2.ActivarPersecucionRapida();
                }
            }
        }

        bool DetectarToalla()
        {
            Ray ray = new Ray(camaraJugador.transform.position, camaraJugador.transform.forward);
            RaycastHit hit;

            if (Physics.SphereCast(ray, radioInteraccion, out hit, distanciaInteraccion))
                return hit.collider != null && hit.collider.gameObject == gameObject;

            return false;
        }
    }

    public bool ToallaEntregada => entregada;
}