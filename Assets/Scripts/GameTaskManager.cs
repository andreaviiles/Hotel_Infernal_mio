using UnityEngine;
using UnityEngine.SceneManagement;

// Gestiona el tiempo general, las tareas y el comportamiento del demonio
public class GameTaskManager : MonoBehaviour
{
    [Header("Referencias Tareas")]
    public BedTaskManager bedTaskManager;         // Manager de la tarea de la cama
    public DemonBehaviour demonBehaviour;         // Comportamiento del demonio

    [Header("Interfaz de usuario")]
    public GameObject juegoFinalizadoCanvas;      // Canvas de victoria
    public GameOverUITMP interfazGameOver;        // Canvas de derrota
    public GameObject Indice;                     // Canvas del checklist
    public GameObject TelefonoCanvas;
    public GameObject VentiladorCanvas;
    public GameObject TermometroCanvas;

    [Header("Temporizadores")]
    public float tiempoDemonio = 210f;            // 3.5 minutos para tareas del demonio
    public float tiempoGeneral = 480f;            // 8 minutos para todas las tareas

    // Variables internas de tiempo
    private float tiempoRestanteDemonio;
    private float tiempoRestanteGeneral;
    private bool persecucionActivada = false;     // Si el demonio esta en persecucion suave
    private bool modoMatarActivado = false;       // Si el demonio esta en modo matar
    private bool demonioCalmado = false;          // Si el demonio fue calmado
    private bool juegoGanado = false;             // Si el jugador gano
    private bool juegoPerdido = false;            // Si el jugador perdio




    void Start()
    {
        // Inicializar temporizadores
        tiempoRestanteDemonio = tiempoDemonio;
        tiempoRestanteGeneral = tiempoGeneral;

        // Ocultar canvas de victoria al inicio
        if (juegoFinalizadoCanvas != null)
            juegoFinalizadoCanvas.SetActive(false);
    }

    void Update()
    {
        // Si el juego termino, no hacer nada
        if (juegoGanado || juegoPerdido) return;

        // Reducir tiempo solo si no se gano
        if (!juegoGanado)
        {
            tiempoRestanteDemonio -= Time.deltaTime;
            tiempoRestanteGeneral -= Time.deltaTime;
        }

        // Verificar condicion de victoria
        if (TodasLasTareasCompletadas() && !juegoGanado)
        {
            juegoGanado = true;
            MostrarVictoria();
            return;
        }

        // Verificar condicion de derrota por tiempo
        if (tiempoRestanteGeneral <= 0f && !juegoPerdido)
        {
            juegoPerdido = true;
            MostrarDerrota("Tiempo agotado");
            return;
        }

        // Logica del demonio (solo si no esta calmado y no se gano)
        if (!demonioCalmado && !juegoGanado)
        {
            VerificarTareasDemonio();
        }
    }

    // Verifica las tareas especificas del demonio (cama y latas)
    void VerificarTareasDemonio()
    {
        bool latasOk = TrashPickUp.TareaCompletada();
        bool camaOk = bedTaskManager != null && bedTaskManager.TareaCompletada();
        bool tareasDemonioCompletas = latasOk && camaOk;

        // Si se completan las tareas del demonio, calmarlo
        if (tareasDemonioCompletas && !demonioCalmado)
        {
            demonioCalmado = true;
            if (demonBehaviour != null)
            {
                demonBehaviour.Calmar();
            }
        }

        // Si no se completan a tiempo, activar comportamientos del demonio
        if (!tareasDemonioCompletas && !demonioCalmado)
        {
            // A 60 segundos: activar persecucion suave
            if (tiempoRestanteDemonio <= 60f && !persecucionActivada)
            {
                demonBehaviour.ActivarPersecucionSuave();
                persecucionActivada = true;
            }

            // A 0 segundos: activar modo matar
            if (tiempoRestanteDemonio <= 0f && !modoMatarActivado)
            {
                demonBehaviour.ActivarModoMatar();
                modoMatarActivado = true;
            }
        }
    }

