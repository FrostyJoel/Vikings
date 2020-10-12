using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SC_GameManager : MonoBehaviour
{
    public static SC_GameManager single;
    public static bool gameStart;
    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    public int amountOfRooms;
    public int minimumAmountOfEnemyRooms;
    public Camera tempStarterCam;


    [Header ("HideInInspector")]
    public int enemyRoomAmount;
    public List<Transform> playerSpawnPos = new List<Transform>();
    public List<SC_EnemyStats> enemies = new List<SC_EnemyStats>();
    private void Awake()
    {
        if(single == null)
        {
            single = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(single);
        }
        GetComponentInChildren<SC_RoomManager>().maxAmountOfRooms = amountOfRooms;
    }

    public void RemoveFromEnemyList(SC_EnemyStats enemy)
    {
        if (enemies.Contains(enemy))
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == enemy)
                {
                    enemies.Remove(enemies[i]);
                    if (enemies.Count <= 0)
                    {
                        GameDone();
                    }
                    break;
                }
            }
        }
    }

    public void GetRandomRoomToSpawnEnemies(List<GameObject> spawnedRooms)
    {
        enemyRoomAmount = Random.Range(minimumAmountOfEnemyRooms, spawnedRooms.Count);
        List<GameObject> enemySpawnedRooms = new List<GameObject>();
        for (int i = 0; i < enemyRoomAmount; i++)
        {
            int randomIndex = Random.Range(0, spawnedRooms.Count);
            GameObject RandomRoom = spawnedRooms[randomIndex];
            if (!enemySpawnedRooms.Contains(RandomRoom) && !RandomRoom.GetComponent<SC_Room>().hasEnemies)
            {
                enemySpawnedRooms.Add(RandomRoom);
                RandomRoom.GetComponent<SC_Room>().hasEnemies = true;
            }
        }

        foreach (GameObject room in enemySpawnedRooms)
        {
            Debug.Log(room.name);
            RandomEnemyAmount(room.GetComponent<SC_Room>());
        }
        SpawnPlayer();
    }

    public void RandomEnemyAmount(SC_Room room)
    {
        if(room.spawnPosEnemies.Length <= 0) { return; }
        List<Transform> enemySpawnPos = new List<Transform>();
        for (int i = 0; i < room.spawnPosEnemies.Length; i++)
        {
            int randomIndex = Random.Range(0, room.spawnPosEnemies.Length);
            Transform randomTransform = room.spawnPosEnemies[randomIndex];

            if (!enemySpawnPos.Contains(randomTransform))
            {
                enemySpawnPos.Add(randomTransform);
            }
        }

        foreach (Transform spawnPos in enemySpawnPos)
        {
            SpawnEnemy(spawnPos);
        }
    }

    public void SpawnEnemy(Transform spawnPoint)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint);
        SC_EnemyStats enemyScript = enemy.GetComponent<SC_EnemyStats>();
        if (!enemies.Contains(enemyScript))
        {
            enemies.Add(enemyScript);
        }
    }

    public void SpawnPlayer()
    {

        int randomIndex = Random.Range(0, playerSpawnPos.Count);

        Instantiate(playerPrefab.gameObject, playerSpawnPos[randomIndex].position, playerSpawnPos[randomIndex].rotation, null);
        tempStarterCam.gameObject.SetActive(false);
        gameStart = true;

        Debug.Log(gameStart);
    }

    public void GameDone()
    {
        Debug.Log("GameDone");
        if (SC_UiManager.single != null)
        {
            SC_UiManager.single.GetNextRoom();
        }
    }

}
