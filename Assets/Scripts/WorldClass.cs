using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectClass;

public class WorldClass : MonoBehaviour
{
    [SerializeField]
    private float ambientTemperature = 25.0f;
    private int rows = 5;
    private int cols = 5;
    //Lists with the objects class, to be able to access to the class in other srips. I can't acces to the components of the gameObjects from classes that don't inherit frmo MonoBehaviour, like SocketScript
    //UnityException: GetComponentFastPath can only be called from the main thread. If I try to get the ObjectComponent from the SocketScript
    private List<ObjectClass> foods = new List<ObjectClass>();
    private List<ObjectClass> totalObjects = new List<ObjectClass>();
    private List<Vector3> possiblePositions = new List<Vector3>();
    public List<GameObject> speakers = new List<GameObject>();
    private void Start()
    {
        initPossiblPositions();
        initObjects();

    }

    private void initPossiblPositions()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int x = 0; x < cols; x++)
            {
                possiblePositions.Add(new Vector3(x, 0, i));
            }
        }

    }

    private void initObjects()
    {
        GameObject[] fireTag = GameObject.FindGameObjectsWithTag("Fire");
        //Debug.Log("Number of fires: " + fireTag.Length.ToString()); 
        GameObject[] bulbTag = GameObject.FindGameObjectsWithTag("Bulb");
        //Debug.Log("Number of bulbs: " + bulbTag.Length.ToString());
        GameObject[] foodObjectsTag = GameObject.FindGameObjectsWithTag("Apple");
        //Debug.Log("Number of appls: " + foodObjectsTag.Length.ToString());
        GameObject[] speakerObjectsTag = GameObject.FindGameObjectsWithTag("Speaker");

        for (int i = 0; i < fireTag.Length; i++)
        {
            //Debug.Log("fireTag position removed: " + fireTag[i].transform.position.x.ToString() + fireTag[i].transform.position.x.ToString());
            //Debug.Log("Fire position removed: " + fireTag[i].transform.position.x + "," + fireTag[i].transform.position.z);
            possiblePositions.Remove(new Vector3(fireTag[i].transform.position.x, 0, fireTag[i].transform.position.z));
            Temperature temp = fireTag[i].GetComponent<ObjectClass>().getTemperature();
            float colliderRadius = (ambientTemperature - temp.getCelsius()) / -temp.getDispersion();
            Debug.Log("Collider Radius = " + colliderRadius.ToString());
            fireTag[i].GetComponent<ObjectClass>().setTemperatureCollider(colliderRadius * 2);

            totalObjects.Add(fireTag[i].GetComponent<ObjectClass>());
        }

        for (int i = 0; i < foodObjectsTag.Length; i++)
        {
            //Debug.Log("Food position removed: " + foodObjectsTag[i].transform.position.x.ToString() + foodObjectsTag[i].transform.position.x.ToString());
            possiblePositions.Remove(new Vector3(foodObjectsTag[i].transform.position.x, 0, foodObjectsTag[i].transform.position.z));
            //Debug.Log("Food position removed: " + foodObjectsTag[i].transform.position.x+ ","+ foodObjectsTag[i].transform.position.z);
            //for (int x = 0; x < possiblePositions.Count; x++)
            //{
            //    Debug.Log("Possible position: " + possiblePositions[x].x.ToString()+ ","+ possiblePositions[x].z.ToString());
            //}
            totalObjects.Add(foodObjectsTag[i].GetComponent<ObjectClass>());
            foods.Add(foodObjectsTag[i].GetComponent<ObjectClass>());
        }
        for (int i = 0; i < bulbTag.Length; i++)
        {
            //Debug.Log("bulbTag position removed: " + bulbTag[i].transform.position.x.ToString() + bulbTag[i].transform.position.x.ToString());
            possiblePositions.Remove(new Vector3(bulbTag[i].transform.position.x, 0, bulbTag[i].transform.position.z));
            //Debug.Log("Bulb position removed: " + bulbTag[i].transform.position.x + "," + bulbTag[i].transform.position.z);
            Temperature temp = bulbTag[i].GetComponent<ObjectClass>().getTemperature();
            float colliderRadius = (ambientTemperature - temp.getCelsius()) / -temp.getDispersion();
            bulbTag[i].GetComponent<ObjectClass>().setTemperatureCollider(colliderRadius * 2);

            totalObjects.Add(bulbTag[i].GetComponent<ObjectClass>());
        }

        for (int i = 0; i < speakerObjectsTag.Length; i++)
        {
            //Debug.Log("bulbTag position removed: " + bulbTag[i].transform.position.x.ToString() + bulbTag[i].transform.position.x.ToString());
            possiblePositions.Remove(new Vector3(speakerObjectsTag[i].transform.position.x, 0, speakerObjectsTag[i].transform.position.z));
            //Debug.Log("Bulb position removed: " + speakerObjectsTag[i].transform.position.x + "," + speakerObjectsTag[i].transform.position.z);

            //For the inverse square relationship --> intensityRedcution = 20*log(distance2/distance1) and distance1 is always 0.5 : https://www.qlight.com/productdata/techdata/en/08_Intensity_of_Sound.pdf
            //Distance1 should be defined by the scale of the avatar and the scale of the object which transmits sound

            //With this equation, we will find the distance where the sound reduction is equal to soundIntensity with: 10^(y/20) * 0.5 = distance, where y is the soundIntensity felt at distance1
            float distance = (float)(Mathf.Pow(10, speakerObjectsTag[i].GetComponent<ObjectClass>().getSoundIntensity() / 20) * 0.01);
            Debug.Log("Object intensiy: " + speakerObjectsTag[i].GetComponent<ObjectClass>().getSoundIntensity().ToString() + "and distance: " + distance);
            //We have to set the max distance depending on the intensity

            speakerObjectsTag[i].GetComponent<ObjectClass>().setSoundCollider(distance * 2);
            speakerObjectsTag[i].GetComponent<AudioSource>().maxDistance = distance;

            speakers.Add(speakerObjectsTag[i]);

            totalObjects.Add(speakerObjectsTag[i].GetComponent<ObjectClass>());
        }

        //for (int i = 0; i < obstaclesObjectsTag.Length; i++)
        //{
        //    possiblePositions.Remove(new Vector3(obstaclesObjectsTag[i].transform.position.x, 0, obstaclesObjectsTag[i].transform.position.z));
        //    totalObjectsGameObject.Add(obstaclesObjectsTag[i]);
        //}

        //for (int i = 0; i < totalObjectsGameObject.Count; i++)
        //{
        //    Debug.Log("Object: " + totalObjectsGameObject[i].name + " at position " + totalObjectsGameObject[i].transform.position.x+ "," + totalObjectsGameObject[i].transform.position.z);
        //}
    }

    public int getRows()
    {
        return rows;
    }
    public int getColumns()
    {
        return cols;
    }
    public List<ObjectClass> geTotalObjects()
    {
        return totalObjects;
    }
    public void setTotalObjects(List<ObjectClass> totalObjects)
    {
        this.totalObjects = totalObjects;
    }
    public List<ObjectClass> getFood()
    {
        return foods;
    }
    public void setFood(List<ObjectClass> possibleFoods)
    {
        foods = possibleFoods;
    }
    public List<Vector3> getPossiblePositions()
    {
        return possiblePositions;
    }
    public void setPossiblePositions(List<Vector3> possiblePositions)
    {
        this.possiblePositions = possiblePositions;
    }


    public float getAmbientTemperature()
    {
        return ambientTemperature;
    }

    public void setAmbientTemperature(float ambientTemperature)
    {
        this.ambientTemperature = ambientTemperature;
    }

}
