using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;

public class DS4  {

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
