using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    bool opened = false;
    public bool openRight = true;
    Vector2 startPos;
    void Start()
    {
        startPos = transform.position;
    }

    public void Toogle(){
        opened = !opened;
        if(opened){
            //Open
            LeanTween.move(gameObject,(Vector3)startPos + transform.right * transform.localScale.x * (openRight ? 1 : -1),0.5f).setEaseOutCubic();
        }else{
            //Close
            LeanTween.move(gameObject,(Vector3)startPos,0.5f).setEaseOutCubic();
        }
    }
}
