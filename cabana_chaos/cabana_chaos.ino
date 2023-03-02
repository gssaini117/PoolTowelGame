/*
Cabana Chaos
*/

//Set analog input pins for game's sensors:
//4 photoresistors detect presence of umbrellas
//umbrella letters map to patrons on screen (A upper left,
int umbrellaPinA = 1;
int umbrellaPinB = 2;
int umbrellaPinC = 14;
int umbrellaPinD = 15;

//2 photoresistors detect presence of towels
//left and right relative to facing screen (maps to towels on screen)
int towelPinLeft = 6;
int towelPinRight = 7;

//2 water sensors to detect wetness of left and right sides of pool
//left and right relative to facing screen Left: Patrons A,B | Right: Patrons C,D
int waterPinLeft = 0;
int waterPinRight = 8;


//to calibrate umbrella and towel photoresistors
//note: when game begins, towels are in place covering photoresistors, umbrellas are off so photoresistors uncovered
int numberCalibrationReads = 50;
//as a starting off point, find average value for each photoresistor based on current lighting conditions
int averageValueUmbrellaA = 0;
int averageValueUmbrellaB = 0;
int averageValueUmbrellaC = 0;
int averageValueUmbrellaD = 0;
int averageValueTowelLeft = 0;
int averageValueTowelRight = 0;

//average values for each photoresistor-will use this for towel detection
int averageValueOverallLight = 0;

//create threshold multipliers for the umbrellas and towels
float thresholdMultiplierUmbrella = 0.75;
float thresholdMultiplierTowel = 1.25;


//set variables to track state of the umbrella photoresistors
//where 0 = uncovered, 1 = covered
//game begins with umbrellas not covering patrons
int stateUmbrellaA = 0;
int stateUmbrellaB = 0;
int stateUmbrellaC = 0;
int stateUmbrellaD = 0;

//set variables to track state of the towel photoresistors
//where 0 = towel present, 1 = towel missing
//game begins with towels present
int stateTowelLeft = 1;
int stateTowelRight = 1;

//set variables to monitor presence of water
//where 0 = dry, 1 = wet
//game begins with water sensors dry
int stateWaterLeft = 0;
int stateWaterRight = 0;

//set a minimum value to consider when a water sensor is wet
int minDetectedWaterValue = 100;


void setup() {
   Serial.begin(9600);

//calibrate to find average values for each of the umbrella and towel photoresistors
//note again that towel photoresistors will be calibrated when covered, and umbrella photoresistors will be calibrated uncovered
   for(int i =0; i < numberCalibrationReads; i++){
    averageValueUmbrellaA += analogRead(umbrellaPinA);
    averageValueUmbrellaB += analogRead(umbrellaPinB);
    averageValueUmbrellaC += analogRead(umbrellaPinC);
    averageValueUmbrellaD += analogRead(umbrellaPinD);
    averageValueTowelLeft += analogRead(towelPinLeft);
    averageValueTowelRight += analogRead(towelPinRight);

    delay(10);
  }
  averageValueUmbrellaA /= numberCalibrationReads;
  averageValueUmbrellaB /= numberCalibrationReads;
  averageValueUmbrellaC /= numberCalibrationReads;
  averageValueUmbrellaD /= numberCalibrationReads;
  averageValueTowelLeft /= numberCalibrationReads;
  averageValueTowelRight /= numberCalibrationReads;

  averageValueOverallLight = ((averageValueUmbrellaA + averageValueUmbrellaB + averageValueUmbrellaC + averageValueUmbrellaD)/4);

/*
//For debugging can check calibrated averages
Serial.println("Average Value A: " + String(averageValueUmbrellaA));
Serial.println("Average Value B: " + String(averageValueUmbrellaB));
Serial.println("Average Value C: " + String(averageValueUmbrellaC));
Serial.println("Average Value D: " + String(averageValueUmbrellaD));
Serial.println("Average Value Towel Left: " + String(averageValueTowelLeft));
Serial.println("Average Value Towel Right: " + String(averageValueTowelRight));
Serial.println("Average Overall Light: " + String(averageValueOverallLight));
Serial.println("_________");
*/

}

