using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeGroup : MonoBehaviour {
    [SerializeField] string volumeParameter = "MasterVolume";
    [SerializeField] Slider slider;
    [SerializeField] float multiple = 30f;
    [SerializeField] private Toggle toggle;
    private bool disableToggleEvent;

    private void Awake() {
        slider.onValueChanged.AddListener(ChangedValue);
        toggle.onValueChanged.AddListener(ToggleChangedValue);
    }

    void Start() {
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
    }

    private void ToggleChangedValue(bool enableSound) {
        if (disableToggleEvent)
            return;

        if (enableSound)
            slider.value = slider.maxValue;
        else
            slider.value = slider.minValue;
    }

    private void ChangedValue(float value) {
        SoundsManager.Instance.Mixer.SetFloat(volumeParameter, Mathf.Log10(value) * multiple);
        disableToggleEvent = true;
        toggle.isOn = slider.value > slider.minValue;
        disableToggleEvent = false;
    }

    private void OnDisable() {
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }

}