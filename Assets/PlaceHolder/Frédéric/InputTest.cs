using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Controls;
public class InputTest : MonoBehaviour {

    bool connectedOnce;
    bool ControllerIsMissing => Controller == null;

    static bool isDS4 => Gamepad.current.device.name == "DualShock4GamepadHID";
    public static ButtonControl gyroX = null;
    public static ButtonControl gyroY = null;
    public static ButtonControl gyroZ = null;

    // Acceleration
    //public static ButtonControl acclX = null;
    //public static ButtonControl acclY = null;
    //public static ButtonControl acclZ = null;

    public  static Gamepad Controller = null;
    private static DualShock4GamepadHID DS4Controller = null;


    private static void SetController(string layoutFile = null) {
        CheckForMultipleController();
        if (!isDS4) {
            Controller = Gamepad.current;
            return;
        }
        if (DS4Controller != null) return;

        // Read layout from JSON file
        string layout = File.ReadAllText(layoutFile == null ? "Assets/Scripts/customLayout.json" : layoutFile);

        // Overwrite the default layout
        InputSystem.RegisterLayoutOverride(layout, "DualShock4GamepadHID");

        var ds4 = (DualShock4GamepadHID)Gamepad.current;
        Controller = DS4Controller = ds4;
        BindControls(DS4Controller);
    }

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
    }

    private void Awake() {
        SearchController();
        if (Controller != null) connectedOnce = true;
        InputSystem.onDeviceChange +=
        (device, change) => {
            switch (change) {

                case InputDeviceChange.Added:
                    // New Device.
                    Controller = null;
                    DS4Controller = null;
                    connectedOnce = true;
                    Debug.Log(change);
                    break;

                case InputDeviceChange.Disconnected:
                    // Device got unplugged.
                    Controller = null;
                    DS4Controller = null;
                    Debug.Log(Gamepad.current);
                    Debug.Log(change);
                    break;

                case InputDeviceChange.Reconnected:
                    // Plugged back in.
                    SetController();
                    Debug.Log(change);
                    break;

                default:
                    // See InputDeviceChange reference for other event types.
                    break;
            }
        };
    }
    private void Update() {
        if (ControllerIsMissing) SearchController();
        SetControllerLED(Color.Lerp(Color.green, Color.red, Mathf.Sin(Time.time) + 1) / 2);
    }
    void SearchController() {
        if (ControllerIsMissing) {
            try {
                SetController();
            } catch {
                if (connectedOnce) {
                    Debug.Log("Reconnect the Gamepad pls");
                } else {
                    Debug.Log("Plug a Gamepad pls");
                }
            }
        }
    }
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
    public static Quaternion GetGyroRotation(float scale = 1) {
        if (DS4Controller == null) return Quaternion.identity;
        float x = ProcessRawData(gyroX.ReadValue()) * scale;
        float y = ProcessRawData(gyroY.ReadValue()) * scale;
        float z = -ProcessRawData(gyroZ.ReadValue()) * scale;
        return Quaternion.Euler(x, y, z);
    }
    private static float ProcessRawData(float data) {
        return data > 0.5 ? 1 - data : -data;
    }


    public static void SetControllerLED(Color color) {
        if (DS4Controller != null) DS4Controller.SetLightBarColor(color);
    }
    
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

    public static float UpDown() {
        if (Controller == null) return 0f;
        return Controller.rightTrigger.ReadValue() - Controller.leftTrigger.ReadValue();
    }

}
