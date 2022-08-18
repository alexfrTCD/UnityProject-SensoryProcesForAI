using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ObjectClass;

public class GameController : MonoBehaviour
{
    //This board should be initialized outside of this script. That's why we are doing the infoList.Add()
    WorldClass board;

    SocketScript communicationScript;
    AvatarController avatar;
    SensesControl sensesControl;
    public GameObject Food;
    public GameObject Avatar;
    private Vector3 targetPosition = Vector3.zero;
    float movementSpeed = 1000.0f;
    bool firstInit = true;
    // Start is called before the first frame update
    int actualUnityInstruction = 1;
    float timer;
    string newAction = "init";
    GameObject modifiableobject;
    bool actionFinished = true;
    int counter = 0;
    float temperatureFelt = 0;
    float glareFelt = 0;
    float soundFelt = 0;
    bool rotation = false;
    bool movement = false;
    private bool randomPos = true;
    private void Awake()
    {

        QualitySettings.vSyncCount = 2;
        //Application.targetFrameRate = 300;
    }
    void Start()
    {
        board = GameObject.Find("Board").GetComponent<WorldClass>();


        avatar = GameObject.Find("Avatar").GetComponent<AvatarController>();
        sensesControl = GameObject.Find("Avatar").GetComponent<SensesControl>();
        avatar.transform.forward = new Vector3(0.0f, 0.0f, 1.0f);
        communicationScript = new SocketScript();
        communicationScript.initCommunication(board, avatar.transform.position, avatar.transform.forward);
        Debug.Log("Forward INIT: " + avatar.transform.forward.x.ToString() + "," + avatar.transform.forward.y.ToString() + "," + avatar.transform.forward.z.ToString());


    }

