#include <LiquidCrystal_I2C.h>

#define RST_PIN 12
#define ANA_PIN_1 A0
#define ANA_PIN_2 A1
#define IN_VOLTAGE 5

LiquidCrystal_I2C display(0x3F, 16, 2);

float voltage_1;
float voltage_2;
String receivedMessage = "";

#define PRCTINITCMD "prctInit"
void prctInit() {
  Serial.print("{\"M1VAL\":");
  Serial.print(0);
  Serial.print(",");
  Serial.print("\"M2VAL\":");
  Serial.print(0);
  Serial.print("}");
  delay(100);
  digitalWrite(RST_PIN, LOW); 
}

#define DISPLAYINIT "displayInit"
void displayInit() {
  Serial.begin(9600);
  display.init();
  display.backlight();

  display.setCursor(0, 0);
  display.print("Wheel Voyage");
  display.setCursor(0, 1);
  display.print("Prototype v1");
}

#define DISPLAYCLEAR "displayClear"
void displayClear() {
  display.clear();
  display.setCursor(0, 0);
}

void setup() {
  digitalWrite(RST_PIN, HIGH);
  pinMode(RST_PIN, OUTPUT);
  pinMode(ANA_PIN_1, INPUT);
  pinMode(ANA_PIN_2, INPUT);

  displayInit();
}

void loop() {
  if (Serial.available() > 0) {
    String receivedMessage = Serial.readStringUntil('\n'); 
    Serial.print("{");
    Serial.print("\"Received\":\"");
    Serial.print(receivedMessage);
    Serial.println("\"}");

    display.clear();
    display.setCursor(0, 0);

    // print receive message
    if (receivedMessage.length() <= 16) {
      display.print(receivedMessage);
    } else {
      display.print(receivedMessage.substring(0, 16));
      display.setCursor(0, 1);
      display.print(receivedMessage.substring(16));
    }

    // func command
    if (receivedMessage == PRCTINITCMD)
      prctInit();

    if (receivedMessage == DISPLAYINIT)
      displayInit();

    if (receivedMessage == DISPLAYCLEAR)
      displayClear();

    receivedMessage = "";
  }

  voltage_1 = analogRead(ANA_PIN_1) * IN_VOLTAGE * 0.00489;
  voltage_2 = analogRead(ANA_PIN_2) * IN_VOLTAGE * 0.00489;

  Serial.print("{\"M1VAL\":");
  Serial.print(voltage_1);
  Serial.print(",");
  Serial.print("\"M2VAL\":");
  Serial.print(voltage_2);
  Serial.print("}");

  Serial.println();
  delay(10);
}
