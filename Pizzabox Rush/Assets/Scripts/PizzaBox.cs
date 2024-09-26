using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PizzaBox : MonoBehaviour
{
    public delegate void SaveWorldAction(int objID, Transform trans);
    public static event SaveWorldAction OnSaveWorld;

    public delegate void ResetWorldAction(int objID, Transform trans);
    public static event ResetWorldAction OnResetWorld;

    public delegate void ResetVariables();
    public static event ResetVariables OnResetVariables;

    [SerializeField] private LayerMask TrackLayer;
    [SerializeField] private float HorizontalDist = 2.7f, 
        JumpPower = 15f, DashFallPower = 30f, LerpSpeed = 20f, 
        TravelSpeed = 15f, AddOneVelocityAfterSceconds = 10f, DistanceThreshold = 200f;

    [SerializeField] private MeshRenderer[] meshRenderers;

    [SerializeField] private Text distance;

    [SerializeField] private float unitOfDistance = 10f;

    private Rigidbody pizzaBoxRB;

    public float currDist;

    float verticalUpAxis, verticalDownAxis, currPosX = 0f, startSpeed;

    float deltaDist, prevDist;

    private void Awake()
    {
        pizzaBoxRB = GetComponent<Rigidbody>();
        startSpeed = TravelSpeed;

        pizzaBoxRB.constraints = RigidbodyConstraints.FreezeRotation;

        GameManager.Instance.OnGameStart += Setup;
    }

    private void Setup()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].enabled = true;
            meshRenderers[i].gameObject.layer = 0;
        }
    }

    private void Update()
    {
        deltaDist = transform.position.z - prevDist;

        if (GameManager.Instance.HasGameStarted)
        {
            currDist += ((unitOfDistance * deltaDist) / 100f);
            
            distance.text = (currDist > 0f) ? ((int)currDist).ToString() + " m" : "0 m";
        }
        
        //Maps the arrow keys input to respectfully decrement and increment 'currPosX' with the value 'HorizontalDist'
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) currPosX -= HorizontalDist;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) currPosX += HorizontalDist;

        //Ternary operation to assign a non zero value when the corresponding input keys are pressed
        verticalUpAxis = (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) ? 1f : 0f;
        verticalDownAxis = (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) ? -1f : 0f;

        Dodge();
        Travel();

        Jump(verticalUpAxis);
        DashFall(verticalDownAxis);

        ResetWorldTransform();

        prevDist = transform.position.z;
    }

    private void ResetWorldTransform()
    {
        if (transform.position.z >= DistanceThreshold)
        {
            //Save all relative distance of all other objects to PizzaBox
            if (OnSaveWorld != null)
                OnSaveWorld(this.GetInstanceID(), transform);

            //Reset PizzaBox's Position
            transform.position = new Vector3(transform.position.x, 0f, 0f);

            //Reset all other objects while keeping their relative distances intact
            if (OnResetWorld != null)
                OnResetWorld(this.GetInstanceID(), transform);

            //Reset variables used in keeping track
            if (OnResetVariables != null)
                OnResetVariables();
        }
    }

    void Travel()
    {
        //Adds 1 velocity to 'TravelSpeed' after set seconds has elasped
        float velPerSec = Time.time / AddOneVelocityAfterSceconds;
        TravelSpeed = (GameManager.Instance.HasGameStarted) ? startSpeed + velPerSec : startSpeed;
        pizzaBoxRB.velocity = new Vector3(pizzaBoxRB.velocity.x, pizzaBoxRB.velocity.y, TravelSpeed);
    }

    void Dodge()
    {
        //Clamps value of 'currPosX' between range '-HorizontalDist' and 'HorizontalDist'
        if (currPosX < -HorizontalDist) currPosX = -HorizontalDist;
        if (currPosX > HorizontalDist) currPosX = HorizontalDist;

        float newX = Mathf.MoveTowards (pizzaBoxRB.position.x, currPosX, Time.deltaTime * LerpSpeed);
        
        //Moves player's position on x-axis to 'newX' 
        pizzaBoxRB.MovePosition(new Vector3(newX, pizzaBoxRB.position.y, pizzaBoxRB.position.z));
    }

    void Jump(float input)
    {
        if (!IsGrounded())
            return;

        pizzaBoxRB.AddForce(Vector3.up * input * JumpPower, ForceMode.Impulse);
    }

    void DashFall(float input)
    { 
        pizzaBoxRB.AddForce(Vector3.up * input * DashFallPower, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        //projects a box from player's position to check for collision with 'TrackLayer'
        Vector3 groundedBoxPos = transform.position + new Vector3(0f, -0.5f, 0f);
        Vector3 groundedBoxScal = new Vector3(1.5f, 0.3f, 2f);

        Collider[] projection = Physics.OverlapBox(groundedBoxPos, groundedBoxScal, Quaternion.identity, TrackLayer);

        return (projection.Length > 0);
    }
}
