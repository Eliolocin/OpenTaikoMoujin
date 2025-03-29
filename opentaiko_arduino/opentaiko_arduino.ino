const int leftMotorPin = 6; //change this
const int rightMotorPin = 9; //change this

bool rollActive = false;
unsigned long tapEndTime = 0;

void setup() {
  Serial.begin(9600);
  pinMode(leftMotorPin, OUTPUT);
  pinMode(rightMotorPin, OUTPUT);
  stopVibration();  // Ensure motors are off initially
}

void loop() {
  // Check for incoming serial commands.
  if (Serial.available() > 0) {
    String command = Serial.readStringUntil('\n');
    command.trim(); // Remove any extra whitespace/newline.
    processCommand(command);
  }
  
  // If in TAP mode, check if duration has expired.
  if (!rollActive && tapEndTime > 0 && millis() >= tapEndTime) {
    stopVibration();
    tapEndTime = 0;
  }
  
  // In roll mode, motors remain on continuously.
}

void processCommand(String cmd) {
  // Expected command formats:
  // SET,<leftPWM>,<rightPWM>
  // TAP,<leftPWM>,<rightPWM>,<duration>
  // STARTROLL,<PWM>
  // STOP

  int firstComma = cmd.indexOf(',');
  String action = (firstComma == -1) ? cmd : cmd.substring(0, firstComma);

  if (action == "SET") {
    // Format: SET,<leftPWM>,<rightPWM>
    int secondComma = cmd.indexOf(',', firstComma + 1);
    if (secondComma > 0) {
      int leftPWM = cmd.substring(firstComma + 1, secondComma).toInt();
      int rightPWM = cmd.substring(secondComma + 1).toInt();
      setVibration(leftPWM, rightPWM);
    }
  }
  else if (action == "TAP") {
    // Format: TAP,<leftPWM>,<rightPWM>,<duration>
    int secondComma = cmd.indexOf(',', firstComma + 1);
    int thirdComma = cmd.indexOf(',', secondComma + 1);
    if (secondComma > 0 && thirdComma > 0) {
      int leftPWM = cmd.substring(firstComma + 1, secondComma).toInt();
      int rightPWM = cmd.substring(secondComma + 1, thirdComma).toInt();
      int duration = cmd.substring(thirdComma + 1).toInt();
      setVibration(leftPWM, rightPWM);
      tapEndTime = millis() + duration;
      rollActive = false;
    }
  }
  else if (action == "STARTROLL") {
    // Format: STARTROLL,<PWM>
    int commaIndex = cmd.indexOf(',');
    if (commaIndex > 0) {
      int strength = cmd.substring(commaIndex + 1).toInt();
      rollActive = true;
      setVibration(strength, strength);
    }
  }
  else if (action == "STOP") {
    rollActive = false;
    stopVibration();
  }
}

void setVibration(int leftStrength, int rightStrength) {
  analogWrite(leftMotorPin, constrain(leftStrength, 0, 255));
  analogWrite(rightMotorPin, constrain(rightStrength, 0, 255));
}

void stopVibration() {
  analogWrite(leftMotorPin, 0);
  analogWrite(rightMotorPin, 0);
}
