using UnityEditor;
public class DeveloperMenu {
    [MenuItem("Developer/Clear Saves")]
    public static void ClearSaves() {
        throw new System.NotImplementedException();
    }
    [MenuItem("Developer/CheatCodes/InvokeChiaki")]
    public static void Chiaki() {
        throw new System.NotImplementedException("とてもきれいだ");
    }
    [MenuItem("Developer/GameManager/GMProgressionToMax")]
    public static void GMProgressionToMax() {
        GameManager.Instance.UpdateProgression(15);
    }
    [MenuItem("Developer/GameManager/Save")]
    public static void SaveGM() {
        GameManager.Instance.SaveGameManager();
        SoundsManager.Instance.SaveSettings();
    }
    [MenuItem("Developer/GameManager/Load")]
    public static void LoadGM() {
        GameManager.Instance.LoadGameManager();
        SoundsManager.Instance.LoadSettings();
    }
    [MenuItem("Developer/Keyboard")]
    public static void OpenKeyboard() {
        UIManager.Instance.OpenKeyboard();
    }
}
