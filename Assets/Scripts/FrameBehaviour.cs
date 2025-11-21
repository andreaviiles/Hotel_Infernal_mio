using UnityEngine;

public class FrameBehavior : MonoBehaviour
{
    private bool recto = false;
    private bool enderezando = false;

    [Header("Rotación recta del cuadro")]
    public Vector3 rotacionRecta = new Vector3(0f, 0f, -180f); // Ajusta según tu escena

    [Header("Velocidad de enderezado")]
    public float velocidadRotacion = 120f; // grados por segundo

    private Quaternion rotacionObjetivo;

    public bool EstaRecto => recto;

    public void Enderezar()
    {
        if (!recto && !enderezando)
        {
            rotacionObjetivo = Quaternion.Euler(rotacionRecta);
            enderezando = true;
        }
    }

    void Update()
    {
        if (enderezando)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                rotacionObjetivo,
                velocidadRotacion * Time.deltaTime
            );

            // Cuando llega a la rotación objetivo
            if (Quaternion.Angle(transform.rotation, rotacionObjetivo) < 0.1f)
            {
                transform.rotation = rotacionObjetivo;
                recto = true;
                enderezando = false;
                Debug.Log($"Cuadro {gameObject.name} enderezado.");
            }
        }
    }
}
