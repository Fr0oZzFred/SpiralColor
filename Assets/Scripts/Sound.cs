using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public AudioMixerGroup outputGroup;
    [Range(.1f, 3f)]
    public float pitch = 1f;
    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
