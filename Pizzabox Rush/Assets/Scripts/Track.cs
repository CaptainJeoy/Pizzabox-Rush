using UnityEngine;

[System.Serializable]
public struct ObstacleProperties
{
    public GameObject Obstacle;
    public float ZOffset;
    public float YPoint;
}

public class Track : SceneObject
{
    public float TrackLength = 18f;

    [SerializeField] private float xSpaceBtw = 2.5f;
    [SerializeField] private Vector2 obstacleAmtRange;

    [SerializeField] private ObstacleProperties[] Obstacle;

    private int totalAmountOfObstacles, currAmtOfObstacleSpawned;
    private float currLocalZPoint, currLocalXPoint;

    public void Setup()
    {
        //Starting local Z point in the Track object's area
        currLocalZPoint = -TrackLength / 2f;

        //Starting local X point in the Track object's area
        currLocalXPoint = -xSpaceBtw; 

        currAmtOfObstacleSpawned = 0;
        totalAmountOfObstacles = Random.Range((int)obstacleAmtRange.x, (int)obstacleAmtRange.y);

        PizzaBox.OnSaveWorld += StoredDistance;
        PizzaBox.OnResetWorld += ResetPosition;
    }

    public void UnregisterObject()
    {
        PizzaBox.OnSaveWorld -= StoredDistance;
        PizzaBox.OnResetWorld -= ResetPosition;

        RemovePosition(this.GetInstanceID());
    }

    public void SpawnObstacles()
    {
        //Spawns and places obstacles locally on top of the Track object
        while (currAmtOfObstacleSpawned < totalAmountOfObstacles && currLocalZPoint < TrackLength / 2f)
        {
            int randObstacle = Random.Range(0, Obstacle.Length);

            //calculates a max value such that when Quantized, makes the value 'xSpaceBtw' be the upper limit
            int xRangeMax = (int)(xSpaceBtw + (xSpaceBtw / 2f));
            currLocalXPoint = QuantizeValue(Random.Range(-xRangeMax, xRangeMax), xSpaceBtw);  //therefore, integer random value will either be '-xSpaceBtw', 0, or 'xSpaceBtw'

            //Adds padding to the first spawned obstacle from the edge
            if (currAmtOfObstacleSpawned <= 0) currLocalZPoint += Obstacle[randObstacle].ZOffset;

            GameObject obstacle = Instantiate(Obstacle[randObstacle].Obstacle);
            obstacle.transform.parent = transform;
            obstacle.transform.localPosition = new Vector3(currLocalXPoint, Obstacle[randObstacle].YPoint, currLocalZPoint);

            currLocalZPoint += Obstacle[randObstacle].ZOffset;
            currAmtOfObstacleSpawned++;
        }
    }

    //Approximates 'val' to a Nearest Multiple of 'baseValue'
    private float QuantizeValue(float val, float baseValue)
    {
        if (val % baseValue != 0f)
        {
            float mod = val % baseValue;
            return (mod < (baseValue - 0.5f)) ? val - mod : (val - mod) + baseValue;
        }

        return val;
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