using UnityEngine;

public class SettingsBootstrap : MonoBehaviour
{
    void Awake()
    {
        if (SettingsManager.Instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Managers/SettingsManager");
            if (prefab != null)
            {
                Instantiate(prefab);
                Debug.Log("SettingsManager instanciado");
            }
            else
            {
                Debug.LogError("Prefab SettingsManager no encontrado");
            }
        }
    }
}