    // Verifica si todas las 12 tareas estan completadas
    bool TodasLasTareasCompletadas()
    {
        Debug.Log("=== INICIANDO VERIFICACIÓN DE TAREAS ===");

        // 1. Latas (5)
        bool latasOk = TrashPickUp.TareaCompletada();
        Debug.Log($"Latas completadas: {latasOk}");
        if (!latasOk) return false;

        // 2. Cama
        bool camaOk = bedTaskManager != null && bedTaskManager.TareaCompletada();
        Debug.Log($"Cama completada: {camaOk}");
        if (!camaOk) return false;

        // 3. Toalla - USAR MÉTODO ESTÁTICO
        bool toallaOk = ToallaPickup.TareaCompletadaStatic();
        Debug.Log($"Toalla completada: {toallaOk}");
        if (!toallaOk) return false;

        // 4. Patito
        bool patitoOk = PatitoPickup.TareaCompletada();
        Debug.Log($"Patitos completados: {patitoOk} (Entregados: {PatitoPickup.patitosEntregados}/{PatitoPickup.totalPatitos})");
        if (!patitoOk) return false;

        // 5. Limpieza
        CleanerManager limpieza = FindFirstObjectByType<CleanerManager>();
        bool limpiezaOk = limpieza != null && limpieza.TareaCompletada();
        Debug.Log($"Limpieza completada: {limpiezaOk} (Limpieza encontrada: {limpieza != null})");
        if (!limpiezaOk) return false;

        // 6. Váteres
        ToiletTaskManager vateres = FindFirstObjectByType<ToiletTaskManager>();
        bool vateresOk = vateres != null && vateres.TareaCompletada();
        Debug.Log($"Váteres completados: {vateresOk} (Váteres encontrados: {vateres != null})");
        if (!vateresOk) return false;

        // 7. Grifos
        FaucetTaskManager grifos = FindFirstObjectByType<FaucetTaskManager>();
        bool grifosOk = grifos != null && grifos.TareaCompletada();
        Debug.Log($"Grifos completados: {grifosOk} (Grifos encontrados: {grifos != null})");
        if (!grifosOk) return false;

        // 8. Cuadros
        FrameTaskManager cuadros = FindFirstObjectByType<FrameTaskManager>();
        bool cuadrosOk = cuadros != null && cuadros.TareaCompletada();
        Debug.Log($"Cuadros completados: {cuadrosOk} (Cuadros encontrados: {cuadros != null})");
        if (!cuadrosOk) return false;

        // 9. Lámparas
        LampTaskManager lamparas = FindFirstObjectByType<LampTaskManager>();
        bool lamparasOk = lamparas != null && lamparas.TareaCompletada();
        Debug.Log($"Lámparas completadas: {lamparasOk} (Lámparas encontradas: {lamparas != null})");
        if (!lamparasOk) return false;

        // 10. Teléfono
        TelefonoInteract telefono = FindFirstObjectByType<TelefonoInteract>();
        bool telefonoOk = telefono != null && telefono.TareaCompletada();
        Debug.Log($"Teléfono completado: {telefonoOk} (Teléfono encontrado: {telefono != null})");
        if (!telefonoOk) return false;

        // 11. Termómetro
        TermometroInteract termometro = FindFirstObjectByType<TermometroInteract>();
        bool termometroOk = termometro != null && termometro.TareaCompletada();
        Debug.Log($"Termómetro completado: {termometroOk} (Termómetro encontrado: {termometro != null})");
        if (!termometroOk) return false;

        // 12. Ventilador
        VentiladorInteract ventilador = FindFirstObjectByType<VentiladorInteract>();
        bool ventiladorOk = ventilador != null && ventilador.TareaCompletada();
        Debug.Log($"Ventilador completado: {ventiladorOk} (Ventilador encontrado: {ventilador != null})");
        if (!ventiladorOk) return false;

        Debug.Log("=== ¡TODAS LAS TAREAS COMPLETADAS! ===");
        return true;
    }

