using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public Slider sensibilidadSlider;
    public Slider volumenSlider;

    public MouseLook mouseLookScript;

    void Start()
    {
        sensibilidadSlider.value = SettingsManager.Instance.sensibilidad;
        volumenSlider.value = SettingsManager.Instance.volumen;

        sensibilidadSlider.onValueChanged.AddListener(SetSensibilidad);
        volumenSlider.onValueChanged.AddListener(SetVolumen);
    }




    public void SetSensibilidad(float value)
    {
        SettingsManager.Instance.SetSensibilidad(value);

        if (mouseLookScript != null)
            mouseLookScript.ActualizarSensibilidad(value);
    }

    public void SetVolumen(float value)
    {
        SettingsManager.Instance.SetVolumen(value);
    }
}
