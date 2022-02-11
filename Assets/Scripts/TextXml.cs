using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Text))]
public class TextXml : MonoBehaviour{
    Text text;
    public string name;
    void Start(){
        text = GetComponent<Text>();
    }
    public void UpdateText() {
        text.text = ChangeLanguage.Instance.Replace(name);
    }
}