    // Cuenta cuantas tareas estan completadas
    int ContarTareasCompletadas()
    {
        int contador = 0;

        if (TrashPickUp.TareaCompletada())
        {
            contador++;
            Debug.Log("✅ Latas contadas");
        }

        if (bedTaskManager != null && bedTaskManager.TareaCompletada())
        {
            contador++;
            Debug.Log("✅ Cama contada");
        }

        // Toalla - USAR MÉTODO ESTÁTICO
        if (ToallaPickup.TareaCompletadaStatic())
        {
            contador++;
            Debug.Log("✅ Toalla contada");
        }

        if (PatitoPickup.TareaCompletada())
        {
            contador++;
            Debug.Log($"✅ Patitos contados: {PatitoPickup.patitosEntregados}/{PatitoPickup.totalPatitos}");
        }

        CleanerManager limpieza = FindFirstObjectByType<CleanerManager>();
        if (limpieza != null && limpieza.TareaCompletada())
        {
            contador++;
            Debug.Log("✅ Limpieza contada");
        }

        ToiletTaskManager vateres = FindFirstObjectByType<ToiletTaskManager>();
        if (vateres != null && vateres.TareaCompletada())
        {
            contador++;
            Debug.Log("✅ Váteres contados");
        }

        FaucetTaskManager grifos = FindFirstObjectByType<FaucetTaskManager>();
        if (grifos != null && grifos.TareaCompletada())
        {
            contador++;
            Debug.Log("✅ Grifos contados");
        }

        FrameTaskManager cuadros = FindFirstObjectByType<FrameTaskManager>();
        if (cuadros != null && cuadros.TareaCompletada())
        {
            contador++;
            Debug.Log("✅ Cuadros contados");
        }

        LampTaskManager lamparas = FindFirstObjectByType<LampTaskManager>();
        if (lamparas != null && lamparas.TareaCompletada())
        {
            contador++;
            Debug.Log("✅ Lámparas contadas");
        }

        TelefonoInteract telefono = FindFirstObjectByType<TelefonoInteract>();
        if (telefono != null && telefono.TareaCompletada())
        {
            contador++;
            Debug.Log("✅ Teléfono contado");
        }

        TermometroInteract termometro = FindFirstObjectByType<TermometroInteract>();
        if (termometro != null && termometro.TareaCompletada())
        {
            contador++;
            Debug.Log("✅ Termómetro contado");
        }

        VentiladorInteract ventilador = FindFirstObjectByType<VentiladorInteract>();
        if (ventilador != null && ventilador.TareaCompletada())
        {
            contador++;
            Debug.Log("✅ Ventilador contado");
        }

        Debug.Log($"🔢 Total tareas completadas: {contador}/12");
        return contador;
    }

    // Muestra la pantalla de victoria
    void MostrarVictoria()
    {
        Time.timeScale = 0f; // Pausar el juego
        if (juegoFinalizadoCanvas != null)
        {
            juegoFinalizadoCanvas.SetActive(true);
        }
        Debug.Log("¡VICTORIA! Todas las tareas completadas.");
    }

    // Muestra la pantalla de derrota
    public void MostrarDerrota(string motivo)
    {
        juegoPerdido = true;
        Time.timeScale = 0f; // Pausar el juego
        if (interfazGameOver != null)
        {
            interfazGameOver.ShowGameOverMessage();
        }
        Debug.Log("DERROTA: " + motivo);
    }

    // Llamado cuando el demonio mata al jugador
    public void JugadorMuertoPorDemonio()
    {
        if (!juegoGanado && !juegoPerdido)
        {
            MostrarDerrota("El demonio te atrapó");
        }
    }

