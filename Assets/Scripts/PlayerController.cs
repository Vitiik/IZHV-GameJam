using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public float movementSpeed;
    public bool lockControls = false;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!lockControls){
            var ver = Input.GetAxis("Vertical");
            var hor = Input.GetAxis("Horizontal");

            rb.velocity += (Vector2.up * movementSpeed * ver + Vector2.right * movementSpeed * hor) * Time.deltaTime;
        }
    }
}
