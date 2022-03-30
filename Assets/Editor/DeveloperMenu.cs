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
    [MenuItem("Developer/GameManager/Save")]
    public static void SaveGM() {
        GameManager.Instance.SaveGameManager();
    }
    [MenuItem("Developer/GameManager/Load")]
    public static void LoadGM() {
        GameManager.Instance.LoadGameManager();
    }
    [MenuItem("Developer/Keyboard")]
    public static void onKeyboard() {
        UIManager.Instance.OpenKeyboard();
    }
}
