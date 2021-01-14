using UnityEngine;
using System.Collections.Generic;

public class TrackManager : MonoBehaviour
{
    [SerializeField] private Track[] tracks;
    [SerializeField] private Transform playerTrans;

    [SerializeField] private int spawnAtStart = 3;
    [SerializeField] private float thresholdDist = 25f, deactivateDist = 18f, downDist = 10f, upDist = 3f;

    private Queue<Track> liveTracks = new Queue<Track>();

    private float currentDist = 0f, spanDist, ddist = 0f;
    private int counter = 1;

    private void Start()
    {
        spanDist = deactivateDist;

        //Spawns and Arranges some tracks ahead on start
        for (int i = 0; i < spawnAtStart; i++)
        {
            ArrangeTrack();
        }

        PizzaBox.OnResetVariables += ResetVariables;
    }

    private void Update()
    {
        UpdateTracks();

        //Spawns and places new tracks ahead as the player is moving forward
        if (playerTrans.position.z > currentDist - thresholdDist)
        {
            ArrangeTrack();
            counter++;
        }
    }

    private void ResetVariables()
    {
        Track[] tempTracks = liveTracks.ToArray();
        currentDist = tempTracks[tempTracks.Length - 1].transform.position.z + tempTracks[tempTracks.Length - 1].TrackLength;

        counter = 1;
        spanDist = deactivateDist;
        ddist = tempTracks[tempTracks.Length - 1].transform.position.y;
    }

    //Spawns, Arranges and Queues track
    private void ArrangeTrack()
    {
        Track trackObj = Instantiate(tracks[Random.Range(0, tracks.Length)]);
        trackObj.Setup();

        trackObj.transform.position = new Vector3(0f, ddist, currentDist);
        trackObj.SpawnObstacles();

        //Decrements 'ddist' if counter equals a value that is divisible without reminder by the given random number
        if(counter % Random.Range(3, 10) == 0) ddist -= downDist;
 
        liveTracks.Enqueue(trackObj);

        currentDist += trackObj.TrackLength;
    }

    //Dequeues, Unregisters and Destroys track behind when player moves past it
    private void UpdateTracks()
    {
        if (playerTrans.position.z > spanDist)
        {
            Track oldTrack = liveTracks.Dequeue();
            oldTrack.UnregisterObject();

            Destroy(oldTrack.gameObject);

            spanDist += deactivateDist;
        }
    }
}
