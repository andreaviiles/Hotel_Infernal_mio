using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public float sensibilidad = 120f;
    public float volumen = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sensibilidad = PlayerPrefs.GetFloat("Sensibilidad", 120f);
            volumen = PlayerPrefs.GetFloat("Volumen", 1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSensibilidad(float value)
    {
        sensibilidad = value;
        PlayerPrefs.SetFloat("Sensibilidad", value);
        PlayerPrefs.Save();
    }

    public void SetVolumen(float value)
    {
        volumen = value;
        PlayerPrefs.SetFloat("Volumen", value);
        PlayerPrefs.Save();
    }
}

