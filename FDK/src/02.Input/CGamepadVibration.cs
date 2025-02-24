using SharpDX.XInput;

namespace FDK;

public class CGamepadVibration : IDisposable {
    private Controller controller;
    private bool isConnected;
    private Vibration vibrationState;

    public CGamepadVibration(UserIndex userIndex = UserIndex.One) {
        controller = new Controller(userIndex);
        isConnected = controller.IsConnected;
        vibrationState = new Vibration();
    }

    public bool IsConnected() {
        isConnected = controller.IsConnected;  // Update connection state
        return isConnected;
    }

    /// <summary>
    /// Sets vibration strength for both motors
    /// </summary>
    /// <param name="leftMotor">Left (heavy) motor strength (0-1)</param>
    /// <param name="rightMotor">Right (light) motor strength (0-1)</param>
    public void SetVibration(float leftMotor, float rightMotor) {
        if (!IsConnected()) return;

        vibrationState.LeftMotorSpeed = (ushort)(Math.Clamp(leftMotor, 0f, 2f) * 65535f);
        vibrationState.RightMotorSpeed = (ushort)(Math.Clamp(rightMotor, 0f, 2f) * 65535f);
        
        controller.SetVibration(vibrationState);
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
        
        vibrationState.LeftMotorSpeed = 0;
        vibrationState.RightMotorSpeed = 0;
        controller.SetVibration(vibrationState);
    }

    public void Dispose() {
        StopVibration();
    }
}
