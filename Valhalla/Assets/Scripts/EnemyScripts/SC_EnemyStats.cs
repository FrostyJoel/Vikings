using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SC_EnemyStats : MonoBehaviour
{
    [Header("EnemyStats")]
    [Range(5f, 10f)]
    [SerializeField] private float damage = 5f;
    [Range(20f,50f)]
    [SerializeField] private float maxHealth = 20f;
    [Range(0.1f, 5f)]
    [SerializeField] private float attackRange = 1f;
    [Range(0.1f, 1f)]
    [SerializeField] private float attackRadius;

    [SerializeField] private Transform attackSphere;
    [SerializeField] private LayerMask playerMask;

    [Header("EnemyMovement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float invulnerableTimer = 1f;

    [Header("HideInInspector")]
    public NavMeshAgent myAgent;
    public Rigidbody myRB;
    public Animator myAnimator;
    public bool roomClosed;

    bool gotHit = false;
    float curHealth;
    RaycastHit hit;

    void Awake()
    {
        if (!SC_TopDownController.single)
        {
            Debug.LogWarning("No Player Found");
            return;
        }
        curHealth = maxHealth;
        myRB = GetComponent<Rigidbody>();
        myAnimator = GetComponentInChildren<Animator>();
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
        SetRotationAndMovement();
    }

    private void SetRotationAndMovement()
    {
        if (myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Die")) 
        {
            myAgent.velocity = Vector3.zero;
            myAgent.isStopped = true;
            return;
        }
        if (myAgent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            transform.rotation = Quaternion.LookRotation(myAgent.velocity.normalized);
        }

        if (!gotHit)
        {
            if (myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                myAnimator.ResetTrigger("Attack");
                if (!IsInvoking(nameof(RestartWalking)))
                {
                    Invoke(nameof(RestartWalking), myAnimator.GetCurrentAnimatorStateInfo(0).length);
                }
            }
            else
            {
                MovingAnim();
                EnemyMovement();
            }
        }
    }

    private void MovingAnim()
    {
        float vel;
        if (Mathf.Abs(myAgent.velocity.x) < Mathf.Abs(myAgent.velocity.z))
        {
            if (myAgent.velocity.z < 0)
            {
                vel = myAgent.velocity.z * -1;
            }
            else
            {
                vel = myAgent.velocity.z;
            }
        }
        else
        {
            if (myAgent.velocity.x < 0)
            {
                vel = myAgent.velocity.x * -1;
            }
            else
            {
                vel = myAgent.velocity.x;
            }
        }

        myAnimator.SetFloat("Moving", vel);
    }

    private void EnemyMovement()
    {
        //Todo Change Dis To locked Room
        if (roomClosed)
        {
            SC_TopDownController player = SC_TopDownController.single;
            float dis = Vector3.Distance(player.transform.position, transform.position);

            if (dis < attackRange)
            {
                if (!myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
                {
                    myAnimator.SetTrigger("Attack");
                }
                myAgent.isStopped = true;
                myAgent.velocity = Vector3.zero;
            }
            else
            {
                myAgent.destination = player.transform.position;
                if (myAgent.isStopped)
                {
                    RestartWalking();
                }
            }
        }
    }

    private bool IsEqual(float a, float b)
    {
        if(a >= b - Mathf.Epsilon && a <= b + Mathf.Epsilon)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void DealDamage()
    {
        if (Physics.SphereCast(attackSphere.position, attackRadius, transform.forward, out RaycastHit hit, attackRange))
        {
            Debug.Log("HitSomething");
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                Debug.Log("DamageToPlayer");
                SC_TopDownController.single.DealDamage(damage);
            }
        }
    }

    public void RestartWalking()
    {
        if (myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            myAnimator.SetTrigger("Reset");
        }
        gotHit = false;
        if (!myAgent.updatePosition)
        {
            myRB.isKinematic = true;
            myAgent.updatePosition = true;
            myAgent.nextPosition = transform.position;
        }
        myAgent.isStopped = false;
    }

    private void InvulnerableReset()
    {
        gotHit = false;
    }

    private void Die()
    {
        myRB.velocity = Vector3.zero;
        SC_GameManager.single.RemoveFromEnemyList(this);
        myAnimator.SetTrigger("Death");
        Destroy(gameObject, myAnimator.GetCurrentAnimatorStateInfo(0).length + 1f);
    }

    public void DealDamageToSelf(float damage)
    {
        if (!gotHit)
        {
            curHealth -= damage;
            gotHit = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Wall") && gotHit)
        {
            RestartWalking();
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(attackSphere.position, attackSphere.position + attackSphere.forward * attackRange);
        Gizmos.DrawWireSphere(attackSphere.position + attackSphere.transform.forward * attackRange, attackRadius);
    }
}
