using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectClass : MonoBehaviour
{
    //Temperature struct
    public struct Temperature
    {
        private float celsius;
        private float dispersion;

        public float getCelsius()
        {
            return celsius;
        }
        public float getDispersion()
        {
            return dispersion;
        }

        public void setCelsius(float celsius)
        {
            this.celsius = celsius;
        }
        public void setDispersion(float dispersion)
        {
            this.dispersion = dispersion;
        }
    }

    private float distanceToAvatar { get; set; } = -0.1f;
    private string objectType { get; set; }

    public Temperature temperature = new Temperature();
    private float lightIntensity { get; set; }
    private float soundIntensity { get; set; }

    private GameObject temperatureCollider;

    private GameObject soundCollider;   
    // Start is called before the first frame update

    void Awake()
    {
        if (this.tag == "Apple")
        {
            objectType = "Food";
            temperature.setCelsius(25);
            temperature.setDispersion(25);
            lightIntensity = 0;
        }
        else if (this.tag == "Fire")
        {
            if(this.name == "FireFixed1")
            {
                Debug.Log("FireFixed1 set");
                objectType = "Fire";
                temperatureCollider = this.gameObject.transform.GetChild(1).gameObject;
                float intensityValue = Random.Range(55, 75);
                temperature.setCelsius(60);
                temperature.setDispersion(10);
            }

            else if (this.name == "FireFixed2")
            {
                Debug.Log("FireFixed2 set");
                objectType = "Fire";
                temperatureCollider = this.gameObject.transform.GetChild(1).gameObject;
                float intensityValue = Random.Range(55, 75);
                temperature.setCelsius(75);
                temperature.setDispersion(15);
            }

            else
            {
                Debug.Log("FireFixed3 set");
                objectType = "Fire";
                temperatureCollider = this.gameObject.transform.GetChild(1).gameObject;
                float intensityValue = Random.Range(55, 75);
                temperature.setCelsius(60);
                temperature.setDispersion(10);
            }


            soundIntensity = 30.0f;
            lightIntensity = 15;

        }

        else if (this.tag == "Bulb")
        {
            objectType = "Bulb";
            temperatureCollider = this.gameObject.transform.GetChild(1).gameObject;
            temperature.setCelsius(35);
            temperature.setDispersion(15);

            //https://technical-tips.com/blog/hardware/luminance-cdm2-explained-simply--33616. The higher the candela, the higher the brightness
            lightIntensity = 1000f * ((this.gameObject.transform.GetChild(2).GetComponent<Light>().intensity + this.gameObject.transform.GetChild(2).GetComponent<Light>().range)/ 2);
            Debug.Log("Intensity: " + lightIntensity.ToString() + "for bulb " + this.name);
            soundIntensity = 10.0f;

        }


        else if (this.tag == "Speaker")
        {
            if(this.name == "SpeakerFixed1")
            {
                Debug.Log("Initializing speaker object 1");
                objectType = "Speaker";
                soundCollider = this.gameObject.transform.GetChild(3).gameObject;

                float intensityValue = Random.Range(80, 130);

                //soundIntensity = 130.0f; //This is the intensity felt at 1cm
                soundIntensity = 115.0f;


            }

            else if (this.name == "SpeakerFixed2")
            {
                Debug.Log("Initializing speaker object 2");
                objectType = "Speaker";
                soundCollider = this.gameObject.transform.GetChild(3).gameObject;

                float intensityValue = Random.Range(80, 130);

                //soundIntensity = 130.0f; //This is the intensity felt at 1cm
                soundIntensity = 100.0f;
            }

            else if (this.name == "SpeakerFixed3")
            {
                Debug.Log("Initializing speaker object 3");
                objectType = "Speaker";
                soundCollider = this.gameObject.transform.GetChild(3).gameObject;

                float intensityValue = Random.Range(80, 130);

                //soundIntensity = 130.0f; //This is the intensity felt at 1cm
                soundIntensity = 115.0f;
            }

            else
            {
                //Debug.Log("Initializing speaker object");
                objectType = "Speaker";
                soundCollider = this.gameObject.transform.GetChild(3).gameObject;

                float intensityValue = Random.Range(80, 130);

                //soundIntensity = 130.0f; //This is the intensity felt at 1cm
                soundIntensity = 120.0f;
            }



            temperature.setCelsius(25);
            temperature.setDispersion(15);


        }
        else
        {
            Debug.Log("HELP ME!!");
        }
    }

    public float getLightIntensity()
    {
        return lightIntensity;
    }

    public GameObject getTemperatureCollider()
    {
        return temperatureCollider;
    }
    public void setTemperatureCollider(float radius)
    {
        this.temperatureCollider.transform.localScale = new Vector3(radius,radius,radius);
    }
    public void setSoundCollider(float radius)
    {
        this.soundCollider.transform.localScale = new Vector3(radius, radius, radius);
    }
    public float getSoundIntensity()
    {
        return soundIntensity;
    }

    public Temperature getTemperature()
    {
        return temperature;
    }
    public void setTemperature(Temperature temp)
    {
        this.temperature = temp;
    }

    public float getDistance()
    {
        return distanceToAvatar;
    }

    public void setDistance(float distance)
    {
        this.distanceToAvatar = distance;
    }

    public string getObjectType()
    {
        return objectType;
    }





}
