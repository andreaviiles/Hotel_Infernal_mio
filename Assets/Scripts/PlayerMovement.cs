using UnityEngine;
using System.Collections.Generic;

// Controla el movimiento del jugador incluyendo caminar, correr, salto,
// doble salto y dash. Ahora también gestiona si el objeto que lleva
// reduce o no la velocidad.
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;  // Referencia al CharacterController
    public float speed = 5f;                // Velocidad normal
    public float sprintSpeed = 8f;          // Velocidad al correr (Shift)
    public float gravity = -20f;            // Fuerza de gravedad
    public float jump = 3.2f;               // Fuerza del salto
    public float doubleJump = 0.6f;         // Fuerza del doble salto
    public float doubleTap = 0.3f;          // Tiempo para doble pulsación (dash)

    public float dashSpeed = 14f;           // Velocidad del dash
    public float dashDuration = 0.4f;       // Duración del dash

    private Vector3 velocity;               // Velocidad vertical
    private bool enSuelo;                   // Si está tocando el suelo
    private int jumpCont = 0;               // Contador para doble salto
    private float lastJump = 0f;            // Tiempo del último salto

    private bool isDashing = false;         // Si está haciendo dash
    private float dashTimer = 0f;           // Temporizador del dash
    private Vector3 dashDirection;          // Dirección del dash
    private KeyCode dashKey;                // Tecla asociada al dash

    private Vector3 moveInput;              // Movimiento horizontal
    private Dictionary<KeyCode, float> lastKeyPress = new Dictionary<KeyCode, float>();

    private bool llevaObjeto = false;       // Si el jugador lleva algo
    private bool objetoPesa = true;         // Si el objeto reduce velocidad

    // NUEVO — Permite indicar si el objeto que llevas reduce velocidad o no
    public void LlevarObjeto(bool estado, bool reduceVelocidad)
    {
        llevaObjeto = estado;
        objetoPesa = reduceVelocidad;
    }

    // Atajo para saber si lleva algo
    public bool EstaLlevandoObjeto => llevaObjeto;

    // Llamado desde los scripts cuando se entrega un objeto
    public void SoltarObjeto()
    {
        llevaObjeto = false;
        objetoPesa = false;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Verificar si está en el suelo
        enSuelo = controller.isGrounded;
        if (enSuelo && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpCont = 0;
        }

        // Dash solo si NO lleva objeto
        if (!llevaObjeto)
        {
            DetectDash(KeyCode.W, transform.forward);
            DetectDash(KeyCode.S, -transform.forward);
            DetectDash(KeyCode.A, -transform.right);
            DetectDash(KeyCode.D, transform.right);
        }

        // Movimiento horizontal
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.W)) z = 1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;

        moveInput = (transform.right * x + transform.forward * z).normalized;

        // Si está haciendo dash, ignorar movimiento normal
        if (isDashing)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f || !Input.GetKey(dashKey))
                isDashing = false;
        }
        else
        {
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;

            // REDUCCIÓN DE VELOCIDAD SOLO SI EL OBJETO PESA
            if (llevaObjeto && objetoPesa)
                currentSpeed *= 0.6f;

            controller.Move(moveInput * currentSpeed * Time.deltaTime);
        }

        // SALTO + DOBLE SALTO
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float timeSinceLastJump = Time.time - lastJump;
            lastJump = Time.time;

            if (jumpCont < 2)
            {
                if (jumpCont == 1 && timeSinceLastJump > doubleTap)
                    return;

                float jumpForce = Mathf.Sqrt(jump * -2f * gravity);

                if (jumpCont == 1)
                    jumpForce *= doubleJump;

                velocity.y = jumpForce;
                jumpCont++;
            }
        }

        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        if (velocity.y < -50f)
            velocity.y = -50f;

        controller.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }

    // Detectar doble pulsación para dash
    void DetectDash(KeyCode key, Vector3 direction)
    {
        if (Input.GetKeyDown(key))
        {
            if (lastKeyPress.ContainsKey(key) && Time.time - lastKeyPress[key] < doubleTap)
                StartDash(direction, key);

            lastKeyPress[key] = Time.time;
        }
    }

    // Iniciar dash
    void StartDash(Vector3 direction, KeyCode key)
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashDirection = direction;
        dashKey = key;
    }
}
