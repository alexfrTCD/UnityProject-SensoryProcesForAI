using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(SensesControl))]
public class FieldOfViewEditor : Editor
{
    // Start is called before the first frame update
    private void OnSceneGUI()
    {
        SensesControl fov = (SensesControl)target;

        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.GetChild(0).transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.GetChild(0).transform.eulerAngles.y, fov.angle / 2);

        //Pitagoras theorem to optimize overlapBox to it's maxim:
        //square(hipothenuse) = square(a) + square(b)
        //hipotenusa = radius
        //a = fov.transform.position.z - (fov.transform.position + viewAngle02 * fov.radius).z. Use the absolute just in case the result is negative
        float sizeZ = Mathf.Abs(fov.raycastPosition.z - (fov.raycastPosition + viewAngle02 * fov.radius).z);
        //now resolving z by pitagoras theorem: b = squareRoot(square(hipootenuse) - square(a)
        float sizeX = Mathf.Sqrt(Mathf.Pow(fov.radius, 2) - Mathf.Pow(sizeZ, 2));

        Handles.color = Color.red;
        Handles.DrawWireDisc(fov.raycastPosition, new Vector3(0, 1, 0), fov.radius);

        Handles.color = Color.blue;
        Handles.DrawLine(fov.raycastPosition, fov.raycastPosition + viewAngle01 * fov.radius);
        Handles.DrawLine(fov.raycastPosition, fov.raycastPosition + viewAngle02 * fov.radius);


        foreach (ObjectClass go in fov.getSightList())
        {
            if(go!= null)
            {
                Handles.color = Color.green;
                Handles.DrawLine(fov.raycastPosition, go.gameObject.transform.position);
            }

        }
        foreach (GameObject go in fov.getListNotViewed())
        {
            Handles.color = Color.red;
            Handles.DrawLine(fov.raycastPosition, go.gameObject.transform.position);
        }


    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}


