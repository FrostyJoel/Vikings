using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SC_GameManager : MonoBehaviour
{
    public static SC_GameManager single;
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

    public void UpdateEnemyList(SC_EnemyStats enemy)
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
    public void GameDone()
    {
        Debug.Log("GameDone");
        SC_MenuManager.single.GetNextRoom();
    }

}