    private void Update()
    {
        avatar.movement();
        if (Input.GetKeyDown("space"))
        {
            sensesControl.viewField();
            float glareFelt = sensesControl.getGlare();
            float temperatureFelt = sensesControl.calculateTemperature(board.getAmbientTemperature());
            float soundFelt = sensesControl.calculateSound();

            Debug.Log("Temp felt: " + temperatureFelt.ToString());
            Debug.Log("Sound felt: " + soundFelt.ToString());
            Debug.Log("Glare felt = " + glareFelt.ToString());
            Tuple<bool, float, float> tuple = checkModifiableAction(newAction);

            avatar.actualizeDrives(tuple, temperatureFelt, glareFelt, soundFelt);
        }


        if (Input.GetKeyDown("l"))
        {
            avatar.changeForward(new Vector3(1.0f, 0.0f, 0.0f));
        }
        if (Input.GetKeyDown("j"))
        {
            avatar.changeForward(new Vector3(-1.0f, 0.0f, 0.0f));
        }
        if (Input.GetKeyDown("k"))
        {
            avatar.changeForward(new Vector3(0.0f, 0.0f, -1.0f));

        }
        if (Input.GetKeyDown("i"))
        {
            avatar.changeForward(new Vector3(0.0f, 0.0f, 1.0f));

        }




        if (communicationScript.getInit())
        {
            sensesControl.viewField();
            //Debug.Log(communicationScript.getNumberInstructions().ToString() + " " + actualUnityInstruction.ToString() + " " + actionFinished.ToString());

            if (firstInit)
            {
                actualizeObservation();
                firstInit = false;
                Debug.Log(communicationScript.getNumberInstructions().ToString() + " " + actualUnityInstruction.ToString() + " " + actionFinished.ToString());
                actualUnityInstruction++;
                Debug.Log("First Init done");

                Debug.Log("ActionFinished = " + actionFinished.ToString());

            }

            else if (communicationScript.getNumberInstructions() == actualUnityInstruction && actionFinished)
            {

                //Debug.Log("At Avatar position: " + avatar.transform.position.x + "," + avatar.transform.position.z + " and forward " + avatar.transform.forward + " glare is: " + glareFelt.ToString() + "" + " Temp felt is: " + temperatureFelt.ToString() + " and Sound felt: " + soundFelt.ToString());

                rotation = false;
                movement = false;
                //RECEIVING NEW ACTION DECIDED (PYTHON)
                newAction = communicationScript.getAction();
                //Debug.Log("Action received " + newAction);
                //newEpsilon = communicationScript.receiveEpsilon();
                //Debug.Log("Instruction: " + actualUnityInstruction.ToString());
                //Checking the new view after the movement
                if (newAction == "up" || newAction == "moveUpRot")
                {
                    targetPosition = avatar.transform.position + new Vector3(0.0f, 0.0f, 1.0f);

                    movement = true;
                    if (newAction == "moveUpRot")
                    {
                        rotation = true;
                    }
                }
                else if (newAction == "down" || newAction == "moveDownRot")
                {
                    targetPosition = avatar.transform.position + new Vector3(0.0f, 0.0f, -1.0f);
                    movement = true;
                    if (newAction == "moveDownRot")
                    {
                        rotation = true;
                    }

                }
                else if (newAction == "right" || newAction == "moveRightRot")
                {
                    targetPosition = avatar.transform.position + new Vector3(1.0f, 0.0f, 0.0f);
                    movement = true;
                    if (newAction == "moveRightRot")
                    {
                        rotation = true;
                    }



                }
                else if (newAction == "left" || newAction == "moveLeftRot")
                {
                    targetPosition = avatar.transform.position + new Vector3(-1.0f, 0.0f, 0.0f);
                    movement = true;
                    if (newAction == "moveLeftRot")
                    {
                        rotation = true;
                    }

                }

                else if (newAction == "rotRight")
                {
                    //avatar.transform.forward = new Vector3(1.0f, 0.0f, 0.0f);
                    avatar.changeForward(new Vector3(1.0f, 0.0f, 0.0f));
                }
                else if (newAction == "rotLeft")
                {
                    //avatar.transform.forward = new Vector3(-1.0f, 0.0f, 0.0f);
                    avatar.changeForward(new Vector3(-1.0f, 0.0f, 0.0f));
                }
                else if (newAction == "rotUp")
                {
                    //avatar.transform.forward  =  new Vector3(0.0f, 0.0f, 1.0f);
                    avatar.changeForward(new Vector3(0.0f, 0.0f, 1.0f));
                }
                else if (newAction == "rotDown")
                {
                    //avatar.transform.forward  =  new Vector3(0.0f, 0.0f, -1.0f);
                    avatar.changeForward(new Vector3(0.0f, 0.0f, -1.0f));
                }

                else if (newAction == "eat")
                {
                    targetPosition = avatar.transform.position + new Vector3((int)avatar.transform.forward.x, (int)avatar.transform.forward.y, (int)avatar.transform.forward.z);
                    //Debug.Log("Avatar position in eat: " + avatar.transform.position.x.ToString() + ","+ avatar.transform.position.y.ToString() + "," +avatar.transform.position.z.ToString());
                    //Debug.Log("Forward Position in eat: " + avatar.transform.forward.x.ToString()+ ","+ avatar.transform.forward.y.ToString() + "," +avatar.transform.forward.z.ToString());
                    //Debug.Log("Target Position in eat: " + targetPosition.x.ToString()+ ","+ targetPosition.y.ToString()+ "," +targetPosition.z.ToString());
                }

                //Debug.Log("Target position:" + targetPosition.x.ToString() + " " + targetPosition.z.ToString());
                actionFinished = false;
                actualUnityInstruction++;

            }

            if (!actionFinished)
            {
                //Debug.Log("action not finished");
                if (newAction == "eat")
                {

                    eat(targetPosition);
                    actionFinished = true;
                    actualizeObservation();
                }

                else if (newAction == "left" || newAction == "moveLeftRot" || newAction == "right" || newAction == "moveRightRot" || newAction == "down" || newAction == "moveDownRot" || newAction == "up" || newAction == "moveUpRot")
                {
                    actionExecuted(newAction, rotation, movement);
                }

                else if (newAction == "rotLeft" || newAction == "rotRight" || newAction == "rotUp" || newAction == "rotDown" || newAction == "idle")
                {
                    //sensesControl.viewField();
                    actualizeObservation();
                    actionFinished = true;
                }

                else if (newAction == "notAchieved")
                {



                    float positionX = communicationScript.receiveNumber();
                    float positionZ = communicationScript.receiveNumber();

                    GameObject objectPosition = GameObject.Find("Food(Clone)");
                    if (objectPosition == null)
                    {
                        objectPosition = GameObject.Find("Food");
                    }

                    //Debug.Log("Object position trying to eat: " + objectPosition.ToString());

                    List<Vector3> possiblePositions = board.getPossiblePositions();
                    //Now we have to do the following:
                    //1: Destroy the food in the scene
                    //2: Create the new food in the new position in the scene
                    //3: Actualize all list

                    List<ObjectClass> actualObjects = board.geTotalObjects();
                    bool founded = false;
                    int counter = 0;

                    while (founded == false && counter < actualObjects.Count)
                    {
                        if (actualObjects[counter].transform.position.x == objectPosition.transform.position.x && actualObjects[counter].transform.position.z == objectPosition.transform.position.z)
                        {


                            List<ObjectClass> hearingList = sensesControl.getTouchList();
                            hearingList.Remove(actualObjects[counter]);
                            sensesControl.setTouchList(hearingList);


                            List<ObjectClass> sightList = sensesControl.getSightList();
                            sightList.Remove(actualObjects[counter]);
                            sensesControl.setSightList(sightList);

                            //Debug.Log("Steps completed!");
                            //Step 1
                            Destroy(actualObjects[counter].gameObject);
                            //Step 2
                            GameObject newFood = GameObject.Instantiate(Food, new Vector3(positionX, -0.2f, positionZ), Quaternion.identity);
                            Temperature temp = new Temperature();
                            temp.setDispersion(0);
                            temp.setCelsius(0);
                            newFood.GetComponent<ObjectClass>().setTemperature(temp);
                            //Step 3

                            //Remove from the objectClass list. Actualizing the board
                            actualObjects.RemoveAt(counter);
                            actualObjects.Add(newFood.GetComponent<ObjectClass>());
                            board.setTotalObjects(actualObjects);

                            List<ObjectClass> actualFood = board.getFood();
                            actualFood.Remove(actualObjects[counter]);
                            actualFood.Add(newFood.GetComponent<ObjectClass>());
                            board.setFood(actualFood);

                            //actualize possiblePositions
                            possiblePositions.Remove(new Vector3(positionX, 0, positionZ));
                            possiblePositions.Add(new Vector3(objectPosition.transform.position.x, 0, objectPosition.transform.position.z));
                            board.setPossiblePositions(possiblePositions);
                            founded = true;

                        }
                        else
                        {
                            counter++;
                        }
                    }

                    if (randomPos == false)
                    {
                        avatar.transform.position = new Vector3(0.0f, -1.0f, 0.0f);
                        avatar.changeForward(new Vector3(0.0f, 0.0f, 1.0f));
                    }

                    communicationScript.setAction("other");
                    actionFinished = true;

                }
            }



        }

    }


