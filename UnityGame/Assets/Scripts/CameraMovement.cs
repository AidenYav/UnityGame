using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;

    private float timeToLockOn; //Once the player stops moving, this is how long it will take the camera to center the player onto the screen

    public Vector2 initialVel, accel;

    private IEnumerator currentMove;
    // Start is called before the first frame update
    void Start()
    {
        timeToLockOn = 0.75f;

    }

    // Update is called once per frame
    void Update()
    {
        goToTarget((Vector2)player.transform.position);
    }

    public Vector2 getDistanceFromTarget(Vector2 v){
        return v - (Vector2) this.gameObject.transform.position;
    }

    public void goToTarget(Vector2 v){
        Vector2 vel = getDistanceFromTarget(v) * 2 / timeToLockOn;
        Vector2 accel = -vel;
        if (currentMove != null){
            StopCoroutine(currentMove);
        }
        StartCoroutine(currentMove = move(vel, accel));

    }

    private IEnumerator move(Vector2 vel, Vector2 accel){
        initialVel = vel;
        this.accel = accel;
        //While velocity is not 0
        //velocity approaches 0
        //Meaning Accel + --> vel -
        //Meaning Accel - --> vel +
        //If accel is positive, multi = 1
        //If accel is negative, multi = -1
        float xNeg = accel.x/Mathf.Abs(accel.x);
        float yNeg = accel.y/Mathf.Abs(accel.y);
        while ( (xNeg*vel.x < 0 || yNeg*vel.y < 0) && vel.magnitude > 0.01) //Accel is positive and the magnitude of movement is meaningful
        { 
            //This moves the camera
            transform.Translate(vel * Time.deltaTime);
            //This changes the velocity of the camera moving
            vel -= accel * Time.deltaTime;
            

            //This means that the camera is already in the desired position
            if (xNeg*vel.x > 0){
                vel.x = 0;
                accel.x = 0;
            }
            if (yNeg*vel.y > 0){
                vel.y = 0;
                accel.y = 0;
            }
            yield return null;
        }
        StopCoroutine(currentMove);
    }

    
}
