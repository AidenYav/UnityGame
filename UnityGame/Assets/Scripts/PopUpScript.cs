using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpScript : MonoBehaviour
{

    public SpriteRenderer tutorialText, shadow;

    public float shadowAlpha = 0.8f;

    public float popUpSpeed = -1;
    [SerializeField] private int numOfObjectsInside = 0;

    private IEnumerator popUpAnimation;

    private Color textColor, shadowColor;

    // Start is called before the first frame update
    void Start()
    {
        if (popUpSpeed <= 0){
            popUpSpeed = 0.5f;
        }
        textColor = tutorialText.color;
        shadowColor = shadow.color;
        textColor.a  = 0;
        shadowColor.a = 0;
        tutorialText.color = textColor;
        shadow.color = shadowColor;
    }


    public void OnTriggerEnter2D(Collider2D other){

        if(other.gameObject.tag == "Player"){
            numOfObjectsInside++;
        }
        if(numOfObjectsInside == 1){
            UpdatePopUp();
        }
        
    }

    public void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.tag == "Player"){
            numOfObjectsInside--;
        }
        if(numOfObjectsInside == 0){
            UpdatePopUp();
        }
    }

    public void UpdatePopUp(){
        if (popUpAnimation != null){
            StopCoroutine(popUpAnimation);
        }
        StartCoroutine(popUpAnimation = PopUpAnimation());
    }

    public IEnumerator PopUpAnimation(){
        
        bool popUp = numOfObjectsInside > 0;
        float timePassed = 0;

        while (timePassed < popUpSpeed){

        
            timePassed += Time.deltaTime;
            float rate = timePassed / popUpSpeed;

            if (!popUp){
                rate = 1-rate;
            }
            
            float val =  Mathf.Pow(rate , 2 );
            textColor.a  = val;
            shadowColor.a = shadowAlpha * val;

            tutorialText.color = textColor;
            shadow.color = shadowColor;

            yield return null;
        }

        if (popUp){
            textColor.a  = 1;
            shadowColor.a = shadowAlpha;
        }else{
            textColor.a  = 0;
            shadowColor.a = 0;
        }
        tutorialText.color = textColor;
        shadow.color = shadowColor;
    }


}