    void createNewFood()
    {
        communicationScript.actualizePossiblePositions(board);
    }

    void actionExecuted(string newAction, bool rotation, bool movement)
    {

        //This if checks if the avatar is already in his target position or not
        if (Vector3.Distance(avatar.transform.position, targetPosition) > 0.01f)
        {
            //Debug.Log("Walk");
            //Debug.Log("Move to: (" + targetPosition.x.ToString() + targetPosition.y.ToString() + targetPosition.z.ToString() + ")");
            //Debug.Log("Action: " + newAction);


            if (movement)
            {
                if (rotation)
                {
                    avatar.rotateToObject(targetPosition, movementSpeed * Time.deltaTime);
                }

                avatar.moveToObject(targetPosition, movementSpeed * Time.deltaTime);
                avatar.playAnimation("Speed", 0.5f);
            }



        }

        else
        {


            if (newAction == "moveUpRot")
            {
                avatar.changeForward(new Vector3(0.0f, 0.0f, 1.0f));
            }
            else if (newAction == "moveDownRot")
            {
                avatar.changeForward(new Vector3(0.0f, 0.0f, -1.0f));

            }
            else if (newAction == "moveRightRot")
            {
                avatar.changeForward(new Vector3(1.0f, 0.0f, 0.0f));
            }
            else if (newAction == "moveLeftRot")
            {
                avatar.changeForward(new Vector3(-1.0f, 0.0f, 0.0f));
            }
            //Debug.Log("Avatar position: " + avatar.transform.position.x.ToString() + "," + avatar.transform.position.y.ToString() + "," + avatar.transform.position.z.ToString());
            //Debug.Log("Forward Position: " + avatar.transform.forward.x.ToString() + "," + avatar.transform.forward.y.ToString() + "," + avatar.transform.forward.z.ToString());
            //Debug.Log("Target Position: " + targetPosition.x.ToString() + "," + targetPosition.y.ToString() + "," + targetPosition.z.ToString());

            //Debug.Log("Inside else distance");
            actualizeObservation();
            actionFinished = true;
        }




    }

