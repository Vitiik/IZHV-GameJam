using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Key : Item
{
    public List<Door> doors;
    bool cantInteract = false;

    public override void Start()
    {
        base.Start();
    }

    public override void Interact()
    {
        if(cantInteract) return;
        base.Interact();
        cantInteract = true;
        GetComponent<Light2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        Destroy(this.gameObject,1f);
        foreach (var door in doors)
        {
            door.Toogle();
        }
    }
}
