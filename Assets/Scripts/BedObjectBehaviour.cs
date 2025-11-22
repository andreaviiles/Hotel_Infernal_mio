using UnityEngine;

public class BedObjectBehavior : MonoBehaviour
{
    private int fase = 0; // 0 = sin interactuar, 1 = rotado, 2 = movido

    [Header("Transformaciones personalizadas")]
    public Vector3 rotacionFinal;   // Rotación específica para este objeto
    public Vector3 posicionFinal;   // Desplazamiento específico para este objeto

    public bool EstaCompletado => fase >= 2;

    public void Interactuar()
    {
        if (fase == 0)
        {
            // Primera interacción: rotar
            transform.rotation = Quaternion.Euler(rotacionFinal);
            fase = 1;
            Debug.Log($"{gameObject.name} rotado (fase 1).");
        }
        else if (fase == 1)
        {
            // Segunda interacción: mover
            transform.position = posicionFinal;
            fase = 2;
            Debug.Log($"{gameObject.name} movido (fase 2).");
        }
    }
}