    private void actualizeObservation()
    {
        //SENDING OBSERVATION
        //Debug.Log("Observation: "+ avatar.transform.position.ToString()+ avatar.transform.rotation.ToString()+ avatar.getHealth()+ avatar.getHunger()+ temperatureFelt.ToString()+ glareFelt.ToString());
        //sensesControl.printSightList();


        //Debug.Log("Actual position = " + avatar.transform.position.ToString());
        //Debug.Log("Actual rotation: " + avatar.transform.rotation.ToString());
        glareFelt = sensesControl.getGlare();
        temperatureFelt = sensesControl.calculateTemperature(board.getAmbientTemperature());
        //Debug.Log("Glare felt = " + glareFelt.ToString());
        //Debug.Log("Temp felt: " + temperatureFelt.ToString());
        //Debug.Log("Sound felt: " + soundFelt.ToString());
        soundFelt = sensesControl.calculateSound();
        //Debug.Log("Sound felt: " + soundFelt);
        sendSensesInfo(temperatureFelt, glareFelt, soundFelt);
        //Debug.Log("Observation:" + temperatureFelt.ToString() + "," + glareFelt.ToString() + "," + soundFelt.ToString());
        //Debug.Log("Avatar Position: " + avatar.transform.position.x.ToString() + " " + avatar.transform.position.z.ToString());
        //Debug.Log("Avatar Rotation: " + avatar.transform.rotation.x.ToString() + " " + avatar.transform.rotation.z.ToString());
        //Debug.Log("Avatar Forward: " + avatar.transform.forward.x.ToString() + " " + avatar.transform.forward.z.ToString());

        sendObjectsInfo();


        Tuple<bool, float, float> tuple = checkModifiableAction(newAction);

        avatar.actualizeDrives(tuple, temperatureFelt, glareFelt, soundFelt);
        sendDrivesInfo();
        avatar.checkRestart();

        if (newAction == "eat")
        {
            //Debug.Log("Inside eat possiblity");
            communicationScript.actualizePossiblePositions(board);
        }
        //OBSERVATION SENDED

    }

    private void sendDrivesInfo()
    {
        communicationScript.sendNewValue(avatar.getHealth());
        //Debug.Log("Actual heatlh" + avatar.getHealth());
        communicationScript.sendNewValue(avatar.getHunger());
        //Debug.Log("Actual hunguer" + avatar.getHunger());
    }

