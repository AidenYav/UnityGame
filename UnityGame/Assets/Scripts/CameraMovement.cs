using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;

    private float timeToLockOn; //Once the player stops moving, this is how long it will take the camera to center the player onto the screen

    public Vector2 initialVel, accel;

    private float zoomDistance = 5;


    private IEnumerator currentMove, currentZoom;
    private GameObject target;
    public GameObject multiplayerDivider, Camera2;
    // Start is called before the first frame update
    void Start()
    {
        timeToLockOn = 0.75f;
        setTargetPlayer();
    }

    public void setTargetPlayer(){
        target = player;
    }

    public void setTarget(GameObject newTarget){
        target = newTarget;
    }


    // Update is called once per frame
    void Update()
    {
        goToTarget((Vector2)target.transform.position);
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

    public void zoomToTarget(float targetDist){
        if (multiplayerDivider == null){
            Debug.LogWarning("Cannot call zoom for this camera object!");
        }
        if (currentZoom != null){
            StopCoroutine(currentZoom);
        }
        StartCoroutine(currentZoom = zoom( (double) targetDist));

    }

    public void resetCameraZoom(){
        zoomToTarget(zoomDistance);
    }


    public IEnumerator zoom(double targetDist){
        Camera cam = gameObject.GetComponent<Camera>();
        Camera cam2 = Camera2.GetComponent<Camera>();
        double initialZoom = cam.orthographicSize;
        double zoomDist = targetDist - initialZoom;
        double timePassed = 0;

        //Is the camera zooming in?
        bool zoomIn = zoomDist <= 0;
        multiplayerDivider.SetActive(zoomIn);
        
        
        
        while(timePassed < timeToLockOn){
           
            timePassed += Time.deltaTime;

            float rate = (float) timePassed / timeToLockOn;

            float val = Mathf.Sqrt(1 - Mathf.Pow(rate - 1 , 2 ));
            
            if (zoomIn){
                cam.rect = new Rect(0,0,1.0f - 0.5f*val,1);
                cam2.rect = new Rect(1.0f - 0.5f*val, 0, 1.0f - 0.5f*val, 1);
            }else{
                cam.rect = new Rect(0,0,0.5f + 0.5f*val,1);
                cam2.rect = new Rect(0.5f + 0.5f*val, 0, 0.5f + 0.5f*val, 1);
            }
            
            cam.orthographicSize = (float) initialZoom + (float) zoomDist*val;
            yield return null;
        }

        cam.orthographicSize = (float) targetDist;
    }
}
