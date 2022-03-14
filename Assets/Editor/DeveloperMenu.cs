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

    [MenuItem("Developer/GameManager/SetGSToMainMenu")]
    public static void SetGSToMainMenu() {
        GameManager.Instance.SetState(GameState.MainMenu);
    }
    [MenuItem("Developer/GameManager/SetGSToInGame")]
    public static void SetGSToInGame() {
        GameManager.Instance.SetState(GameState.InGame);
    }
    [MenuItem("Developer/GameManager/SetGSToPause")]
    public static void SetGSToPause() {
        GameManager.Instance.SetState(GameState.Pause);
    }
    [MenuItem("Developer/GameManager/SetGSToScore")]
    public static void SetGSToScore() {
        GameManager.Instance.SetState(GameState.Score);
    }
    [MenuItem("Developer/GameManager/SetGSToCredits")]
    public static void SetGSToCredits() {
        GameManager.Instance.SetState(GameState.Credits);
    }
}
