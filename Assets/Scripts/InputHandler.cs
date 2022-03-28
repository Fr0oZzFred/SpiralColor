using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Controls;
public class InputHandler : MonoBehaviour {

    public static ButtonControl gyroX = null;
    public static ButtonControl gyroY = null;
    public static ButtonControl gyroZ = null;

    // Acceleration
    //public static ButtonControl acclX = null;
    //public static ButtonControl acclY = null;
    //public static ButtonControl acclZ = null;

    bool connectedOnce;
    bool ControllerIsMissing => Controller == null;
    static bool isDS4 => Gamepad.current.device.name == "DualShock4GamepadHID";

    public  static Gamepad Controller = null;
    private static DualShock4GamepadHID DS4Controller = null;
    public string ErrorMessage {
        get {
            return errorMessage;
        }
    }
    private string errorMessage = "";
    public static InputHandler Instance { get; private set; }

    /// <summary>
    /// Search a frist time the controller, and add a switch on OnDeviceChange Unity Event
    /// </summary>
    private void Awake() {
        if (!Instance) Instance = this;
        SearchController();
        if (Controller != null) connectedOnce = true;
        InputSystem.onDeviceChange +=
        (device, change) => {
            if (GameManager.Instance.CurrentState == GameState.Boot) return;
            switch (change) {
                case InputDeviceChange.Added:
                    // New Device.
                    Controller = null;
                    DS4Controller = null;
                    connectedOnce = true;
                    break;

                case InputDeviceChange.Disconnected:
                    // Device got unplugged.
                    Controller = null;
                    DS4Controller = null;
                    break;

                case InputDeviceChange.Reconnected:
                    // Plugged back in.
                    SetController();
                    break;

                default:
                    // See InputDeviceChange reference for other event types.
                    break;
            }
        };
    }

    private void Update() {
        if (!GameManager.Instance) return;
        if (GameManager.Instance.CurrentState == GameState.Boot) return;
        if (ControllerIsMissing) SearchController();
        
    }

    void SearchController() {
        if (ControllerIsMissing) {
            try {
                SetController();
            } catch {
                if (connectedOnce) {
                    errorMessage = "Reconnect the Gamepad please !";
                } else {
                    errorMessage = "Plug a Gamepad please !";
                }
                if (GameManager.Instance)
                    GameManager.Instance.SetState(GameState.ControllerDisconnected);
            }
        }
    }

    /// <summary>
    /// if the Controller != null or if the gamepad != DS4 will return the Gamepad.current
    /// else it will read a custom layout controller for the Gyroscope and return it
    /// </summary>
    /// <param name="layoutFile"> use this param is you wanna use a other LayoutFile than Assets/Scripts/customLayout.json </param>
    private static void SetController(string layoutFile = null) {
        CheckForMultipleController();
        if (!isDS4) {
            Controller = Gamepad.current;
            GameManager.Instance.SetState(GameManager.Instance.OldState);
            return;
        }
        if (DS4Controller != null) return;

        // Read layout from JSON file
        string layout = File.ReadAllText(layoutFile == null ? Application.dataPath + "/StreamingAssets/customLayout.json" : layoutFile);

        // Overwrite the default layout
        InputSystem.RegisterLayoutOverride(layout, "DualShock4GamepadHID");

        var ds4 = (DualShock4GamepadHID)Gamepad.current;
        Controller = DS4Controller = ds4;
        BindControls(DS4Controller);

        GameManager.Instance.SetState(GameManager.Instance.OldState);
    }

    /// <summary>
    /// Make Current the last plugged DS4 or if they aren't DS4 plugged -> Make the last plugged Gamepad current
    /// </summary>
    static void CheckForMultipleController() {
        if (Gamepad.all.Count == 0) return;
        if (Gamepad.all.Count < 2) {
            Gamepad.all[0].MakeCurrent();
            return;
        }
        foreach(Gamepad g in Gamepad.all) {
            if(g.device.name == "DualShock4GamepadHID") {
                g.MakeCurrent();
                return;
            }
        }
        Gamepad.all[0].MakeCurrent();
    }

    #region DS4 Controls
    private static void BindControls(Gamepad ds4) {
        gyroX = ds4.GetChildControl<ButtonControl>("gyro X 14");
        gyroY = ds4.GetChildControl<ButtonControl>("gyro Y 16");
        gyroZ = ds4.GetChildControl<ButtonControl>("gyro Z 18");

        //acclX = ds4.GetChildControl<ButtonControl>("accl X 20");
        //acclY = ds4.GetChildControl<ButtonControl>("accl Y 22");
        //acclZ = ds4.GetChildControl<ButtonControl>("accl Z 24");
    }

    /*public static void ReadAcceleration() {
        Debug.Log("X" + acclX.ReadValue());
        Debug.Log("Y" + acclY.ReadValue());
        Debug.Log("Z" + acclZ.ReadValue());
    }*/

    private static float ProcessRawData(float data) {
        return data > 0.5 ? 1 - data : -data;
    }
    public static Quaternion GetGyroRotation(float scale = 1) {
        if (DS4Controller == null) return Quaternion.identity;
        float x = ProcessRawData(gyroX.ReadValue()) * scale;
        float y = ProcessRawData(gyroY.ReadValue()) * scale;
        float z = -ProcessRawData(gyroZ.ReadValue()) * scale;
        return Quaternion.Euler(x, y, z);
    }

    public static void SetControllerLED(Color color) {
        if (DS4Controller != null) DS4Controller.SetLightBarColor(color);
    }
    #endregion

    #region Gamepad Controls
    public static Vector2 GetLeftStickValues() {
        if (Controller == null) return Vector2.zero;
        return new Vector2(
            Controller.leftStick.x.ReadValue(),
            Controller.leftStick.y.ReadValue()
            );
    }

    public static Vector2 GetRightStickValues() {
        if (Controller == null) return Vector2.zero;
        return new Vector2(
            Controller.rightStick.x.ReadValue(),
            Controller.rightStick.y.ReadValue()
            );
    }

    public static float UpDownTrigger() {
        if (Controller == null) return 0f;
        return Controller.rightTrigger.ReadValue() - Controller.leftTrigger.ReadValue();
    }
    #endregion
}
