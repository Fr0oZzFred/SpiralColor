using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
[System.Serializable]
public class SoundsSettingsData {
    public float master, music, SFX;
    public Dictionary<string, bool> pieces;
    public SoundsSettingsData(SoundsManager data) {
        master = data.volumeGroup[0].SliderValue;
        music = data.volumeGroup[1].SliderValue;
        SFX = data.volumeGroup[2].SliderValue;
    }
}
public class SoundsManager : MonoBehaviour {

    [SerializeField] AudioMixer mixer;
    [SerializeField] List<AudioMixerSnapshot> snapshots;

    public List<VolumeGroup> volumeGroup = new List<VolumeGroup>();

    public AudioMixer Mixer {
        get {
            return mixer;
        }
    }

    public Sound[] sounds;
    Sound current;
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
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
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
    public void SaveSettings() {
        SaveSystem.SaveSoundsSettings(this);
    }
    public void LoadSettings() {
        SoundsSettingsData data = SaveSystem.LoadSoundsSettings();
        volumeGroup[0].Load(data.master);
        volumeGroup[1].Load(data.music);
        volumeGroup[2].Load(data.SFX);
    }
    private void OnGameStateChanged(GameState newState) {
        if (GameManager.Instance.OldState == GameState.Options) SaveSettings();
        if (!CustomSettings()) {
            switch (newState) {
                case GameState.MainMenu:
                    StopCurrentMusic();
                    Play("MainMenuTheme");
                    //ChangeSnapshot(0, 1f);
                    break;
                case GameState.InLevel:
                    //ChangeSnapshot(1, 1f);
                    break;
                case GameState.Options:
                    if (System.IO.File.Exists(Application.persistentDataPath + "/SoundsSettings.data")) {
                        LoadSettings();
                    }
                    break;
                case GameState.Credits:
                    StopCurrentMusic();
                    Play("CreditsTheme");
                    break;
            }
        }
    }

    bool CustomSettings() {
        for (int i = 0; i < volumeGroup.Count; i++) {
            if (volumeGroup[i].SliderValue != 1) return true;
        }
        return false;
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        if (s.outputGroup.name == "Music") current = s;
        s.source.Play();
    }
    public void StopCurrentMusic() {
        if (current != null) {
            current.source.Stop();
            current = null;
        }
    }
    public void PauseCurrentMusic() {
        if (current != null) {
            current.source.Pause();
        }
    }
    public void UnPauseCurrentMusic() {
        if (current != null) {
            current.source.UnPause();
        }
    }
    public void ChangeSnapshot(int index, float time) {
        if (index >= 0 && index < snapshots.Count) {
            snapshots[index].TransitionTo(time);
        }
    }

    public void ChangeSnapshot(string name, float time) {
        for (int i = 0; i < snapshots.Count; i++) {
            if (snapshots[i].name == name) {
                ChangeSnapshot(i, time);
                return;
            }
        }
        Debug.Log("Snapshot :" + name + " not found!");
    }
    public void PlayClickSound() {
        Play("ButtonClick");
    }

    public void PlayButtonStartSound() {
        Play("ButtonStart");
    }
}
