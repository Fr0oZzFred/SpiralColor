using System;
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
    [SerializeField] AudioMixerSnapshot menu;
    [SerializeField] AudioMixerSnapshot enJeu;
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

    // Start is called before the first frame update
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }
    }
}
