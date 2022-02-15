using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Xml;
public enum Balise { Name, Waifu, Game}
[RequireComponent(typeof(Text))]
public class TextXml : MonoBehaviour {
    public Balise balise;
    Text text;
    void Start() {
        ChangeLanguage.Instance.listText.Add(this);
        text = GetComponent<Text>();
        UpdateText();
    }
    public void UpdateText() {
        text.text = ChangeLanguage.Instance.Replace(balise.ToString());
    }
}