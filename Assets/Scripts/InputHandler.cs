using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Controls;
public class InputHandler : MonoBehaviour
{
    #region variables
    bool connectedOnce = false;
    public static DualShock4GamepadHID ds4Controller;
    #endregion

    private void Start() {
        SearchController();
        
        //if (!connectedOnce) Message au GameManager pour dire que le joueur doit connecté sa dualshock 

    }

    void SearchController() {
        try {
            ds4Controller = DS4.GetController();
            connectedOnce = true;
        } catch {
            try {
                Gamepad gamepad = Gamepad.current;
                connectedOnce = true;
            } catch {
                Debug.LogWarning(Gamepad.current.name + "is supported but not optimal");
            }
        }
    }

    private void Update() {
        InputSystem.onDeviceChange +=
        (device, change) => {
            switch (change) {
                case InputDeviceChange.Added:
                    SearchController();
                    Debug.Log(change);
                    // New Device.
                    break;
                case InputDeviceChange.Disconnected:
                    SearchController();
                    Debug.Log(change);
                    // Device got unplugged.
                    break;
                case InputDeviceChange.Reconnected:
                    SearchController();
                    Debug.Log(change);
                    // Plugged back in.
                    break;
                default:
                    Debug.Log(change);
                    // See InputDeviceChange reference for other event types.
                    break;
            }
        };
    }
    
    public static Gamepad GetCurrentGamepad() {
        if (Gamepad.current == null) throw new ArgumentNullException();
        return Gamepad.current;
    }
    public static void SetControllerColor(Color color) {
        if (ds4Controller != null) ds4Controller.SetLightBarColor(color);
    }

    public static Quaternion GetGyroRotation() {
        if(ds4Controller != null)  return DS4.GetRotation();
        return new Quaternion();
    }

    public static Vector2 GetLeftStickValues() {
        if (Gamepad.current == null) throw new ArgumentNullException("Current Gamepad is null");
        return new Vector2(
            Gamepad.current.leftStick.x.ReadValue(),
            Gamepad.current.leftStick.y.ReadValue()
            );
    }

    public static Vector2 GetRightStickValues() {
        if (Gamepad.current == null) throw new ArgumentNullException("Current Gamepad is null");
        return new Vector2(
            Gamepad.current.rightStick.x.ReadValue(),
            Gamepad.current.rightStick.y.ReadValue()
            );
    }
}

public class DS4 {

    public static ButtonControl gyroX = null;
    public static ButtonControl gyroY = null;
    public static ButtonControl gyroZ = null;

    // Acceleration
    //public static ButtonControl acclX = null;
    //public static ButtonControl acclY = null;
    //public static ButtonControl acclZ = null;

    public static DualShock4GamepadHID controller = null;
    public static DualShock4GamepadHID GetController(string layoutFile = null) {
        string layout = File.ReadAllText(layoutFile == null ? "Assets/Scripts/customLayout.json" : layoutFile);
        InputSystem.RegisterLayoutOverride(layout, "DualShock4GamepadHID");
        var ds4 = (DualShock4GamepadHID)Gamepad.current;
        DS4.controller = ds4;
        BindControls(DS4.controller);
        return ds4;
    }

    private static void BindControls(Gamepad ds4) {
        gyroX = ds4.GetChildControl<ButtonControl>("gyro X 14");
        gyroY = ds4.GetChildControl<ButtonControl>("gyro Y 16");
        gyroZ = ds4.GetChildControl<ButtonControl>("gyro Z 18");

        //acclX = ds4.GetChildControl<ButtonControl>("accl X 20");
        //acclY = ds4.GetChildControl<ButtonControl>("accl Y 22");
        //acclZ = ds4.GetChildControl<ButtonControl>("accl Z 24");
    }

    public static Quaternion GetRotation(float scale = 1) {
        float x = ProcessRawData(gyroX.ReadValue()) * scale;
        float y = ProcessRawData(gyroY.ReadValue()) * scale;
        float z = -ProcessRawData(gyroZ.ReadValue()) * scale;
        return Quaternion.Euler(x, y, z);
    }
    /*public static void ReadAcceleration() {
        Debug.Log("X" + acclX.ReadValue());
        Debug.Log("Y" + acclY.ReadValue());
        Debug.Log("Z" + acclZ.ReadValue());
    }*/
    private static float ProcessRawData(float data) {
        return data > 0.5 ? 1 - data : -data;
    }

}