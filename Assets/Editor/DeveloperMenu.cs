using UnityEditor;
public class DeveloperMenu {
    [MenuItem("Developer/Clear Saves")]
    public static void ClearSaves() {
        throw new System.NotImplementedException();
    }
    [MenuItem("Developer/CheatCodes/InvokeChiaki")]
    public static void Chiaki() {
        throw new System.NotImplementedException("Trop beau pour être vrai");
    }
    [MenuItem("Developer/GameManager/GMProgressionToMax")]
    public static void GMProgressionToMax() {
        GameManager.Instance.UpdateProgression(4);
    }
}
