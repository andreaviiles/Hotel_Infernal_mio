using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VentiladorInteract : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject canvasVentilador;   // Canvas del ventilador
    public Slider sliderTiempo;           // Slider para ajustar el tiempo
    public TMP_Text textoTiempo;          // Texto que muestra el tiempo actual
    public TMP_Dropdown dropdownPotencia; // Dropdown para seleccionar la potencia
    public Button botonConfirmar;         // Botón para confirmar

    [Header("Referencias externas")]
    public DemonBehaviour2 demonio2;      // Referencia al demonio
    public PlayerMovement playerMovement; // Referencia al jugador

    [Header("Valores correctos")]
    public int potenciaCorrecta = 3;
    public int tiempoCorrecto = 90;

    private bool abierto = false;

    void Start()
    {
        canvasVentilador.SetActive(false);

        // Configurar listeners
        sliderTiempo.onValueChanged.AddListener(ActualizarTiempo);
        dropdownPotencia.onValueChanged.AddListener(ActualizarPotencia);
        botonConfirmar.onClick.AddListener(ValidarVentilador);

        // Inicializar texto
        ActualizarTiempo(sliderTiempo.value);
        ActualizarPotencia(dropdownPotencia.value);
    }

    void Update()
    {
        // Solo interactúa si no lleva objeto
        if (!abierto && !playerMovement.EstaLlevandoObjeto && DetectarVentilador() && Input.GetKeyDown(KeyCode.E))
        {
            abierto = true;
            canvasVentilador.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Debug.Log("Canvas del ventilador abierto");
        }
    }

    bool DetectarVentilador()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.SphereCast(ray, 0.5f, out hit, 3.5f))
            return hit.collider != null && hit.collider.gameObject == gameObject;

        return false;
    }

    void ActualizarTiempo(float valor)
    {
        textoTiempo.text = valor.ToString("F0") + " min";
    }

    void ActualizarPotencia(int indice)
    {
        textoTiempo.text = textoTiempo.text; // mantener el texto del tiempo
        Debug.Log("Potencia seleccionada: " + dropdownPotencia.options[indice].text);
    }

    public void ValidarVentilador()
    {
        int tiempo = (int)sliderTiempo.value;
        int potencia = int.Parse(dropdownPotencia.options[dropdownPotencia.value].text);

        if (tiempo == tiempoCorrecto && potencia == potenciaCorrecta)
        {
            Debug.Log("Ventilador configurado correctamente");
            CerrarCanvas();
        }
        else
        {
            Debug.Log("Ventilador configurado incorrectamente, demonio enfadado!");
            CerrarCanvas();

            if (demonio2 != null)
                demonio2.ActivarPersecucionRapida();
        }
    }


    void CerrarCanvas()
    {
        canvasVentilador.SetActive(false);
        abierto = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
