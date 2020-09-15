using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_OpeningCheck : MonoBehaviour
{
    //Step 1: Get available pieces from the dungeon to spawn
    //Step 2: Spawn a connected piece
    //Step 3: Wait for the validation period to complete
    //Step 4: Mark the opening as connected and notify the room that a connected chamber is ready

    public List<SC_RoomCheck> availableRooms = new List<SC_RoomCheck>();
    public SC_RoomCheck owner;
    public SC_OpeningCheck connectingTo;

    [Range(0,360)]
    [SerializeField] float rotation;
    [SerializeField] List<SC_RoomCheck> attemptedPieces = new List<SC_RoomCheck>();

    private SC_RoomCheck.AvailableSlots avaSlots;
    private SC_DungeonGeneration dungeonMaster;
    private bool tempConnection;
    private bool smallRoomSpawned;
    private bool mediumRoomSpawned;
    private bool largeRoomSpawned;
    private SC_RoomCheck newlySpawned;

    private void Awake()
    {
        dungeonMaster = SC_DungeonGeneration.instance;
        owner = GetComponentInParent<SC_RoomCheck>();
    }

    public void AssignAvailableRooms(SC_RoomCheck.AvailableSlots slots)
    {
        avaSlots = slots;
        if(avaSlots == SC_RoomCheck.AvailableSlots.Room)
        {
            avaSlots = (SC_RoomCheck.AvailableSlots)RandomizerSlots();
        }

        if(avaSlots == SC_RoomCheck.AvailableSlots.HallWay)
        {
            availableRooms = SC_DungeonGeneration.instance.allCorridors;
        }

        if(avaSlots == SC_RoomCheck.AvailableSlots.SmallRooms)
        {
            availableRooms = SC_DungeonGeneration.instance.allSmallRooms;
        }
        if (avaSlots == SC_RoomCheck.AvailableSlots.MediumRooms)
        {
            availableRooms = SC_DungeonGeneration.instance.allMediumRooms;
        }
        if (avaSlots == SC_RoomCheck.AvailableSlots.LargeRooms)
        {
            availableRooms = SC_DungeonGeneration.instance.allLargeRooms;
        }
    }

    public int RandomizerSlots()
    {
        //int randRoom = Random.Range((int)SC_RoomCheck.AvailableSlots.SmallRooms, (int)SC_RoomCheck.AvailableSlots.LargeRooms);
        int randRoom = (int)SC_RoomCheck.AvailableSlots.SmallRooms;
        return randRoom;
    }

    public IEnumerator SpawnConnectedPiece()
    {
        int temprandomRoom = Random.Range(0, availableRooms.Count - 1);
        while (connectingTo == null)
        {
            //Debug.Log(gameObject.name + "SpawningConnectionPiece");
            for (int i = temprandomRoom; i < availableRooms.Count;)
            {
                if (i == availableRooms.Count-1)
                {
                    i = 0;
                }
                if(attemptedPieces.Count >= availableRooms.Count)
                {
                    CheckToSpawnWall();
                }
                switch (avaSlots)
                {
                    case SC_RoomCheck.AvailableSlots.HallWay:
                        {
                            if (connectingTo != null)
                            {
                                AddToDungeon();
                                yield break;
                            }
                            else
                            {
                                SpawnRoom(availableRooms[i]);
                                yield return new WaitForSeconds(dungeonMaster.spawnPieceDelay);
                                i = CheckForNextSpawn(i);
                            }
                        }
                        break;
                    case SC_RoomCheck.AvailableSlots.SmallRooms:
                        {
                            if (connectingTo != null)
                            {
                                AddToDungeon();
                                dungeonMaster.currSpawnAmountOfRooms++;
                                newlySpawned.transform.parent = dungeonMaster.roomList;
                                yield break;
                            }
                            else
                            {
                                SpawnRoom(availableRooms[i]);
                                yield return new WaitForSeconds(dungeonMaster.spawnPieceDelay);
                                i = CheckForNextSpawn(i);
                            }
                        }
                        break;
                    case SC_RoomCheck.AvailableSlots.MediumRooms:
                        {
                            
                            if (connectingTo != null)
                            {
                                AddToDungeon();
                                dungeonMaster.currSpawnAmountOfRooms++;
                                newlySpawned.transform.parent = dungeonMaster.roomList;
                                yield break;
                            }
                            else
                            {
                                SpawnRoom(availableRooms[i]);
                                yield return new WaitForSeconds(dungeonMaster.spawnPieceDelay);
                                i = CheckForNextSpawn(i);
                            }
                        }
                        break;
                    case SC_RoomCheck.AvailableSlots.LargeRooms:
                        {
                            
                            if (connectingTo != null)
                            {
                                AddToDungeon();
                                dungeonMaster.currSpawnAmountOfRooms++;
                                newlySpawned.transform.parent = dungeonMaster.roomList;
                                yield break;
                            }
                            else
                            {
                                SpawnRoom(availableRooms[i]);
                                yield return new WaitForSeconds(dungeonMaster.spawnPieceDelay);
                                i = CheckForNextSpawn(i);
                            }
                        }
                        break;
                }
            }
        }
    }

    private void AddToDungeon()
    {
        if (!owner.connectedOpenings.Contains(this))
        {
            if (!dungeonMaster.actualDungeon.Contains(newlySpawned))
            {
                newlySpawned.transform.parent = dungeonMaster.corridorList;
                dungeonMaster.actualDungeon.Add(newlySpawned);
                owner.connectedOpenings.Add(this);
            }
        }
    }

    private int CheckForNextSpawn(int i)
    {
        if (connectingTo == null)
        {
            Debug.Log("Next");
            i++;
        }
        return i;
    }

    private void CheckToSpawnWall()
    {
        Debug.Log("SpawnWall");
        newlySpawned = Instantiate(dungeonMaster.wall, transform.position, transform.rotation);
    }

    private void SpawnRoom(SC_RoomCheck spawnedProp)
    {
        SC_RoomCheck newRoom = Instantiate(spawnedProp,transform.position + spawnedProp.disToMiddle , Quaternion.Euler(0, owner.transform.rotation.y + rotation, 0));
        newlySpawned = newRoom;
        newlySpawned.transform.parent = dungeonMaster.transform;
        attemptedPieces.Add(spawnedProp);
        Debug.Log(spawnedProp.name);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "ConnectingPoint")
        {
            if (connectingTo == null)
            {
                //Debug.Log("Connecting");
                connectingTo = other.GetComponent<SC_OpeningCheck>();
            }
        }
    }
}
