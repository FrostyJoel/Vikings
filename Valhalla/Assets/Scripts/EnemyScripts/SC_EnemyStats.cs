using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SC_EnemyStats : MonoBehaviour
{
    [Header("EnemyStats")]
    [Range(5f, 10f)]
    [SerializeField] float damage = 5f;
    [Range(20f,50f)]
    [SerializeField] float maxHealth = 20f;
    [Range(20f, 50f)]
    [SerializeField] float attackRange = 1f;

    [Header("EnemyMovement")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float invulnerableTimer = 1f;

    [HideInInspector]
    public NavMeshAgent myAgent;
    public Rigidbody myRB;
    bool gotHit = false;
    float curHealth;
    RaycastHit hit;

    //Todo Remove
    public bool roomClosed;
    [SerializeField] float attackDelay = 2f;

    void Awake()
    {
        if (!SC_TopDownController.single)
        {
            Debug.LogWarning("No Player Found");
            return;
        }
        curHealth = maxHealth;
        myRB = GetComponent<Rigidbody>();
        myAgent = GetComponent<NavMeshAgent>();
        myAgent.updateRotation = true;
        myAgent.stoppingDistance = attackRange;
    }

    // Update is called once per frame
    void Update()
    {
        if(curHealth <= 0)
        {
            Die();
        }
    }
    private void FixedUpdate()
    {
        if (myAgent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(myAgent.velocity.normalized);
        }

        if (!gotHit)
        {
            TempMovement();
        }
        else
        {
            CancelInvoke(nameof(DealDamage));
            if (!IsInvoking(nameof(RestartWalking)))
            {
                Invoke(nameof(RestartWalking), 3f);
            }
        }
    }

    private void TempMovement()
    {
        //Todo Change Dis To locked Room
        if (roomClosed)
        {
            SC_TopDownController player = SC_TopDownController.single;
            float dis = Vector3.Distance(player.transform.position, transform.position);
            myAgent.destination = player.transform.position;

            if (dis > attackRange)
            {
                if (!IsInvoking(nameof(DealDamage)) && myAgent.isStopped)
                {
                    if (!IsInvoking(nameof(RestartWalking)))
                    {
                        Invoke(nameof(RestartWalking), 0.5f);
                    }
                }
                else
                {
                    CancelInvoke(nameof(DealDamage));
                    if (!IsInvoking(nameof(RestartWalking)))
                    {
                        Invoke(nameof(RestartWalking), 2f);
                    }
                }
            }
            else
            {
                myAgent.isStopped = true;
                myAgent.velocity = Vector3.zero;
                InvokeRepeating(nameof(DealDamage), 0, attackDelay);
            }
        }
    }

    public void DealDamage()
    {
        SC_TopDownController.single.DealDamage(damage);
    }

    public void RestartWalking()
    {
        if (!myAgent.updatePosition)
        {
            gotHit = false;
            myAgent.nextPosition = transform.position;
            myRB.isKinematic = true;
            myAgent.updatePosition = true;
            myAgent.isStopped = false;
        }
    }

    private void InvulnerableReset()
    {
        gotHit = false;
    }

    private void Die()
    {
        SC_GameManager.single.UpdateEnemyList(this);
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
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Wall") && gotHit)
        {
            Invoke(nameof(RestartWalking),0f);
        }
    }
}