void loop() {

//read the values for each of the photoresistors and compare to initial calibrated values to update state
int valueUmbrellaA = analogRead(umbrellaPinA);
int valueUmbrellaB = analogRead(umbrellaPinB);  
int valueUmbrellaC = analogRead(umbrellaPinC);
int valueUmbrellaD = analogRead(umbrellaPinD);  
int valueTowelLeft = analogRead(towelPinLeft);
int valueTowelRight = analogRead(towelPinRight);
//read the values for each of the water sensors  
int valueWaterLeft = analogRead(waterPinLeft);
int valueWaterRight = analogRead(waterPinRight);


//Update each of the umbrella states

//Umbrella A
if(valueUmbrellaA < thresholdMultiplierUmbrella * averageValueUmbrellaA ){
stateUmbrellaA = 1;
//Serial.println("Patron A shaded " + String(valueUmbrellaA) + "," + String(stateUmbrellaA));
}else{
stateUmbrellaA = 0;
//Serial.println("Patron A tanning: " + String(valueUmbrellaA) + "," + String(stateUmbrellaA));
}

//Umbrella B
if(valueUmbrellaB < thresholdMultiplierUmbrella * averageValueUmbrellaB ){
stateUmbrellaB = 1;
//Serial.println("Patron B shaded " + String(valueUmbrellaB) + "," + String(stateUmbrellaB));
}else{
stateUmbrellaB = 0;
//Serial.println("Patron B tanning: " + String(valueUmbrellaB) + "," + String(stateUmbrellaB));
}

//Umbrella C
if(valueUmbrellaC < thresholdMultiplierUmbrella * averageValueUmbrellaC ){
stateUmbrellaC = 1;
//Serial.println("Patron C shaded " + String(valueUmbrellaC) + "," + String(stateUmbrellaC));
}else{
stateUmbrellaC = 0;
//Serial.println("Patron C tanning: " + String(valueUmbrellaC) + "," + String(stateUmbrellaC));
}

//Umbrella D
if(valueUmbrellaD < thresholdMultiplierUmbrella * averageValueUmbrellaD ){
stateUmbrellaD = 1;
//Serial.println("Patron D shaded " + String(valueUmbrellaD) + "," + String(stateUmbrellaD));
}else{
stateUmbrellaD = 0;
//Serial.println("Patron D tanning: " + String(valueUmbrellaD) + "," + String(stateUmbrellaD));
}


//Update both of the towel states

if(valueTowelLeft < thresholdMultiplierUmbrella * averageValueOverallLight){
stateTowelLeft = 1;
//Serial.println("Left towel present: "+ String(valueTowelLeft) + "," + String(stateTowelLeft));
}else{
stateTowelLeft = 0;
//Serial.println("Left towel missing: " + String(valueTowelLeft) + "," + String(stateTowelLeft));
}

if(valueTowelRight < thresholdMultiplierUmbrella * averageValueOverallLight){
stateTowelRight = 1;
//Serial.println("Right towel present: "+ String(valueTowelRight) + "," + String(stateTowelRight));
}else{
stateTowelRight = 0;
//Serial.println("Right towel missing: " + String(valueTowelRight) + "," + String(stateTowelRight));
}


/*
//Left towel
if(valueTowelLeft > thresholdMultiplierTowel * averageValueTowelLeft){
stateTowelLeft = 0;
Serial.println("Left towel missing: " + String(valueTowelLeft) + "," + String(stateTowelLeft));
}else{
stateTowelLeft = 1;
Serial.println("Left towel present: "+ String(valueTowelLeft) + "," + String(stateTowelLeft));
}

//Right Towel
if(valueTowelRight > thresholdMultiplierTowel * averageValueTowelRight){
stateTowelRight = 0;
Serial.println("Right towel missing: " + String(valueTowelRight) + "," + String(stateTowelRight));
}else{
stateTowelRight = 1;
Serial.println("Right towel present: " + String(valueTowelRight) + "," + String(stateTowelRight));
}
*/

//Update wetness states

//Left water sensor
if(valueWaterLeft >= minDetectedWaterValue){
    stateWaterLeft = 1;
    //Serial.println("Left wet: " + String(valueWaterLeft) + "," + String(stateWaterLeft));
  }else{
    stateWaterLeft = 0;
    //Serial.println("Left dry: " + String(valueWaterLeft)+ "," + String(stateWaterLeft));
  }
  
//Right water sensor
if(valueWaterRight >= minDetectedWaterValue){
    stateWaterRight = 1;
    //Serial.println("Right wet: " + String(valueWaterRight) + "," + String(stateWaterRight));
  }else{
    stateWaterRight = 0;
    //Serial.println("Right dry: "+ String(valueWaterRight) + "," + String(stateWaterRight));
  }


//Serial.println("_________");

//Communication Protocol
//Commas separating each sensor value
//Order of values: umbrellaA, umbrellaB, umbrellaC, umbrellaD, towelLeft, towelRight, waterLeft, waterRight
// 0 = absent/dry  1 = present/wet


Serial.println(String(stateUmbrellaA) + "," + String(stateUmbrellaB) + "," + String(stateUmbrellaC) + "," + String(stateUmbrellaD) + "," + String(stateTowelLeft) + "," + String(stateTowelRight) + "," + String(stateWaterLeft) + "," + String(stateWaterRight));

delay(50);
}
