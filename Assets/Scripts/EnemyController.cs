using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public enum EnemeyMovementType{
    pingPong,
    continuously
}

public class EnemyController : MonoBehaviour
{
    public SplineContainer spline;
    public EnemeyMovementType movementType = EnemeyMovementType.pingPong;
    public float splineMovementSpeed;
    public float catchMovementSpeed;
    public float spotDistance;
    Rigidbody2D rb;
    NavMeshAgent agent;

    public float currentDistanceOnSpline;
    float t;
    float splineLength;

    bool flipDirection;
    [HideInInspector]
    public bool playerSpotted;
    float touchingPlayerTimer = 0f;
    bool touchingPlayer;

    float currentLerpTime = 0f;

    GameObject player;

    public float rotationSpeed = 50f;
    float zAngleVelocity = 0.0f;

    [Header("Sounds")]
    public List<AudioClip> spotSounds = new List<AudioClip>();
    public AudioClip playerHurtSound;

    AudioSource source;

    void Start()
    {   
        source = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
		agent.updateUpAxis = false;
        splineLength = spline.CalculateLength();
    }

    void Update(){
        if(!playerSpotted){
            int angle = 15;
            for (int i = 0; i < angle; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position,Quaternion.AngleAxis(-30 + i*4, Vector3.forward) * transform.up,spotDistance,LayerMask.GetMask("Player","Walls"));
                Debug.DrawLine(transform.position,transform.position + Quaternion.AngleAxis(-30 + i*4, Vector3.forward) * transform.up * spotDistance);
                if(hit){
                    if(hit.transform.tag == "Player"){
                        playerSpotted = true;
                        GameManager.instance.source.Play();
                        source.PlayOneShot(spotSounds[UnityEngine.Random.Range(0,spotSounds.Count)]);
                        break;
                    }

                }
            }
        }
        
        if(!playerSpotted){
            FollowPath();
        }else{
            CatchPlayer();
        }

        if(touchingPlayer && GameManager.instance.hitTaken < 3){
            touchingPlayerTimer += Time.deltaTime;
            if(touchingPlayerTimer >= 1f){
                HitPlayer();
                touchingPlayerTimer = 0f;
            }
        }
    }

    void CatchPlayer(){
        agent.SetDestination(player.transform.position);
        
        Vector3 dir = (player.transform.position - transform.position).normalized;
        transform.up = new Vector3(dir.x, dir.y, 0f);
        /*
        float catchSpeed = Mathf.Lerp(0.02f, catchMovementSpeed, currentLerpTime / 2f);
        currentLerpTime += Time.deltaTime;
        rb.MovePosition(rb.transform.position + transform.up * catchSpeed);*/
    }

    void FollowPath()
    {
        currentDistanceOnSpline = Mathf.Clamp(currentDistanceOnSpline,0,splineLength);
        t = currentDistanceOnSpline / splineLength;

        if(movementType == EnemeyMovementType.pingPong){
            if(t >= 1 && !flipDirection){
                flipDirection = true;
            }
            if(t <= 0 && flipDirection){
                flipDirection = false;
            }
        }
        if(movementType == EnemeyMovementType.continuously){
            if(t >= 1 && !flipDirection){
                currentDistanceOnSpline = 0f;
            }
            if(t <= 0 && flipDirection){
                currentDistanceOnSpline = splineLength;
            }
        }
        

        float3 pos, up, tangent;
        spline.Evaluate(t,out pos,out tangent,out up);


        transform.position = pos;
        
        Vector3 forward = tangent;
        forward.Normalize();

        currentDistanceOnSpline += splineMovementSpeed * Time.deltaTime * (flipDirection ? -1 : 1);

        Quaternion targetRotation =  Quaternion.LookRotation(forward, up) * Quaternion.Inverse(Quaternion.LookRotation(Vector3.up * (flipDirection ? -1 : 1), Vector3.forward));
        float zAngle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetRotation.eulerAngles.z, ref zAngleVelocity, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f,targetRotation.eulerAngles.y,zAngle);
        //rb.MovePosition(rb.transform.position + Vector3.up * movementSpeed * ver + Vector3.right * movementSpeed * hor);
    }

    public void HitPlayer(){
        if(!playerSpotted){
            playerSpotted = true;
            GameManager.instance.source.Play();
        } 
        source.PlayOneShot(playerHurtSound);
        GameManager.instance.PlayerTakeHit();
        agent.isStopped = true;
        if(GameManager.instance.hitTaken < 3){
            StartCoroutine(AgentContinue());
        }
        rb.AddForce(-transform.right*100f);
        player.transform.GetComponent<Rigidbody2D>().AddRelativeForce((player.transform.position - transform.position).normalized * 1000f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player"){
            HitPlayer();
            touchingPlayerTimer = 0f;
            touchingPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player"){
            touchingPlayer = false;
        }
    }

    IEnumerator AgentContinue(){
        yield return new WaitForSeconds(0.5f);
        agent.isStopped = false;
    }
}
