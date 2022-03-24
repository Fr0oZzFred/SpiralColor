using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayTestData : MonoBehaviour{
    public InputField username;
    float time = 0;
    int death = 0;
    [SerializeField] string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfUfDhbKpNkaf8i2UbzaDdYQBdtk-jvZwhkmZbUr71nq_L1tQ/formResponse";
    void Update() {
        time += Time.deltaTime;
    }
    public void Play() {
        time = 0;
        death = 0;
    }
    public void Dead() {
        death++;
    }
    IEnumerator Post(string name, string death, string time) {
        WWWForm form = new WWWForm();
        form.AddField("entry.938686525", name);
        form.AddField("entry.751726181", death);
        form.AddField("entry.964582346", time);
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
        Debug.Log(username.text + death.ToString() + Math.Round((decimal)time, 2).ToString());
        StartCoroutine(Post(username.text, death.ToString(), Math.Round((decimal)time, 2).ToString()));
    }
}
