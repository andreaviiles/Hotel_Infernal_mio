using UnityEngine;

public class FaucetBehavior : MonoBehaviour
{
    private bool cerrado = false;
    public Renderer rend;

    [Header("Objeto visual del agua")]
    public GameObject aguaVisual; // ← El objeto que representa el agua

    void Start()
    {
        if (aguaVisual != null)
            aguaVisual.SetActive(true); // Empieza con agua activa
    }

    public bool EstaCerrado => cerrado;

    public void Cerrar()
    {
        if (!cerrado)
        {
            cerrado = true;

            if (rend != null)
                rend.material.color = Color.gray; // Color al cerrar

            if (aguaVisual != null)
                aguaVisual.SetActive(false); // Oculta el agua

            Debug.Log($"Grifo {gameObject.name} cerrado.");
        }
    }

    public void Abrir()
    {
        if (cerrado)
        {
            cerrado = false;

            if (rend != null)
                rend.material.color = Color.blue; // Color al abrir

            if (aguaVisual != null)
                aguaVisual.SetActive(true); // Muestra el agua

            Debug.Log($"Grifo {gameObject.name} abierto.");
        }
    }
}
