#include <ESP8266WiFi.h>
#include <SimplePortal.h>
#include <ESP8266WebServer.h>
#include <Arduino.h>
#include <EEPROM.h>
#include "DHT.h"
#include <GyverButton.h>
//#include <GyverOLED.h>


#define SENSOR_PIN D1
#define DHT_TYPE DHT11 
#define DHT_PIN D2   
#define SPIC_PIN D4
#define RELE_PIN D3

#define connection_attempts 25

const String S_code = "tfCKT-UeQ7W-24yRK-L8PX2-D9waL";

//Код защит

// Имя и пароль вашей сети WiFi
String ssid = "Wifi";
String password = "12345678";

bool is_Flag = 0;
bool is_Invert = 0;
int Mode = 1;
boolean is_ON;
byte EEdel;

uint32_t delays=600000;
uint32_t myTimer1;
uint32_t myTimer2;



ESP8266WebServer HTTP(80);
GButton butt(SENSOR_PIN);

//GyverOLED<SSD1306_128x64, OLED_NO_BUFFER> oled;
DHT dht(DHT_PIN, DHT_TYPE);


void setup() {
  EEPROM.begin(512);
  Serial.begin(9600);
  delay(10);
  butt.setType(HIGH_PULL);
  pinMode(RELE_PIN, OUTPUT);
  pinMode(SPIC_PIN, OUTPUT);

  dht.begin();
  //oled.init();
  //oled.setContrast(255);
  //oled.clear();

 
  //oled.setScale(1); oled.setCursor(1, 1); oled.print("Hello");
  //oled.setCursor(3, 3);oled.print(String(temp));
  
delay(1000); // задержка для возможности сброса настроек
butt.tick();
if(butt.isPress()){
  restart_1(); // сброс SSID and PASS
}
  //tool_for_eeprom(64,70); // заполняет '0' все ячейки с - по
  
  reader();   //чтение из EEPROM
  SO_start(); //звук

  WIFIinit(); // инит. сети попытка подключения, конфиг
  HTTP_init();// инит запросов
  
delay(200);

}



void loop(){
  butt.tick();
  sensor_tick();  // опрос сенсора

  
  debug();
  HTTP.handleClient();
  //delay(1);
}
