using UnityEngine;

public class CleanerBehavior : MonoBehaviour
{
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = Color.green;
    }

    void OnEnable()
    {
        if (rend != null)
            rend.material.color = Color.green;
    }

    void OnDisable()
    {
        Debug.Log($"{gameObject.name} ha sido limpiado.");
    }
}

