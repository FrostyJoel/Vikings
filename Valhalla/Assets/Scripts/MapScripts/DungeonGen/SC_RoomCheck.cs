using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_RoomCheck : MonoBehaviour
{
    public enum AvailableSlots { HallWay, Room,SmallRooms, MediumRooms, LargeRooms }
    public AvailableSlots allAvailablePlacables;
    public SC_OpeningCheck[] allOpenings;
    public List<SC_OpeningCheck> connectedOpenings = new List<SC_OpeningCheck>();
    public bool fullyConnected;
    public bool isSpawning;

    //Todo Remove
    public Vector3 disToMiddle;

    SC_DungeonGeneration dungeonMaster;

    private void Awake()
    {
        dungeonMaster = SC_DungeonGeneration.instance;
        //Step 1: Get all the room openings
        allOpenings = gameObject.GetComponentsInChildren<SC_OpeningCheck>();
    }

    public IEnumerator SpawnRoom()
    {
        //Step 2: Iterate over each opening and spawn adjacent chamber
        int curOpening = 0;
        while (connectedOpenings.Count < allOpenings.Length)
        {
            //Debug.Log(isSpawning);
            if(!isSpawning)
            {
                //Debug.Log(gameObject.name + " Available Openings: " + allOpenings.Length);
                //Debug.Log(gameObject.name + " Connected Openings: " + connectedOpenings.Count);

                allOpenings[curOpening].AssignAvailableRooms(allAvailablePlacables);
                StartCoroutine(allOpenings[curOpening].SpawnConnectedPiece());
                isSpawning = true;
            }

            yield return new WaitForSeconds(dungeonMaster.spawnDelay);
            
            if (allOpenings[curOpening].connectingTo != null)
            {
                isSpawning = false;
                //Step 3: Wait until all adjacent chambers are spawned and are in a valid state   
                if (curOpening < allOpenings.Length - 1)
                {
                    connectedOpenings.Add(allOpenings[curOpening]);
                    curOpening++;
                }
                else
                {
                    //Step 4: Notify dungeon new rooms are ready
                    fullyConnected = true;
                    yield break;
                }
            }
        }
    }

}
