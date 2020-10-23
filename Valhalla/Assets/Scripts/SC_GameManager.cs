using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SC_GameManager : MonoBehaviour
{
    public static SC_GameManager single;
    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    public GameObject hitEffect;
    public int amountOfRooms;
    public int minimumAmountOfEnemyRooms;
    public Camera tempStarterCam;
    public float winOffset;


    [Header ("HideInInspector")]
    public bool gameStart = false;
    public int enemyRoomAmount;
    public bool devMode;
    public bool enemyHpDropActivated;
    public bool killingenemyActivated;
    public List<Transform> playerSpawnPos = new List<Transform>();
    public List<SC_EnemyStats> enemies = new List<SC_EnemyStats>();
    public List<GameObject> allEnemyRooms = new List<GameObject>();

    private void Awake()
    {
        if (single != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            single = this;
            DontDestroyOnLoad(this);
        }

        //tempStarterCam = FindObjectOfType<Camera>();

        if (GetComponentInChildren<SC_RoomManager>() != null)
        { 
            GetComponentInChildren<SC_RoomManager>().maxAmountOfRooms = amountOfRooms;
        }
    }

    private void Update()
    {
        if (!gameStart) { return; }
        if (Input.GetButtonDown("DevMode"))
        {
            devMode = !devMode;
            SC_UiManager.single.devModeText.SetActive(devMode);
        }
        if (devMode)
        {
            if (Input.GetButtonDown("KillEnemies") && !killingenemyActivated)
            {
                foreach (SC_EnemyStats enemy in enemies)
                {
                    enemy.curHealth = 0;
                }
                SC_UiManager.single.killingAllEnemies.SetActive(true);
            }

            if (Input.GetButtonDown("Invulnerability"))
            {
                SC_TopDownController.single.invulnerable = !SC_TopDownController.single.invulnerable;
                SC_UiManager.single.invulnerableText.SetActive(!SC_UiManager.single.invulnerableText.activeSelf);
            }

            if (Input.GetButtonDown("OneShot") && !enemyHpDropActivated)
            {
                foreach (SC_EnemyStats enemy in enemies)
                {
                    enemy.maxHealth = 1f;
                    enemy.curHealth = enemy.maxHealth;
                }
                SC_UiManager.single.enemyHpDrop.SetActive(true);
            }
        }
    }

    public void ResetManager()
    {
        gameStart = false;
        enemyRoomAmount = 0;
        playerSpawnPos.Clear();
        enemies.Clear();
        allEnemyRooms.Clear();
        enemyHpDropActivated = false;
        killingenemyActivated = false;
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
                        if (!IsInvoking(nameof(GameWon)))
                        {
                            Invoke(nameof(GameWon),winOffset);
                        }
                    }
                    break;
                }
            }
            for (int ia = 0; ia < allEnemyRooms.Count; ia++)
            {
                SC_Room currentEnemyRoom = allEnemyRooms[ia].GetComponent<SC_Room>();
                if (currentEnemyRoom.enemiesInRoom.Contains(enemy))
                {
                    currentEnemyRoom.enemiesInRoom.Remove(enemy);
                }
            }
        }
    }

    public void GetRandomRoomToSpawnEnemies(List<GameObject> spawnedRooms)
    {
        allEnemyRooms = spawnedRooms;
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
        int randomEnemyAmountIndex = Random.Range(1, room.maxAmountOfEnemies);
        for (int i = 0; i < randomEnemyAmountIndex; i++)
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
            SpawnEnemy(spawnPos, room);
        }
    }

    public void SpawnEnemy(Transform spawnPoint,SC_Room room)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint);
        SC_EnemyStats enemyScript = enemy.GetComponent<SC_EnemyStats>();
        if (!enemies.Contains(enemyScript))
        {
            enemies.Add(enemyScript);
            room.enemiesInRoom.Add(enemyScript);
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

    public void GameWon()
    {
        Debug.Log("GameWon");
        if (SC_UiManager.single != null)
        {
            gameStart = false;
            SC_UiManager.single.GetGameWonScreen();
        }
    }
    public void GameLost()
    {
        Debug.Log("GameLost");
        if (SC_UiManager.single != null)
        {
            gameStart = false;
            SC_UiManager.single.GetGameLostScreen();
        }
    }
}
