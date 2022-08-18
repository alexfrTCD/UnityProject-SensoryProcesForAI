using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ObjectClass;

public class AvatarController : MonoBehaviour
{
    private List<GameObject> temperaturesAffecting = new List<GameObject>();
    private Animator anim;
    private float health = 100;
    private float hunger = 0;
    private float speed = .05f;
    bool experimentOne = true;
    // Start is called before the first frame update
    void Start()
    {

        anim = this.GetComponent<Animator>();
    }

    private void Update()
    {

    }

    public void movement()
    {
        if (experimentOne == true)
        {
            float xDirection = Input.GetAxis("Horizontal");
            float zDirection = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(xDirection, 0.0f, zDirection);

            transform.position += moveDirection * speed;


        }
    }


    public void actualizeDrives(Tuple<bool, float, float> tuple, float temperature, float glare, float sound)
    {
        //First, check if the agent has tried to eat

        float newHealth = this.health;
        float newHunger = this.hunger;

        if (tuple.Item1 == true)
        {
            newHunger += tuple.Item2;
            newHealth += tuple.Item3;

        }
        float temperatureLost = tempCurve(temperature);
        float glareLost = glareCurve(glare);
        float soundLost = soundCurve(sound);
        //Debug.Log("Health lost by temperature: " + Math.Round(temperatureLost,1).ToString());
        //Debug.Log("Health lost by sound: " + Math.Round(soundLost, 1).ToString());
        //Debug.Log("Health lost by glare: " + Math.Round(glareLost, 1).ToString());
        //Debug.Log("Total Health lost: " + Math.Round((temperatureLost + glareLost + soundLost), 1).ToString());
        newHealth -= (float)Math.Round(temperatureLost, 1);
        newHealth -= (float)Math.Round(soundLost, 1);
        newHealth -= (float)Math.Round(glareLost, 1);


        //Debug.Log("New health:" + newHealth);
        //Sound curve
        //self.avatar.health -= self.avatar

        if (this.health == newHealth){ 
            this.health += 1;
        }
        else{

            this.health = newHealth;
        }

        if(this.hunger == newHunger)
        {

            this.hunger += 1;

        }
        else
        {
            this.hunger = newHunger;
        }
            
    }

    public bool checkRestart()
    {
        if(health <= 0)
        {
            health = 100;
            hunger = 0 ;
            return true;
        }
        else if(hunger >= 100)
        {
            hunger = 0;
            health = 100;
            return true;
        }
        else
        {
            return false;
        }
    }


    private float tempCurve(float tempValue)
    {
        float rmin = 45;
        float rmax = 68;

        if (tempValue < rmin)
        {
            return 0;
        }

        else if (tempValue > rmax)
        {
            return 100;
        }

        else
        {   //=  3E-38x21.857
            //y = 3E-38 * tempValue^21.857
            // R² = 0.9332
            float y = (float)(3E-38  * Math.Pow(tempValue, 21.857f));
            //Debug.Log("Final temp curve result of " + tempValue.ToString() + " is: " + y.ToString());
            return y;
        }

    }

    private float glareCurve(float glareValue)
    {
        //y = 5E-21 (glareValue^11.407)
        // R² = 0.9989
        float damage = 0;
        if (glareValue > 60)
        {
            //damage = (float)(5E-21 * Math.Pow(glareValue, 11.407));
            //0.45x - 30.667
            damage = 0.3f * glareValue - 17f;
            //Debug.Log("Final glare curve result of " + glareValue.ToString() + " is: " + damage.ToString());
        }

        return damage;

    }private float soundCurve(float soundValue)
    {
        //Y = 6E-08e^(0.1386 *soundIntensity) 1E-08e^(0.15* soundIntensity)
        //R = 1
        float damage;
        if(soundValue < 70) {
            damage =  0;
        }

        else if(soundValue >= 153.7)
        {
            damage =  100;
        }

        else
        {
            damage = (float)(1E-08 * Math.Exp(0.15 * soundValue));
        }

        return damage;
    }



    public void initPosition(Vector3 initPosition)
    {
        this.transform.position = initPosition;
    }

    public void changeForward(Vector3 forward)
    {
        this.transform.forward = forward;
    }

    //Avatar is static and do an action in the place he is
    public void lookToTheObject(Vector3 objectPosition)
    {
        this.transform.LookAt(objectPosition);
    }
    public void playAnimation(string variable, float threshold)
    {

        anim.SetFloat(variable, threshold, 0.01f, Time.deltaTime);
    }
    //Avatar is moving and rotate while moving
    public void rotateToObject(Vector3 targetPosition, float step)
    {
        this.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(this.transform.forward, targetPosition - this.transform.position, step * 5, 0.0f));
    }
    public void moveToObject(Vector3 targetPosition, float step)
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, targetPosition, step);
    }

    public void setHealth(float health)
    {
        this.health = health;
    }
    public float getHealth()
    {
        return health;
    }

    public void setHunger(float hunger)
    {
        this.hunger = hunger;
    }
    public float getHunger()
    {
        return hunger;
    }


}
