using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EnemyStats : MonoBehaviour
{
    [Header("EnemyStats")]
    [Range(5f, 10f)]
    [SerializeField] float damage = 5f;
    [Range(20f,50f)]
    [SerializeField] float maxHealth = 20f;
    
    [Header("EnemyMovement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float invulnerableTimer = 1f;
    [SerializeField] float reachedDistance = 1f;
    bool gotHit = false;
    float curHealth;

    //Todo Remove
    [SerializeField] SC_TopDownController player;
    [SerializeField] float rotateSpeed = 7.5f;
    [SerializeField] float aggroRange = 9f;
    [SerializeField] float attackDelay = 2f;
    bool canWalking = true;

    void Awake()
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
        if (gotHit)
        {
            if (!IsInvoking("InvulnerableReset"))
            {
                Invoke("InvulnerableReset", invulnerableTimer);
            }
        }
    }
    private void FixedUpdate()
    {
        if (!gotHit)
        {
            TempMovement();
        }
        else
        {
            CancelInvoke("DealDamage");
            Invoke("RestartWalking", 3f);
        }
    }

    private void TempMovement()
    {
        //ToDO Remove
        float dis = Vector3.Distance(player.transform.position, transform.position);
        if (dis <= aggroRange)
        {
            Vector3 dir = player.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotateSpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

            if (dis >= reachedDistance)
            {
                if (!IsInvoking("DealDamage") && canWalking)
                {
                    transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                }
                else
                {
                    CancelInvoke("DealDamage");
                    Invoke("RestartWalking", 3f);
                }
            }
            else
            {
                canWalking = false;
                InvokeRepeating("DealDamage", 2f, attackDelay);
            }
        }
    }

    public void DealDamage()
    {
        player.DealDamage(damage);
    }

    public void RestartWalking()
    {
        canWalking = true;
    }

    private void InvulnerableReset()
    {
        gotHit = false;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void DealDamageToSelf(float damage)
    {
        if (!gotHit)
        {
            curHealth -= damage;
            Debug.Log(gameObject.name + " Took: " + Mathf.RoundToInt(damage).ToString());
            gotHit = true;
        }
        else
        {
            Debug.Log("invulnerable");
        }
    }
}
