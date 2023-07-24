#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <ArduinoJson.h>
#include <Adafruit_NeoPixel.h>
#ifdef __AVR__
 #include <avr/power.h> // Required for 16 MHz Adafruit Trinket
#endif

// Which pin on the Arduino is connected to the NeoPixels?
// On a Trinket or Gemma we suggest changing this to 1:
#define LED_PIN    2

// How many NeoPixels are attached to the Arduino?
#define LED_COUNT 256

// Declare our NeoPixel strip object:
Adafruit_NeoPixel strip(LED_COUNT, LED_PIN, NEO_GRB + NEO_KHZ800);


const char* ssid = "xxx";
const char* password = "xxx";

ESP8266WebServer server(80);

void handleJSONData()
{
  String jsonStr = server.arg("plain");

  
  //Serial.println(jsonStr);

  DynamicJsonDocument doc(24576);

  DeserializationError error = deserializeJson(doc, jsonStr);

  if (error) {
    Serial.print(F("deserializeJson() failed: "));
    Serial.println(error.f_str());
    return;
  }
  int counter = 0;
  
  //colorWipe(, 50); // Green
  for (JsonObject Color : doc["Colors"].as<JsonArray>()) {
    
    //Serial.println(counter);
    int Color_Red = Color["Red"]; // 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, ...
    //Serial.print(" R=");
    //Serial.println(Color_Red);
    int Color_Green = Color["Green"]; // 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, ...
    //Serial.print(" G=");
    //Serial.println(Color_Green);
    int Color_Blue = Color["Blue"]; // 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, ...
    //Serial.print(" B=");
    //Serial.println(Color_Blue);
    
    strip.setPixelColor(counter, strip.Color(Color_Red, Color_Green,Color_Blue));         //  Set pixel's color (in RAM)
    counter++;

  }
  strip.show();
  Serial.println("Rec Color data");

  server.send(200, "text/plain", "JSON data received successfully.");
  doc.clear();
}

void setup()
{
  // These lines are specifically to support the Adafruit Trinket 5V 16 MHz.
  // Any other board, you can remove this part (but no harm leaving it):

  // END of Trinket-specific code.

  strip.begin();           // INITIALIZE NeoPixel strip object (REQUIRED)
  strip.show();            // Turn OFF all pixels ASAP
  strip.setBrightness(50); // Set BRIGHTNESS to about 1/5 (max = 255)
  Serial.begin(9600);

  // Connect to Wi-Fi
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Connecting to WiFi...");
  }

  Serial.println("WiFi connected.");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());

  // Handle the "/endpoint" route and bind it to the handleJSONData function
  server.on("/endpoint", handleJSONData);

  // Start the server
  server.begin();
}

void loop()
{
  // Handle client requests
  server.handleClient();
}

void colorWipe(uint32_t color, int wait) {
  for(int i=0; i<strip.numPixels(); i++) { // For each pixel in strip...
    strip.setPixelColor(i, color);         //  Set pixel's color (in RAM)
    strip.show();                          //  Update strip to match
    delay(wait);                           //  Pause for a moment
  }
}