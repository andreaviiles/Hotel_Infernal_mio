using UnityEngine;

public class ToallaPickup : MonoBehaviour
{
    public PlayerMovement playerMovement;     // Movimiento del jugador
    public Transform toallaAnchor;            // Punto donde se sujeta la toalla
    public GameObject toallaVisualPrefab;     // Prefab visual que aparece al entregar
    public Transform puntoColocacion;         // Lugar exacto de colocación
    public DemonBehaviour2 demonio2;          // Referencia al demonio

    public Camera camaraJugador;              // Cámara del jugador
    public float radioInteraccion = 0.5f;     // Radio del SphereCast
    public float distanciaInteraccion = 3.5f; // Distancia máxima

    private bool recogida = false;            // Si la toalla ya fue recogida
    private bool entregada = false;           // Si ya se entregó
    private bool cerca = false;               // Si estamos mirando la toalla
    private bool cercaEntrega = false;        // Si estamos cerca del punto de entrega

    void Update()
    {
        // Detectar toalla para recoger
        cerca = DetectarToalla();

        // NO SE PUEDE RECOGER SI YA LLEVA OTRO OBJETO
        if (!recogida && !playerMovement.EstaLlevandoObjeto && cerca && Input.GetKeyDown(KeyCode.E))
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
            // Detectamos si estamos cerca de un punto de entrega
            cercaEntrega = DetectarEntrega();

            Collider[] hits = Physics.OverlapSphere(playerMovement.transform.position, 2f);

            foreach (Collider hit in hits)
            {
                // Correcto
                if (hit.CompareTag("EntregaToalla") && Input.GetKeyDown(KeyCode.E))
                {
                    entregada = true;
                    playerMovement.SoltarObjeto();
                    gameObject.SetActive(false);

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
    }

    // Detectar si miramos la toalla
    bool DetectarToalla()
    {
        Ray ray = new Ray(camaraJugador.transform.position, camaraJugador.transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, radioInteraccion, out hit, distanciaInteraccion))
            return hit.collider != null && hit.collider.gameObject == gameObject;
        return false;
    }

    // Detectar si estamos cerca de un punto de entrega
    bool DetectarEntrega()
    {
        Collider[] hits = Physics.OverlapSphere(playerMovement.transform.position, 2f);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("EntregaToalla") || hit.CompareTag("EntregaToallaWrong"))
                return true;
        }
        return false;
    }

    public bool ToallaEntregada => entregada;

    // GUI para mostrar mensajes en pantalla
    void OnGUI()
    {
        GUIStyle estilo = new GUIStyle(GUI.skin.label);
        estilo.fontSize = 40;
        estilo.normal.textColor = Color.white;
        estilo.alignment = TextAnchor.MiddleCenter;
        Rect mensaje = new Rect(Screen.width / 2 - 200, Screen.height - 120, 400, 80);

        // Mensaje al recoger
        if (cerca && !recogida)
            GUI.Label(mensaje, "Pulsa E para recoger toalla", estilo);

        // Mensaje al entregar
        if (recogida && !entregada && cercaEntrega)
            GUI.Label(mensaje, "Pulsa E para entregar toalla", estilo);
    }
}
