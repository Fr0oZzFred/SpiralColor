using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundsManager : MonoBehaviour {

    [SerializeField] AudioMixer mixer;
    [SerializeField] List<AudioMixerSnapshot> snapshots;

    //Changer seulement les snapshot quand les settings n'ont pas été modifié et l'ajouter correctement au GS


    public AudioMixer Mixer {
        get {
            return mixer;
        }
    }

    public Sound[] sounds;
    public static SoundsManager Instance { get; private set; }
    
    void Awake()
    {
        if(!Instance)
        {
            Instance = this;
        }
        Init();
    }


    void Start() {
        //GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }
    private void Init() {
        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.outputAudioMixerGroup = s.outputGroup;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void OnGameStateChanged(GameState newState) {
        throw new NotImplementedException();
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void ChangeSnapshot(int index) {
        if (index >= 0 && index < snapshots.Count) {
            snapshots[index].TransitionTo(1f);
        }
    }

    public void ChangeSnapshot(string name) {
        for (int i = 0; i < snapshots.Count; i++) {
            if (snapshots[i].name == name) {
                ChangeSnapshot(i);
                return;
            }
        }
        Debug.Log("Snapshot :" + name + " not found!");
    }
}
