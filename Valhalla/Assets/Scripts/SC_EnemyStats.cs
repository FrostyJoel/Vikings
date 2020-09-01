using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EnemyStats : MonoBehaviour
{
    public float maxHealth;
    float curHealth;

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(curHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void DealDamage(float damage)
    {
        curHealth -= damage;
    }
}
