#include <LiquidCrystal_I2C.h>

#define PRCT_MODEL "WVP1"

#define SERIAL_BAUD 230400
#define RST_PIN 12
#define ANA_PIN_1 A0
#define ANA_PIN_2 A1
#define IN_VOLTAGE 5

LiquidCrystal_I2C Display(0x3F, 16, 2);


bool is_savemode = false;
float voltage_1;
float voltage_2;
String receivedMessage = "";

#define PRCTINITCMD "prctInit"
void prctInit() {
  Serial.print("{\"M1VAL\":");
  Serial.print(2);
  Serial.print(",");
  Serial.print("\"M2VAL\":");
  Serial.print(2);
  Serial.print("}");
  delay(1000);
  digitalWrite(RST_PIN, LOW); 
}

#define PRCTINFOCMD "prctInfo"
void prctInfo() {
  Serial.print("{\"MODEL\":\"");
  Serial.print(PRCT_MODEL);
  Serial.print("\"}");
}

#define PRCTSAVEMODECMD "prctSave"
void prctSavemode() {
  is_savemode = true;

  Serial.print("{\"M1VAL\":");
  Serial.print(2);
  Serial.print(",");
  Serial.print("\"M2VAL\":");
  Serial.print(2);
  Serial.print("}");

  Display.noDisplay();
  Display.noBacklight();
  Display.off();
}

#define PRCTNORMALMODECMD "prctNormal"
void prctNormalmode() {
  is_savemode = false;

  displayInit();
}

#define DISPLAYINITCMD "displayInit"
void displayInit() {
  Display.init();
  Display.backlight();
  Display.setCursor(0, 0);
  Display.print("Wheel Voyage");
  Display.setCursor(0, 1);
  Display.print("Prototype v1");
}

#define DISPLAYCLEARCMD "displayClear"
void displayClear() {
  Display.clear();
  Display.setCursor(0, 0);
}

void setup() {
  digitalWrite(RST_PIN, HIGH);
  pinMode(RST_PIN, OUTPUT);
  pinMode(ANA_PIN_1, INPUT);
  pinMode(ANA_PIN_2, INPUT);

  Serial.begin(SERIAL_BAUD);
  displayInit();
}

int last_work_time = 0;
int last_work_timeout = 1000;
void loop() {
  if (Serial.available() > 0) {
    String receivedMessage = Serial.readStringUntil('\n'); 
    Serial.print("{");
    Serial.print("\"Received\":\"");
    Serial.print(receivedMessage);
    Serial.println("\"}");

    Display.clear();
    Display.setCursor(0, 0);

    // print receive message
    if (receivedMessage.length() <= 16) {
      Display.print(receivedMessage);
    } else {
      Display.print(receivedMessage.substring(0, 16));
      Display.setCursor(0, 1);
      Display.print(receivedMessage.substring(16));
    }

    // func command
    if (receivedMessage == PRCTINITCMD)
      prctInit();

    if (receivedMessage == DISPLAYINITCMD)
      displayInit();

    if (receivedMessage == DISPLAYCLEARCMD)
      displayClear();

    if (receivedMessage == PRCTSAVEMODECMD)
      prctSavemode();
    
    if (receivedMessage == PRCTNORMALMODECMD)
      prctNormalmode();

    receivedMessage = "";
  }

  if (!is_savemode) {
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
  else {
    delay(1000);
  }
}
