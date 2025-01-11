using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Diamond : Item
{
    SpriteRenderer pulseSprite;
    bool cantInteract = false;
    public override void Start()
    {
        base.Start();
        pulseSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();   
    }

    public override void Interact()
    {
        GameManager.instance.diamondPickedUp = true;
        if(cantInteract) return;
        base.Interact();
        cantInteract = true;
        GetComponent<SpriteRenderer>().enabled = false;
        
        LeanTween.scale(pulseSprite.gameObject,Vector3.one * 8f, 1f).setEaseOutExpo();
        LeanTween.alpha(pulseSprite.gameObject,0f, 0.2f).setEaseOutExpo().setDelay(0.4f);

        StartCoroutine(DelayTilt());

        Destroy(gameObject,1f);
    }

    IEnumerator DelayTilt(){
        yield return new WaitForSeconds(0.5f);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            //if(Vector2.Distance((Vector2)enemy.transform.position, (Vector2)transform.position) <= 8f){
                GameManager.instance.source.Play();
                enemy.transform.GetComponent<EnemyController>().playerSpotted = true;
            //}
        }
    }
}
