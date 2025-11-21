using UnityEngine;

public class PatitoPickup : MonoBehaviour
{
    public PlayerMovement playerMovement;     // Movimiento del jugador
    public Transform patitoAnchor;            // Punto donde se sujeta el patito
    public GameObject patitoVisualPrefab;     // Patito que aparece en la bañera
    public Transform puntoColocacion;         // Lugar exacto de colocación

    public Camera camaraJugador;              // Cámara del jugador
    public float radioInteraccion = 0.5f;
    public float distanciaInteraccion = 3.5f;

    private bool recogido = false;
    private bool entregado = false;

    void Update()
    {
        // NO SE PUEDE RECOGER SI YA LLEVA OTRO OBJETO
        if (!recogido && !playerMovement.EstaLlevandoObjeto && DetectarPatito() && Input.GetKeyDown(KeyCode.E))
        {
            recogido = true;

            // El patito NO reduce velocidad 
            playerMovement.LlevarObjeto(true, false);

            // Colocar en la mano
            transform.SetParent(patitoAnchor);
            transform.localPosition = new Vector3(0, -0.4f, 0);
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * 0.5f;

            GetComponent<Collider>().enabled = false;

            Debug.Log("Patito recogido");
        }

        // Entregar patito en la bañera
        if (recogido && !entregado)
        {
            Collider[] hits = Physics.OverlapSphere(playerMovement.transform.position, 2f);

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Bañera") && Input.GetKeyDown(KeyCode.E))
                {
                    entregado = true;

                    // El jugador deja de llevar objeto
                    playerMovement.SoltarObjeto();

                    gameObject.SetActive(false);

                    // Crear el patito visual
                    if (patitoVisualPrefab != null && puntoColocacion != null)
                    {
                        Vector3 offset = new Vector3(0, 0.5f, 0);
                        Instantiate(patitoVisualPrefab, puntoColocacion.position + offset, puntoColocacion.rotation);
                    }

                    Debug.Log("Patito entregado");
                }
            }
        }
    }

    // Detectar si miramos al patito
    bool DetectarPatito()
    {
        Ray ray = new Ray(camaraJugador.transform.position, camaraJugador.transform.forward);
        RaycastHit hit;

        if (Physics.SphereCast(ray, radioInteraccion, out hit, distanciaInteraccion))
            return hit.collider != null && hit.collider.gameObject == gameObject;

        return false;
    }

    public bool PatitoEntregado => entregado;
}
