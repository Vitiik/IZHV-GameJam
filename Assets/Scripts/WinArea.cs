using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinArea : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D col){
        if(col.transform.tag == "Player"){
            GameManager.instance.Win();
        }
    }
}