    private Tuple<bool, float, float> checkModifiableAction(string actionChecked)
    {
        Tuple<bool, float, float> tuple = new Tuple<bool, float, float>(false, 0, 0);
        //Debug.Log("Action inside checkModifiable: " + actionChecked);
        if (actionChecked == "eat")
        {
            if (modifiableobject != null)
            {
                if (modifiableobject.tag == "Apple")
                {
                    tuple = new Tuple<bool, float, float>(true, -50, 0);
                }
                else if (modifiableobject.tag == "Fire" || modifiableobject.tag == "Bulb" || modifiableobject.tag == "Speaker")
                {
                    tuple = new Tuple<bool, float, float>(true, 0, -20);
                }
                else
                {
                    tuple = new Tuple<bool, float, float>(false, 0, 30);
                    Debug.Log("Unknown object tried to eat");
                }
            }
            else
            {
                tuple = new Tuple<bool, float, float>(false, 0, 0);
                Debug.Log("Modifiable object is equals to null");
            }

        }

        return tuple;
    }

    private void sendSensesInfo(float temperature, float glare, float soundIntensity)
    {

        communicationScript.sendNewValue(temperature);
        communicationScript.sendNewValue(glare);
        communicationScript.sendNewValue(soundIntensity);
    }


    //if (Input.GetKeyDown(KeyCode.Return))
    //{
    //    Debug.Log("Already bored, finishing the trial");
    //    EditorApplication.isPlaying = false;
    //    //This wouldn't stop the game in a real build. Then you should use Application.Quit(), and the application will c e

    //}




    private void sendObjectsInfo()
    {
        counter++;

        //Debug.Log("Counter: " + counter.ToString());
        //Sending the objects positions detected by smell and sight
        //sigtList
        List<ObjectClass> sightList = sensesControl.getSightList();
        communicationScript.sendNewList(sightList, "sight");
        //for(int i = 0; i < sightList.Count; i++)
        //{
        //    Debug.Log("object:" + sightList[i].tag + sightList[i].transform.position.x + ","+ sightList[i].transform.position.z);
        //}

        //HearingList
        //communicationScript.sendNewList(sensesControl.getHearingList(), "hearing");

        //touchLst
        communicationScript.sendNewList(sensesControl.getTouchList(), "touch");

    }


