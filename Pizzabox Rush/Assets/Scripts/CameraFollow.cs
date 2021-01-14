using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CameraFollow : SceneObject
{
    [SerializeField] private Transform player;

    [SerializeField] private float lerpSpeed = 10f, rotSpeed = 15f;

    private Rigidbody camRB;

    Vector3 OffSetToPlayer;

    private void Start()
    {
        camRB = GetComponent<Rigidbody>();
        camRB.useGravity = false;

        //Calculates an offset to the player on start
        OffSetToPlayer = player.position - transform.position;

        PizzaBox.OnSaveWorld += StoredDistance;
        PizzaBox.OnResetWorld += ResetPosition;
    }

    private void LateUpdate()
    {
        MoveWithPlayer();
        LookAtPlayer();
    }

    //Translates camera 'z' and 'y' positions based on the offset to the player
    private void MoveWithPlayer()
    {
        Vector3 camPos = transform.position;
        camPos.z = player.position.z - OffSetToPlayer.z;

        //Adds a bit of smoothing to the cam's translation on y-axis 
        camPos.y = Mathf.Lerp(camPos.y, player.position.y - OffSetToPlayer.y, Time.deltaTime * lerpSpeed);
        transform.position = camPos;
    }

    //rotates camera in y-axis based on the player's position
    private void LookAtPlayer()
    {
        Vector3 direction = transform.position - player.position;
        direction.Normalize();

        Vector3 rotDir = Vector3.Cross(direction, transform.up);
        Vector3 yaw = Vector3.up * rotDir.y * rotSpeed;
   
        camRB.angularVelocity = yaw;
    }

    public override void StoredDistance(int objID, Transform trans)
    {
        base.StoredDistance(this.GetInstanceID(), this.transform);
    }

    public override void ResetPosition(int objID, Transform trans)
    {
        base.ResetPosition(this.GetInstanceID(), this.transform);
    }

    public override void RemovePosition(int objID)
    {
        base.RemovePosition(this.GetInstanceID());
    }
}
