
using System.Collections.Generic;
using UnityEngine;

public class SC_RoomPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public AvailableSlots tag;
        public List<GameObject> prefab = new List<GameObject>();
    }

    public static SC_RoomPooler single;

    public List<Pool> pools = new List<Pool>();
    public Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

    private void Awake()
    {
        single = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        GameObject poolParent = new GameObject("PoolParent");
        foreach (Pool pool in pools)
        {
            Queue<GameObject> roomPool = new Queue<GameObject>();
            RandomizeRoomPrefabs(pool);
            for (int j = 0; j < pool.prefab.Count; j++)
            {
                GameObject room = Instantiate(pool.prefab[j],poolParent.transform);
                SC_Room roomTest = room.GetComponent<SC_Room>();
                roomTest.roomType = pool.tag;
                for (int i = 0; i < roomTest.attachPoints.Length; i++)
                {
                    AttachPoint currentAttachPoint = roomTest.attachPoints[i];
                    GameObject wall = Instantiate(SC_RoomManager.single.mainWall, currentAttachPoint.point);
                    currentAttachPoint.off = currentAttachPoint.point.localPosition;
                    currentAttachPoint.wall = wall;
                    wall.SetActive(false);
                }
                room.SetActive(false);
                roomTest.isChecker = true;
                roomPool.Enqueue(room);
            }
            poolDictionary.Add((int)pool.tag, roomPool);
        }
    }

    private void RandomizeRoomPrefabs(Pool pool)
    {
        for (int i = 0; i < pool.prefab.Count; i++)
        {
            GameObject temp = pool.prefab[i];
            int randomIndex = Random.Range(i, pool.prefab.Count);
            pool.prefab[i] = pool.prefab[randomIndex];
            pool.prefab[randomIndex] = temp;
        }
    }

    public GameObject SpawnFromPool(AvailableSlots tag,Vector3 position, Quaternion rotation)
    {
        int numberTag = (int)tag;

        if (!poolDictionary.ContainsKey(numberTag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist");
            return null;
        }

        GameObject roomToSpawn = poolDictionary[numberTag].Dequeue();

        roomToSpawn.SetActive(true);
        roomToSpawn.transform.position = position;
        roomToSpawn.transform.rotation = rotation;

        poolDictionary[numberTag].Enqueue(roomToSpawn);

        return roomToSpawn;
    }
}

public enum AvailableSlots
{
    HallWays,
    MainRooms,
    SmallRooms,
    MediumRooms,
    LargeRooms
}
