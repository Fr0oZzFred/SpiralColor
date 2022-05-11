using UnityEngine;
using UnityEngine.UI;

public class VolumeGroup : MonoBehaviour {
    [SerializeField] string volumeParameter = "MasterVolume";
    [SerializeField] Slider slider;
    [SerializeField] float multiple = 30f;
    [SerializeField] Toggle toggle;
    float muteStock;
    bool disableToggleEvent;
    public float SliderValue {
        get {
            return slider.value;
        }
    }
    void Awake() {
        muteStock = slider.value;
        slider.onValueChanged.AddListener(ChangedValue);
        toggle.onValueChanged.AddListener(ToggleChangedValue);
    }
    void ToggleChangedValue(bool enableSound) {
        if (disableToggleEvent)
            return;

        if (enableSound)
            slider.value = muteStock;
        else {
            muteStock = slider.value;
            slider.value = slider.minValue;
        }
    }

    void ChangedValue(float value) {
        SoundsManager.Instance.Mixer.SetFloat(volumeParameter, Mathf.Log10(value) * multiple);
        disableToggleEvent = true;
        toggle.isOn = slider.value > slider.minValue;
        disableToggleEvent = false;
    }
    public void Load(float value) {
        slider.value = value;
        muteStock = value;
    }
}