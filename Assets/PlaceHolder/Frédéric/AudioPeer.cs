using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour{
    AudioSource audioSource;
    public static float[] samples = new float[512];
    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        GetSpectrumAudioSource();
    }
    void GetSpectrumAudioSource() {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }
} 