    void eat(Vector3 objectPosition)
    {
        //Debug.Log("Object position trying to eat: " + objectPosition.ToString());
        //Debug.Log("eat action");
        //Vector3 newFoodPosition = communicationScript.getNewFood(); This line is when we receive the new object from python
        List<Vector3> possiblePositions = board.getPossiblePositions();
        Vector3 newFoodPosition = new Vector3();
        if (randomPos == false)
        {
            possiblePositions.Remove(new Vector3(0, 0, 0));
            int index = UnityEngine.Random.Range(0, possiblePositions.Count);
            newFoodPosition = possiblePositions[index];

            possiblePositions.Add(new Vector3(0, 0, 0));
        }
        else
        {
            possiblePositions.Remove(new Vector3(avatar.transform.position.x, 0, avatar.transform.position.z));
            int index = UnityEngine.Random.Range(0, possiblePositions.Count);
            newFoodPosition = possiblePositions[index];

            possiblePositions.Add(new Vector3(avatar.transform.position.x, 0, avatar.transform.position.z));
        }

        //Now we have to do the following:
        //1: Destroy the food in the scene
        //2: Create the new food in the new position in the scene
        //3: Actualize the foodList in board

        List<ObjectClass> actualObjects = board.geTotalObjects();
        bool founded = false;
        int counter = 0;

        while (founded == false && counter < actualObjects.Count)
        {
            //Debug.Log("actual game object: " + actualGameObject[counter].transform.position.x + " " + actualGameObject[counter].transform.position.z);
            //Debug.Log("Target position: " + targetPosition.x + " " + targetPosition.z);
            if (actualObjects[counter].transform.position.x == objectPosition.x && actualObjects[counter].transform.position.z == objectPosition.z)
            {
                //Debug.Log("Object found");  
                modifiableobject = actualObjects[counter].gameObject;
                //Eat animation.

                if (actualObjects[counter].tag == "Apple")
                {
                    avatar.playAnimation("Speed", 1.0f);

                    //Debug.Log("eat action with apple");
                    //Printing possible food before eating
                    //Debug.Log("Possible positions before eating");
                    //for (int i=0; i<possiblePositions.Count; i++)
                    //{
                    //    Debug.Log("Possible position: " + possiblePositions[i].x.ToString() + possiblePositions[i].y.ToString() + possiblePositions[i].z.ToString());
                    //}

                    List<ObjectClass> hearingList = sensesControl.getTouchList();
                    hearingList.Remove(actualObjects[counter]);
                    sensesControl.setTouchList(hearingList);


                    List<ObjectClass> sightList = sensesControl.getSightList();
                    sightList.Remove(actualObjects[counter]);
                    sensesControl.setSightList(sightList);

                    Debug.Log("YUM!");
                    //Step 1
                    Destroy(actualObjects[counter].gameObject);
                    //Step 2
                    GameObject newFood = GameObject.Instantiate(Food, new Vector3(newFoodPosition.x, -0.2f, newFoodPosition.z), Quaternion.identity);
                    Temperature temp = new Temperature();
                    temp.setDispersion(0);
                    temp.setCelsius(0);
                    newFood.GetComponent<ObjectClass>().setTemperature(temp);


                    //Remove from the objectClass list. Actualizing the board
                    actualObjects.RemoveAt(counter);
                    actualObjects.Add(newFood.GetComponent<ObjectClass>());
                    board.setTotalObjects(actualObjects);

                    List<ObjectClass> actualFood = board.getFood();
                    actualFood.Remove(actualObjects[counter]);
                    actualFood.Add(newFood.GetComponent<ObjectClass>());
                    board.setFood(actualFood);

                    //actualize possiblePositions
                    possiblePositions.Remove(new Vector3(newFoodPosition.x, 0, newFoodPosition.z));
                    possiblePositions.Add(new Vector3(objectPosition.x, 0, objectPosition.z));
                    board.setPossiblePositions(possiblePositions);

                    if (randomPos == false)
                    {
                        avatar.transform.position = new Vector3(0.0f, -1.0f, 0.0f);
                        avatar.changeForward(new Vector3(0.0f, 0.0f, 1.0f));
                    }

                    //Debug.Log("Checking new pos in gameController");
                    //Printing possible food before eating
                    //Debug.Log("Possible positions after eating");
                    //for (int i = 0; i < possiblePositions.Count; i++)
                    //{
                    //    Debug.Log("Possible position: " + possiblePositions[i].x.ToString() + possiblePositions[i].y.ToString() + possiblePositions[i].z.ToString());
                    //}
                    //Loop done

                }


                else
                {

                    //Avatar tried to eat another object
                    avatar.playAnimation("Speed", 2.0f);
                    //if (actualGameObject[counter].tag == "Fire")
                    //{
                    //    modifiableobject = actualGameObject[counter];
                    //    //Debug.Log("Avatar tried to eat a fire");
                    //}

                    //else if (actualGameObject[counter].tag == "Bulb")
                    //{
                    //    modifiableobject = actualGameObject[counter];
                    //    //Debug.Log("Avatar tried to eat a bulb");
                    //}
                    //else if (actualGameObject[counter].tag == "Speaker")
                    //{
                    //    modifiableobject = actualGameObject[counter];
                    //    //Debug.Log("Avatar tried to eat a bulb");
                    //}
                    //else
                    //{
                    //    Debug.Log("Object tag: " + actualGameObject[counter].tag);
                    //    Debug.Log("Unknown object");
                    //    //Debug.Log("Eat action with unknown");
                    //    //modifiableobject = "Unknown";
                    //    modifiableobject = null;
                    //}

                }
                founded = true;
            }
            else
            {
                counter++;
            }
        }
        if (founded == false)
        {
            Debug.Log("Object not found");
        }

    }

}
