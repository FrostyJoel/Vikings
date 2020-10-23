using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_TextFade : MonoBehaviour
{
    public void EnemyKill()
    {
        gameObject.SetActive(false);
        SC_GameManager.single.killingenemyActivated = true;
    }
    public void EnemyHPDrop()
    {
        gameObject.SetActive(false);
        SC_GameManager.single.enemyHpDropActivated = true;
    }
}
