using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ColorController : ISerializationCallbackReceiver {
    public Color color;
    [ListToPopup(typeof(ColorController), "TMPList")]
    public string button;

    public static List<string> TMPList;
    [HideInInspector] public List<string> PopupList = new List<string> 
    { "triangleButton", "circleButton", "crossButton", "squareButton"};

    public void OnBeforeSerialize() {
        TMPList = PopupList;
    }

    public void OnAfterDeserialize() { }
}