    // Dibuja la interfaz de usuario
    // Dibuja la interfaz de usuario
    void OnGUI()
    {
        if (Time.timeScale == 0f) return;

        // Si alguno de los Canvas de interacción está activo, no mostrar GUI
        if ((Indice != null && Indice.activeSelf) ||
            (TelefonoCanvas != null && TelefonoCanvas.activeSelf) ||
            (VentiladorCanvas != null && VentiladorCanvas.activeSelf) ||
            (TermometroCanvas != null && TermometroCanvas.activeSelf))
        {
            return;
        }

        GUIStyle estiloTexto = new GUIStyle(GUI.skin.label);
        estiloTexto.fontSize = 34;
        estiloTexto.normal.textColor = Color.white;
        estiloTexto.alignment = TextAnchor.UpperLeft;

        float anchoBarra = 520f;
        float altoBarra = 45f;
        float x = 35f;
        float y = 55f;

        // CONTADOR DE TAREAS
        int tareasCompletadas = ContarTareasCompletadas();
        int totalTareas = 12;

        GUIStyle estiloContador = new GUIStyle(GUI.skin.label);
        estiloContador.fontSize = 36;
        estiloContador.normal.textColor = Color.yellow;
        estiloContador.alignment = TextAnchor.UpperCenter;
        estiloContador.fontStyle = FontStyle.Bold;

        string textoContador = $"Tareas: {tareasCompletadas}/{totalTareas}";
        GUI.Label(new Rect(Screen.width / 2 - 150, 20, 300, 50), textoContador, estiloContador);

        // BARRA TIEMPO GENERAL
        GUI.color = Color.gray;
        GUI.DrawTexture(new Rect(x, y, anchoBarra, altoBarra), Texture2D.whiteTexture);

        float porcentajeGeneral = tiempoRestanteGeneral / tiempoGeneral;
        GUI.color = juegoGanado ? Color.green : Color.cyan;
        GUI.DrawTexture(new Rect(x, y, anchoBarra * porcentajeGeneral, altoBarra), Texture2D.whiteTexture);

        string textoGeneral = juegoGanado ? "¡VICTORIA!" : $"Tiempo total: {tiempoRestanteGeneral:F0}s";
        GUI.color = juegoGanado ? Color.green : Color.white;
        GUI.Label(new Rect(x, y - 45, anchoBarra, 45), textoGeneral, estiloTexto);

        // BARRA TIEMPO DEMONIO
        if (!demonioCalmado && !juegoGanado)
        {
            float tiempoMostrar = Mathf.Max(0f, tiempoRestanteDemonio);
            float porcentajeDemonio = tiempoMostrar / tiempoDemonio;

            GUI.color = Color.gray;
            GUI.DrawTexture(new Rect(x, y + 60, anchoBarra, 25), Texture2D.whiteTexture);

            if (tiempoMostrar <= 0f || modoMatarActivado)
                GUI.color = Color.red;
            else if (persecucionActivada)
                GUI.color = new Color(1f, 0.5f, 0f);
            else
                GUI.color = new Color(1f, 0.7f, 0.2f);

            GUI.DrawTexture(new Rect(x, y + 60, anchoBarra * porcentajeDemonio, 25), Texture2D.whiteTexture);

            GUIStyle estiloDemonio = new GUIStyle(GUI.skin.label);
            estiloDemonio.fontSize = 28;
            estiloDemonio.normal.textColor = Color.white;
            estiloDemonio.alignment = TextAnchor.UpperLeft;
            estiloDemonio.fontStyle = FontStyle.Bold;

            string textoDemonio = tiempoMostrar <= 0f ? "PELIGRO: Demonio desatado!" : $"Demonio: {tiempoMostrar:F0}s / 210s";
            GUI.Label(new Rect(x, y + 90, anchoBarra, 35), textoDemonio, estiloDemonio);
        }

        // ESTADO DEL DEMONIO
        string estadoDemonio = demonioCalmado ? "DEMONIO: CALMADO" :
                              (modoMatarActivado ? "DEMONIO: MODO MATAR" :
                              (persecucionActivada ? "DEMONIO: ENFADADO" : "DEMONIO: TRANQUILO"));
        GUI.Label(new Rect(x, y + 130, anchoBarra, 45), estadoDemonio, estiloTexto);

        if (juegoGanado)
            GUI.Label(new Rect(x, y + 180, anchoBarra, 45), "TODAS LAS TAREAS COMPLETADAS", estiloTexto);
    }



}