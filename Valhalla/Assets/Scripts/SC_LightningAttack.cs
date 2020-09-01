using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_LightningAttack : MonoBehaviour
{
    public SC_Attacks attack;
    [SerializeField] List<SC_EnemyStats> allEnemies = new List<SC_EnemyStats>();
    private void Start()
    {
        StartCoroutine(DealOverTimedamage());
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            if (!allEnemies.Contains(other.GetComponent<SC_EnemyStats>()))
            {
                allEnemies.Add(other.GetComponent<SC_EnemyStats>());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (!allEnemies.Contains(other.GetComponent<SC_EnemyStats>()))
            {
                allEnemies.Add(other.GetComponent<SC_EnemyStats>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (!allEnemies.Contains(other.GetComponent<SC_EnemyStats>()))
            {
                allEnemies.Add(other.GetComponent<SC_EnemyStats>());
            }
            else
            {
                allEnemies.Remove(other.GetComponent<SC_EnemyStats>());
            }
        }
    }

    IEnumerator DealOverTimedamage()
    {
        while (true)
        {
            if(allEnemies.Count > 0)
            {
                foreach (SC_EnemyStats enemy in allEnemies)
                {
                    enemy.DealDamage(attack.damageOverTimeAmount);
                }
                yield return new WaitForSeconds(attack.delay);
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
