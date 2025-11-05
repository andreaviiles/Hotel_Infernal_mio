using UnityEngine;

public class ToallaPickup : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Transform toallaAnchor;
    public GameObject toallaVisualPrefab;
    public Transform puntoColocacion;

    private bool recogida = false;
    private bool entregada = false;
    private bool jugadorCerca = false;
    private bool cercaEntrega = false;

    void Update()
    {
        // Recoger la toalla
        if (jugadorCerca && !recogida && Input.GetKeyDown(KeyCode.E))
        {
            recogida = true;
            playerMovement.LlevarObjeto(true);

            transform.SetParent(toallaAnchor);
            transform.localPosition = new Vector3(0, -0.4f, 0);
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * 0.5f;

            GetComponent<Collider>().enabled = false;
            Debug.Log("Toalla recogida");
        }

        // Detectar si estamos cerca de la zona de entrega
        if (recogida && !entregada)
        {
            cercaEntrega = false;

            Collider[] hits = Physics.OverlapSphere(playerMovement.transform.position, 2f);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("EntregaToalla"))
                {
                    cercaEntrega = true;

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        entregada = true;
                        playerMovement.LlevarObjeto(false);
                        gameObject.SetActive(false);

                        if (toallaVisualPrefab != null && puntoColocacion != null)
                        {
                            Instantiate(toallaVisualPrefab, puntoColocacion.position, puntoColocacion.rotation);
                        }

                        Debug.Log("Toalla entregada y colocada en el baño");
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            jugadorCerca = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            jugadorCerca = false;
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 20;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;

        if (jugadorCerca && !recogida)
        {
            Rect rect = new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 50);
            GUI.Label(rect, "Presiona E para recoger la toalla", style);
        }

        if (recogida && cercaEntrega && !entregada)
        {
            Rect rect = new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 50);
            GUI.Label(rect, "Presiona E para entregar la toalla", style);
        }
    }
}


