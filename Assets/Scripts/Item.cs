using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Vector2 startSize;
    public AudioClip interactSound;
    AudioSource source;

    public virtual void Start(){
        source = GetComponent<AudioSource>();
        startSize = transform.localScale;
        StartCoroutine(Bobbing());
    }

    IEnumerator Bobbing(){
        LeanTween.scale(gameObject,startSize + Vector2.one*0.02f,0.5f).setEaseOutCubic();
        yield return new WaitForSeconds(0.5f);
        LeanTween.scale(gameObject,startSize,0.5f).setEaseOutCubic();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Bobbing());
    }

    void Update(){

    }

    public virtual void Interact(){
        if(interactSound != null){
            source.PlayOneShot(interactSound);
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col.transform.tag == "Player"){
            Interact();
        }
    }
}
