using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayTestData : MonoBehaviour{
    public static PlayTestData Instance { get; private set; }
    [SerializeField] InputField username;
    [SerializeField] string fieldName, fieldLevel, fieldTime, fieldDeath;
    float time = 0;
    int death = 0;
    [SerializeField] string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfUfDhbKpNkaf8i2UbzaDdYQBdtk-jvZwhkmZbUr71nq_L1tQ/formResponse";
    void Awake() {
        Instance = this;
    }
    void Update() {
        time += Time.deltaTime;
    }
    public void Restart() {
        time = 0;
        death = 0;
    }
    public void Dead() {
        death++;
    }
    IEnumerator Post(string name, string level, string time, string death) {
        WWWForm form = new WWWForm();
        form.AddField(fieldName, name);
        form.AddField(fieldLevel, level);
        form.AddField(fieldTime, time);
        form.AddField(fieldDeath, death);
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form)) {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log(www.error);
            } else {
                Debug.Log("Form upload complete!");
            }
        }
    }
    public void Send() {
        StartCoroutine(Post(username.text, LevelManager.Instance.LevelInt.ToString(), Math.Round((decimal)time, 2).ToString(), death.ToString()));
    }
}
