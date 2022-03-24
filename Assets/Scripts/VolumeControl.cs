using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] string volumeParameter = "MasterVolume";
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider slider;
    [SerializeField] float multiple = 30f;
    [SerializeField] private Toggle toggle;
    [SerializeField] List<AudioMixerSnapshot> snapshots;
    private bool disableToggleEvent;

    private void Awake()
    {
        slider.onValueChanged.AddListener(ChangedValue);
        toggle.onValueChanged.AddListener(ToggleChangedValue);
    }

    private void ToggleChangedValue(bool enableSound)
    {
        if (disableToggleEvent)
            return;
        
        if (enableSound)
             slider.value = slider.maxValue;
        else
             slider.value = slider.minValue;
    }

    private void ChangedValue(float value)
    {
        mixer.SetFloat(volumeParameter, Mathf.Log10(value) * multiple);
        disableToggleEvent = true;
        toggle.isOn = slider.value > slider.minValue;
        disableToggleEvent = false;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
    }

    public void ChangeSnapshots(int index) {
        if(index >= 0 && index < snapshots.Count) {
            snapshots[index].TransitionTo(1f);
            Debug.Log(snapshots[index].name);
        }
    }
}
