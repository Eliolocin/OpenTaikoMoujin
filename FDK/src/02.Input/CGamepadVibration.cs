using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace FDK;

public class CGamepadVibration : IDisposable {
    private SerialPort serialPort;

    public CGamepadVibration(string portName = "COM3") {
        serialPort = new SerialPort(portName, 9600);
        serialPort.Open();
    }

    public bool IsConnected() {
        return serialPort != null && serialPort.IsOpen;
    }

    public void SendCommand(string command)
        {
            if (IsConnected())
            {
                // Append a newline so that the Arduino can use ReadLine()
                serialPort.WriteLine(command);
            }
        }

    /// <summary>
    /// Sets vibration strength for both motors
    /// </summary>
    /// <param name="leftMotor">Left (heavy) motor strength (0-1)</param>
    /// <param name="rightMotor">Right (light) motor strength (0-1)</param>
    public void SetVibration(float leftMotor, float rightMotor) {
        if (!IsConnected()) return;

        int leftPWM = (int)(Math.Clamp(leftMotor, 0f, 1f) * 255);
        int rightPWM = (int)(Math.Clamp(rightMotor, 0f, 1f) * 255);
        string command = $"SET,{leftPWM},{rightPWM}";
        SendCommand(command);
    }

    /// <summary>
    /// Creates a short "tap" vibration effect
    /// </summary>
    /// <param name="strength">Vibration strength (0-1)</param>
    /// <param name="duration">Duration in milliseconds</param>
    public async Task TapAsync(float strength, int duration) {
        if (!IsConnected()) return;

        SetVibration(strength, strength);
        await Task.Delay(duration);
        StopVibration();
    }

    public void StopVibration() {
        if (!IsConnected()) return;
        
         SendCommand("STOP");
    }

    public void Dispose() {
        StopVibration();
        if (serialPort != null && serialPort.IsOpen)
                serialPort.Close();

    }
}
