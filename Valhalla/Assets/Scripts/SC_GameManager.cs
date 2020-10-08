using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SC_GameManager : MonoBehaviour
{
    public static SC_GameManager single;
    public GameObject enemyPrefab;

    [Header ("HideInInspector")]
    public List<SC_EnemyStats> enemies = new List<SC_EnemyStats>();
    private void Awake()
    {
        single = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        FindAllEnemies();
    }

    public void FindAllEnemies()
    {
        enemies = FindObjectsOfType<SC_EnemyStats>().ToList();
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
        List<GameObject> enemySpawnedRooms = new List<GameObject>();
        for (int i = 0; i < spawnedRooms.Count; i++)
        {
            int randomIndex = Random.Range(0, spawnedRooms.Count);
            GameObject RandomRoom = spawnedRooms[randomIndex];
            if (!enemySpawnedRooms.Contains(RandomRoom))
            {
                enemySpawnedRooms.Add(RandomRoom);

            }
        }
    }

    public void GameDone()
    {
        Debug.Log("GameDone");
        if (SC_MenuManager.single != null)
        {
            SC_MenuManager.single.GetNextRoom();
        }
    }

}
