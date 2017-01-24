#include <Adafruit_NeoPixel.h>

#define PIN 6
#define NUMPIXELS 50

Adafruit_NeoPixel pixels = Adafruit_NeoPixel(NUMPIXELS, PIN, NEO_RGB | NEO_KHZ800);

int fieldNum = 0;
int fields[10] = {0};

void setup() {
  pixels.begin(); // This initializes the NeoPixel library.
  Serial.begin(115200);
}

//Format: 1,255,255,255;!

void loop() {
  int chr = Serial.peek();
  if (chr != -1) {
    if (chr == ',') {
      Serial.read();
      fieldNum++;
    } else if (chr == ';') {
      Serial.read();
      pixels.setPixelColor(fields[0], pixels.Color(fields[1], fields[2], fields[3]));
      fieldNum = 0;
    } else if (chr == '!') {
      Serial.read();
      pixels.show();
    } else {
      fields[fieldNum] = Serial.parseInt();
    }
  }
}
