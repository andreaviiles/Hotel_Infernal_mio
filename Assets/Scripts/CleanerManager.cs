using UnityEngine;
using System.Collections.Generic;

public class CleanerManager : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject[] cubosLimpiables;
    public PlayerMovement playerMovement; // ← NUEVO

    private Dictionary<GameObject, int> interacciones = new Dictionary<GameObject, int>();
    private GameObject cuboActual;
    private bool cerca = false;
    private bool limpiezaCompletada = false;

    void Start()
    {
        foreach (GameObject cubo in cubosLimpiables)
        {
            cubo.SetActive(true);
            interacciones[cubo] = 0;

            if (!cubo.TryGetComponent(out CleanerBehavior behavior))
                cubo.AddComponent<CleanerBehavior>();
        }
    }

    void Update()
    {
        if (limpiezaCompletada) return;

        // 🚫 Bloquear si lleva la toalla
        if (playerMovement != null && playerMovement.EstaLlevandoObjeto)
        {
            cerca = false;
            cuboActual = null;
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f))
        {
            if (hit.collider.CompareTag("Cleanable"))
            {
                cuboActual = hit.collider.gameObject;
                cerca = true;
            }
            else
            {
                cerca = false;
            }
        }
        else
        {
            cerca = false;
        }

        if (cerca && Input.GetKeyDown(KeyCode.E) && cuboActual != null)
        {
            if (interacciones.ContainsKey(cuboActual))
            {
                interacciones[cuboActual]++;
                Debug.Log($"Cubo {cuboActual.name}: {interacciones[cuboActual]} interacciones.");

                if (interacciones[cuboActual] >= 3)
                {
                    cuboActual.SetActive(false);
                    interacciones[cuboActual] = 0;
                }

                if (TodosCubosDesactivados())
                {
                    limpiezaCompletada = true;
                    Debug.Log("✅ ¡Tarea de limpieza completada!");
                }
            }
        }
    }

    bool TodosCubosDesactivados()
    {
        foreach (GameObject cubo in cubosLimpiables)
        {
            if (cubo.activeSelf)
                return false;
        }
        return true;
    }

    public bool LimpiezaCompletada => limpiezaCompletada;
}



