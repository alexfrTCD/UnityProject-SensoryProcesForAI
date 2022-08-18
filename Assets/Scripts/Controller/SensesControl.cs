using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using static ObjectClass;

public class SensesControl : MonoBehaviour
{

    private List<ObjectClass> sightList = new List<ObjectClass>();
    private List<ObjectClass> touchList = new List<ObjectClass>();
    private List<ObjectClass> hearingList = new List<ObjectClass>();


    private List<GameObject> sightListDiscarted = new List<GameObject>();
    
    //Half Distance to the furthest point I can see
    public float radius = 3 ;

    [Range(0, 360)]
    public float angle = 60;

    public LayerMask defaultLayer;

    public Vector3 raycastPosition;

    private float temperatureFelt = 0;
    private float glareFelt = 0;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject[] fireTag = GameObject.FindGameObjectsWithTag("Fire");
        //for (int i = 0; i < fireTag.Length; i++)
        //{
        //    touchList.Add(fireTag[i]);
        //    Debug.Log("Object added in touch List: " + fireTag[i].name);
        //}

        //GameObject[] speakerObjectsTag = GameObject.FindGameObjectsWithTag("Speaker");
        //for (int i = 0; i < speakerObjectsTag.Length; i++)
        //{
        //    hearingList.Add(speakerObjectsTag[i]);
        //    Debug.Log("Object added in hearingList List: " + speakerObjectsTag[i].name);
        //}
    }
    private void Update()
    {

    }


    public void viewField()
    {
        //Getting camera angle
        Vector3 viewAngle01 = DirectionFromAngle(this.transform.GetChild(0).transform.eulerAngles.y, this.angle / 2);
        raycastPosition = new Vector3(this.transform.position.x, 0.0f, this.transform.position.z);
        

        ////Pitagoras theorem to optimize overlapBox to it's maxim:
        ////square(hipothenuse) = square(a) + square(b)
        ////hipotenusa = radius
        ////a = fov.transform.position.z - (fov.transform.position + viewAngle02 * fov.radius).z. Use the absolute just in case the result is negative
        //float sizeZ = Mathf.Abs(raycastPosition.z - (raycastPosition + viewAngle01 * this.radius).z);
        ////now resolving z by pitagoras theorem: b = squareRoot(square(hipootenuse) - square(a)
        //float sizeX = Mathf.Sqrt(Mathf.Pow(this.radius, 2) - Mathf.Pow(sizeZ, 2));



        //First - view sense
        sightList.Clear();
        sightListDiscarted.Clear();
        glareFelt = 0;

        //This should be limited for the number of objects that a human can focus. 

        //raycastPosition + (this.transform.forward * radius/2) this line is an optimization. With this line, we do the overlapBox function just for the part in front of the player. 
        //We move the raycast position in front of the player only the half of the radius to be able to calculate everything.
        Collider[] rangeChecks = Physics.OverlapSphere(raycastPosition, radius , defaultLayer);

        //Debug.Log("Inside viewField are: " + rangeChecks.Length.ToString() + " objects");
        if (rangeChecks.Length != 0)
        {
            
            foreach (Collider c in rangeChecks)
            {

                //Debug.Log("Collider detected:" + c.name + " in position: " + c.transform.position.x.ToString() + "," + c.transform.position.y.ToString() + "," + c.transform.position.z.ToString());
                if (c.gameObject.name != "Board" && c.gameObject.transform.parent == null && c.gameObject.name != "Avatar")
                {
                    //Debug.Log("Collider detected, second if:" + c.name + " in position: " + c.transform.position.x.ToString() + "," + c.transform.position.y.ToString() + "," + c.transform.position.z.ToString());


                    Transform target = c.transform;
                    Vector3 directionToTarget = (new Vector3(target.position.x, 0, target.position.z) - raycastPosition).normalized;
                    //Debug.Log("Angle with avatar: " + Vector3.Angle(transform.forward, directionToTarget).ToString());

                    //If inside field of view
                    //It is also checked if is one of the three objects that is just in front of me
                    if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2
                        || new Vector3(this.transform.position.x, 0.0f, this.transform.position.z) + new Vector3(1.0f, 0.0f, 0.0f) == new Vector3(c.gameObject.transform.position.x, 0.0f, c.gameObject.transform.position.z))

                    {

                        float distanceToTarget = Vector3.Distance(raycastPosition, new Vector3(target.position.x, 0, target.position.z));
                        //Debug.Log("Distance To target = " + c.gameObject.name + " " + distanceToTarget.ToString());

                        float distanceBeforeTarget = distanceToTarget - 1.0f;
                        //Debug.Log("Distance Before Target = " + c.gameObject.name + " " + distanceBeforeTarget.ToString());


                        //I'm using default layer all the time because every object can block another object.   
                        if (!Physics.Raycast(raycastPosition, directionToTarget, out RaycastHit hit, distanceBeforeTarget, defaultLayer))
                        {
                            float glare = glareFelt;

                            if (c.gameObject.GetComponent<ObjectClass>().getLightIntensity() * 1.5f != 0)
                            {
                                float theta = Vector3.Angle(new Vector3(transform.forward.x, 0, transform.forward.z), directionToTarget);
                                //Debug.Log("Theta is: " + theta.ToString());
                                if (theta >= 0 && theta < 60)
                                {

                                    glare = (c.gameObject.GetComponent<ObjectClass>().getLightIntensity() / (distanceToTarget * 115)) * (float)Math.Cos(ConvertToRadians(theta));


                                }
                            }
                            


                            //if (c.gameObject.tag == "Bulb")
                            //{
                            //    Debug.Log("Object: " + c.gameObject.name + " glare= " + glare + " at distance: " + distance.ToString());
                            //}
                            //c.gameObject.GetComponent<ObjectClass>().setDistance(distanceToTarget);


                            if (glare > glareFelt)
                            {

                                glareFelt = glare;
                            }

                            //Debug.Log("Collision position " + hit.transform.position + " distance " + hit.distance);
                            sightList.Add(c.gameObject.GetComponent<ObjectClass>());

                        }
                        else
                        {
                            sightListDiscarted.Add(c.gameObject);
                        }



                    }
                }


            }

            //With this, the agent can't be trained

            //foreach (GameObject go in sightListDiscarted)
            //{
            //    Debug.Log("Inside while sight discarted list. GameObject = " + go.name);
            //    bool found = false;
            //    int count = 0;
            //    List<Vector3> boxVertices = new List<Vector3>();
            //    BoxCollider col = go.GetComponent<BoxCollider>();
            //    P000 to P011 are the 4 corners of the left face and P100 to P111 the corners of the right face.
            //    P000 is the left, bottom, back point as seen in local coordinates.
            //    P101 is the right, bottom, front point.
            //    var trans = col.transform;
            //    var min = col.center - col.size * 0.5f;
            //    var max = col.center + col.size * 0.5f;
            //    boxVertices.Add(trans.TransformPoint(new Vector3(min.x, min.y, min.z)));
            //    boxVertices.Add(trans.TransformPoint(new Vector3(min.x, min.y, max.z)));
            //    boxVertices.Add(trans.TransformPoint(new Vector3(min.x, max.y, min.z)));
            //    boxVertices.Add(trans.TransformPoint(new Vector3(min.x, max.y, max.z)));
            //    boxVertices.Add(trans.TransformPoint(new Vector3(max.x, min.y, min.z)));
            //    boxVertices.Add(trans.TransformPoint(new Vector3(max.x, min.y, max.z)));
            //    boxVertices.Add(trans.TransformPoint(new Vector3(max.x, max.y, min.z)));
            //    boxVertices.Add(trans.TransformPoint(new Vector3(max.x, max.y, max.z)));
            //    while (!found && count < boxVertices.Count)
            //    {
            //        int counter = 0; //We need 3 vertices seen to add the object as viewed
            //        Vector3 target = boxVertices[count];
            //        Vector3 directionToTarget = (target - raycastPosition).normalized;

            //        Debug.Log(" Vertex " + count.ToString() + " Position: " + target);

            //        float distanceToTarget = Vector3.Distance(raycastPosition, target);
            //        float distanceBeforeTarget = distanceToTarget - 1.0f;
            //        Debug.Log("Distance Before Target = " + c.gameObject.name + " " + distanceBeforeTarget.ToString());

            //        I'm using default layer all the time because every object can block another object.   
            //        if (!Physics.Raycast(raycastPosition, directionToTarget, out RaycastHit hit, distanceBeforeTarget, defaultLayer))
            //        {
            //            counter++;
            //            if (counter > 4)
            //            {

            //                Debug.Log("Object Added in third option: " + go.name);
            //                sightList.Add(go);
            //                sightListDiscarted.Remove(go);
            //                found = true;
            //            }
            //        }
            //        else
            //        {
            //            count++;
            //        }

            //    }
            //}
        }

        //if (sightList.Count == 0)
        //{
        //    Debug.Log("Nothing inside my view");
        //}
        //else
        //{

        //    foreach (GameObject go in sightList)
        //    {
        //        Debug.Log("Position object view " + go.gameObject.name + " = " + go.transform.position.x.ToString() + go.transform.position.z.ToString());
        //    }
        //}

    }
       


    public float calculateTemperature(float ambientTemp)
    {
        //Debug.Log("TouchList " +  touchList.ToString());
        if(touchList.Count == 0)
        {
            return ambientTemp;
        }
        float maximumTemp = ambientTemp;
        float Tf = ambientTemp;
        if (touchList.Count > 0)
        {
            //Getting maximum temperature being felt
            for (int i = 0; i < touchList.Count; i++)
            {
                //Debug.Log("Object scanning: " + touchList[i].name);
                Temperature temperature = touchList[i].GetComponent<ObjectClass>().getTemperature();
                //Radiation temperature
                Vector3 gameObjectPosition = touchList[i].gameObject.transform.position;
                Vector3 avatarPosition = this.gameObject.transform.position;

                //float distance = Math.Abs(gameObjectPosition.x - avatarPosition.x) + Math.Abs(gameObjectPosition.z - avatarPosition.z);
                float distance = Vector3.Distance(new Vector3(avatarPosition.x, 0, avatarPosition.z), new Vector3(gameObjectPosition.x, 0, gameObjectPosition.z));
                float Tr = temperature.getCelsius() - (distance * temperature.getDispersion());

                //if(gameObject.tag == "Fire")
                //{
                //    Debug.Log("Element: " + touchList[i].name + "Temperature felt = " + Tr.ToString() +" at distance " + distance.ToString());
                //}

                if (Tr > maximumTemp)
                {

                    maximumTemp = Tr;
                }
                
            }

        }


        return maximumTemp;
        


    }

    public float calculateSound()
    {
        List<GameObject> hearingList = GameObject.Find("Board").GetComponent<WorldClass>().speakers;
        //Debug.Log("Hearing list: " + hearingList.ToString());
        if (hearingList.Count == 0)
        {
            Debug.Log("Hearing list is empty");
            return 0;
        }
        float maximumSound = 0;
        if (hearingList.Count > 0)
        {
            //Getting maximum temperature being felt
            for (int i = 0; i < hearingList.Count; i++)
            {

                //Sound intensity heared by the avatar

                Vector3 gameObjectPosition = hearingList[i].gameObject.transform.position;
                Vector3 avatarPosition = this.gameObject.transform.position;

                //float distance = Math.Abs(gameObjectPosition.x - avatarPosition.x) + Math.Abs(gameObjectPosition.z - avatarPosition.z);
                float distance = Vector3.Distance(new Vector3(avatarPosition.x, 0, avatarPosition.z), new Vector3(gameObjectPosition.x, 0, gameObjectPosition.z));
                //Debug.Log("Sound list element: " + hearingList[i].name + "Sound intensity: " + hearingList[i].GetComponent<ObjectClass>().getSoundIntensity() + "at distance: " + distance.ToString());
                //Debug.Log("Distance calculated now: " + distance);
                float intensityReduction = (float)(20 * Math.Log10(distance / 0.5));
                float intensityFelt = hearingList[i].GetComponent<ObjectClass>().getSoundIntensity() - intensityReduction;
                //Debug.Log("Sound felt = " + intensityFelt.ToString() +" at distance " + hearingList[i].GetComponent<ObjectClass>().getDistance().ToString());

                if (intensityFelt > maximumSound)
                {

                    maximumSound = intensityFelt;
                }

            }

        }


        return maximumSound;



    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("OnTrigger enter");
        //Debug.Log(" collider name: " + other.name);
        if (other.gameObject.name == "Temperature")
        {
            touchList.Add(other.gameObject.transform.parent.gameObject.GetComponent<ObjectClass>());
        }

        else if (other.gameObject.name == "Sound")
        {
            Debug.Log("Adding object to hearing list");
            hearingList.Add(other.gameObject.transform.parent.gameObject.GetComponent<ObjectClass>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("OnTrigger exit");
        //Debug.Log("Temperature collider name: " + other.name);
        if (other.gameObject.name == "Temperature")
        {
            touchList.Remove(other.gameObject.transform.parent.gameObject.GetComponent<ObjectClass>());
        }

        else if (other.gameObject.name == "Sound")
        {
            Debug.Log("Rmoving object to hearing list");
            hearingList.Remove(other.gameObject.transform.parent.gameObject.GetComponent<ObjectClass>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //if (other.gameObject.name == "Temperature")
        //{
        //    //if (other.gameObject.transform.parent.gameObject.GetComponent<ObjectClass>() is not null)
        //    //{
        //    //    if (!touchList.Contains(other.gameObject))
        //    //    {
        //    //        touchList.Add(other.gameObject.transform.parent.gameObject);
        //    //    }

        //    //}
        //    for (int i = 0; i < touchList.Count; i++)
        //    {
        //        Vector3 gameObjectPosition = touchList[i].gameObject.transform.position;
        //        Vector3 avatarPosition = this.gameObject.transform.position;

        //        float distance = Math.Abs(gameObjectPosition.x - avatarPosition.x) + Math.Abs(gameObjectPosition.z - avatarPosition.z);
        //        float distanceToTarget = Vector3.Distance(new Vector3(this.transform.position.x, 0.0f, this.transform.position.z), new Vector3(touchList[i].gameObject.transform.position.x, 0.0f, touchList[i].gameObject.transform.position.z));
        //        touchList[i].gameObject.GetComponent<ObjectClass>().setDistance(distance);


        //    }

        //}

        //else if (other.gameObject.name == "Sound")
        //{
        //    //if (other.gameObject.transform.parent.gameObject.GetComponent<ObjectClass>() is not null)
        //    //{
        //    //    if (!hearingList.Contains(other.gameObject))
        //    //    {
        //    //        hearingList.Add(other.gameObject.transform.parent.gameObject);
        //    //    }

        //    //}

        //    for (int i = 0; i < hearingList.Count; i++)
        //    {
        //        Vector3 gameObjectPosition = hearingList[i].gameObject.transform.position;
        //        Vector3 avatarPosition = this.gameObject.transform.position;
        //        Vector3 avatarRotation = this.gameObject.transform.forward;

        //        float distance = Math.Abs(gameObjectPosition.x - avatarPosition.x) + Math.Abs(gameObjectPosition.z - avatarPosition.z);
        //        //Debug.Log("Avatar position: " + this.gameObject.transform.position);
        //        float distanceToTarget = Vector3.Distance(new Vector3(this.gameObject.transform.position.x, 0.0f, this.gameObject.transform.position.z), new Vector3(hearingList[i].gameObject.transform.position.x, 0.0f, hearingList[i].gameObject.transform.position.z));
        //        if (distanceToTarget < 0.5f)
        //        {
        //            distanceToTarget = 0.5f;
        //        }
        //        hearingList[i].gameObject.GetComponent<ObjectClass>().setDistance(distance);
        //    }
        //}

    }

    public float getGlare()
    {
        return glareFelt;
    }

    public double ConvertToRadians(double angle)
    {
        return (Math.PI / 180) * angle;
    }
    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public List<ObjectClass> getSightList()
    {
        return sightList;
    }

    public void printSightList()
    {
        if(sightList.Count > 0)
        {
            for (int i = 0; i < sightList.Count; i++)
            {
                Debug.Log("Object founded: " + sightList[i].tag + "in position: " + sightList[i].transform.position.x.ToString() + " " + sightList[i].transform.position.y.ToString() + "," + sightList[i].transform.position.z.ToString());
            }
        }

        else
        {
            Debug.Log("Sight list empty");
        }

    }

    public void setSightList(List<ObjectClass> sightList)
    {
        this.sightList = sightList;
    }

    public List<ObjectClass> getHearingList()
    {
        return hearingList;
    }

    public void setHearingList(List<ObjectClass> hearingList)
    {
        this.hearingList = hearingList;
    }

    public List<ObjectClass> getTouchList()
    {
        return touchList;
    }

    public void setTouchList(List<ObjectClass> touchList)
    {
        this.touchList = touchList;
    }

    public List<GameObject> getListNotViewed()
    {
        return sightListDiscarted;
    }
}
