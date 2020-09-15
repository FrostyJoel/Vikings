using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_DungeonGeneration : MonoBehaviour
{
    public static SC_DungeonGeneration instance;
    //Step 1: Spawn a room
    //Step 2: Get the first available room that has unconnected openings
    //Step 3: Tell the room to spawn its’ adjacent chambers
    //Step 4: Wait until the room’s openings have spawned
    //Step 5: Add Created Rooms to the dungeon
    //Step 6: Repeat 2 – 6 until there are no more unconnected openings
    [Header("SpawnDelay")]
    [Range(0.1f,5f)]
    public float spawnDelay = 1f;
    [Range(0.1f, 5f)]
    public float spawnPieceDelay = 0.1f;
    

    [Header("AllRoomPrefabs")]
    [SerializeField] SC_RoomCheck mainRoom;
    public List<SC_RoomCheck> allSmallRooms = new List<SC_RoomCheck>();
    public List<SC_RoomCheck> allMediumRooms = new List<SC_RoomCheck>();
    public List<SC_RoomCheck> allLargeRooms = new List<SC_RoomCheck>();
    public List<SC_RoomCheck> allCorridors = new List<SC_RoomCheck>();
    public SC_RoomCheck wall;

    [Header("AmountOfRooms")]
    [Range(0, 10)]
    public int totalAmountOfRooms = 10;

    [Header("AllSpawnedRoomPrefabs")]
    public List<SC_RoomCheck> actualDungeon = new List<SC_RoomCheck>();
    public Transform corridorList;
    public Transform roomList;
    public int currSpawnAmountOfRooms;

    #region SingleTon
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    private void Start()
    {
        SpawnStarterRoom();
    }

    public void SpawnStarterRoom()
    {
        GameObject starterRoom = Instantiate(mainRoom.gameObject, transform);
        starterRoom.transform.parent = roomList;
        SC_RoomCheck start = starterRoom.GetComponent<SC_RoomCheck>();
        actualDungeon.Add(start);
        StartCoroutine(StartAllSpawning());
    }

    IEnumerator StartAllSpawning()
    {
        while(currSpawnAmountOfRooms < totalAmountOfRooms)
        {
            for (int i = 0; i < actualDungeon.Count;)
            {
                if (!actualDungeon[i].fullyConnected)
                {
                    if (!actualDungeon[i].isSpawning)
                    {
                        actualDungeon[i].StartCoroutine(actualDungeon[i].SpawnRoom());
                    }
                }
                yield return new WaitForSeconds(spawnDelay);
                if (actualDungeon[i].fullyConnected)
                {
                    i++;
                }
            }
        }
        Debug.Log("DoneSpawning");
    }
}
