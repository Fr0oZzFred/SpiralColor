using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Xml;
public class ChangeLanguage : MonoBehaviour {
    public static ChangeLanguage Instance { get; private set; }
    public TextAsset dictionary;
    public string languageName;
    public int currentLanguage;
    string waifu;
    public Text waifuText;
    public Dropdown selectLanguage;
    List<Dictionary<string, string>> languages = new List<Dictionary<string, string>>();
    Dictionary<string, string> temp;
    void Awake() {
        Instance = this;
        Reader();
        PatchLanguage();
        DontDestroyOnLoad(gameObject);
    }
    public void PatchLanguage() {
        currentLanguage = selectLanguage.value;
        languages[currentLanguage].TryGetValue("Name", out languageName);
        languages[currentLanguage].TryGetValue("Waifu", out waifu);
        waifuText.text = waifu;
    }
    void Reader() {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(dictionary.text);
        XmlNodeList languageList = doc.GetElementsByTagName("language");
        foreach(XmlNode languageValue in languageList) {
            XmlNodeList content = languageValue.ChildNodes;
            temp = new Dictionary<string, string>();
            foreach(XmlNode value in content) {
                if (value.Name == "Name") temp.Add(value.Name, value.InnerText);
                if (value.Name == "Waifu") temp.Add(value.Name, value.InnerText);
            }
            languages.Add(temp);
        }
    }
    public string Replace(string name) {
        string text;
        languages[currentLanguage].TryGetValue(name, out text);
        if (text == null) throw new System.NullReferenceException("Nom incompatible : " + name);
        return text;
    }
}
