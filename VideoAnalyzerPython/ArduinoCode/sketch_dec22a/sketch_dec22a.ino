#include <TM1637Display.h>
const int CLK = 12; //Set the CLK
const int DIO = 13; //Set the DIO
TM1637Display display(CLK, DIO);  //set up the 4-Digit Display.

// CLK - 12
// DIO - 13
// VCC - 5v
// GND - GND

void setup()
{
  Serial.begin(9600);
  display.setBrightness(0x0a);  //set the diplay to maximum brightness
  display.showNumberDec(0);
}

String inString = "";

void loop() {
  // Read serial input:
  while (Serial.available() > 0) {
    int inChar = Serial.read();
    if (isDigit(inChar)) {
      // convert the incoming byte to a char and add it to the string:
      inString += (char)inChar;
    }
    if (inChar == '\n') {
      display.showNumberDec(inString.toInt());
      inString = "";
    }
  }
}


/*
void loop()
{
  valueN = Serial.readString().toInt();
  if (valueN != valueO){
    data = valueN;
    display.showNumberDec(data); //Display the Variable value;
    valueO = valueN;
  }
}
*/
