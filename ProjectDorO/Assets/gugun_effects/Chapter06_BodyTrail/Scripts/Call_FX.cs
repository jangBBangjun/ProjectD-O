using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class Call_FX : MonoBehaviour

{

    public Object effect;

    public Vector3 Position;

    public Vector3 Rotation;

    public GameObject Player;

    public float Destroytime;



    private void FX(Object pref)

        //Name of Event Function

    {

        GameObject childFX = Instantiate(effect) as GameObject;

        childFX.transform.SetParent(Player.transform, false);



        childFX.transform.localPosition += Position;

        childFX.transform.localRotation = Quaternion.Euler(Rotation);

        

        Destroy(childFX, Destroytime);

    }

 

}