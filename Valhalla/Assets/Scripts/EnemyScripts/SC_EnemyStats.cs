﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SC_EnemyStats : MonoBehaviour
{
    [Header("EnemyStats")]
    //[Range(5f, 10f)]
    public float damage = 5f;
    //[Range(20f,50f)]
    public float maxHealth = 20f;
    //[Range(0.1f, 10f)]
    public float attackRadius = 0.1f;
    //[Range(0.5f,5f)]
    public float stunTime = 0.5f;

    public Transform heightOffset;
    public LayerMask playerMask;
    public ParticleSystem hitEffect;
    public ParticleSystem poof;
    public Slider healthbar;

    [Header("EnemyMovement")]
    public float moveSpeed = 5f;
    public float invulnerableTimer = 1f;
    public float rotSpeed;

    [Header("HideInInspector")]
    public NavMeshAgent myAgent;
    public Rigidbody myRB;
    public Animator myAnimator;
    public bool roomClosed;
    public SC_TopDownController player;
    public bool doneHowl;
    bool playerInRange;
    bool gotHit = false;
    public bool died;
    public float curHealth;
    RaycastHit hit;
    Camera maincam;

    void Awake()
    {
        curHealth = maxHealth;
        myRB = GetComponent<Rigidbody>();
        myAnimator = GetComponentInChildren<Animator>();
        myAgent = GetComponent<NavMeshAgent>();
        myAgent.updateRotation = true;
        
    }

    // Update is called once per frame
    void Update()
    {

        if(curHealth <= 0)
        {
            if (healthbar.gameObject.activeSelf)
            {
                if (!IsInvoking(nameof(TurnOffHealthbar)))
                {
                    Invoke(nameof(TurnOffHealthbar),2f);
                }
            }
            Die();
        }
    }
    private void FixedUpdate()
    {
        if(SC_GameManager.single == null) { return; }
        healthbar.maxValue = maxHealth;
        healthbar.value = curHealth;

        if(maincam == null)
        {
            maincam = FindObjectOfType<Camera>();
        }

        healthbar.transform.LookAt(maincam.transform);

        if (died) { return; }
        if (SC_GameManager.single.gameStart)
        {
            if (player == null)
            {
                player = SC_TopDownController.single;
            }
            if (player.dead)
            {
                myAgent.enabled = false;
                myRB.velocity = Vector3.zero;
                if (myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
                {
                    myAnimator.ResetTrigger("Attack");
                }
            }
            if (!myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                myAgent.isStopped = false;
                SetRotationAndMovement();
            }
            else
            {
                if (IsInvoking(nameof(ResetWalk)))
                {
                    if (myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
                    {
                        myAnimator.ResetTrigger("Attack");
                        myAnimator.SetTrigger("GotHit");
                    }

                }
                myAgent.isStopped = true;
            }
        }
        else
        {
            myAgent.isStopped = true;
        }
    }

    private void SetRotationAndMovement()
    {
        if (myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Die") || IsInvoking(nameof(ResetWalk)))
        {
            return;
        }

        if (roomClosed)
        {
            if (!doneHowl)
            {
                if (!myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Start"))
                {
                    FaceTarget();
                    myAnimator.SetTrigger("Howl");
                    if (!SC_AudioManager.single.IsPlayingSound(AudioType.EnemyHowl))
                    {
                        Invoke(nameof(PlayHowl), 0.75f);
                    }
                }
            }
            else
            {
                myAnimator.ResetTrigger("Howl");
                MovingAnim();
                EnemyMovement();
            }   
        }
    }

    public void PlayHowl()
    {
        SC_AudioManager.single.PlaySound(AudioType.EnemyHowl);
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
        myAgent.destination = player.transform.position;
        float disToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if(disToPlayer <= myAgent.stoppingDistance)
        {
            if (!myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                FaceTarget();
                myAnimator.SetTrigger("Attack");
            }
        }
    }

    public void FaceTarget()
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x,0,dir.y));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotSpeed);
    }

    public void ForceRot()
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.y));
        transform.rotation = lookRot;
    }

    public void DealDamage()
    {
        if (playerInRange)
        {
            Debug.Log("Hitting Player");
            SC_TopDownController.single.DealDamage(damage);
        }
    }

    public void ResetWalk()
    {
        myAgent.nextPosition = transform.position;
        myAgent.updatePosition = true;
    }

    private void InvulnerableReset()
    {
        gotHit = false;
    }

    private void Die()
    {
        died = true;
        myRB.velocity = Vector3.zero;
        myAgent.enabled = false;
        GetComponent<Collider>().enabled = false;
        SC_GameManager.single.RemoveFromEnemyList(this);
        myAnimator.SetTrigger("Death");
    }

    public void TurnOffHealthbar()
    {
        healthbar.gameObject.SetActive(false);
    }

    public void DealDamageToSelf(float damage)
    {
        if (!IsInvoking(nameof(InvulnerableReset)))
        {
            hitEffect.Play();
            SC_AudioManager.single.PlaySound(AudioType.EnemyTakeDamage);
            curHealth -= damage;
            Invoke(nameof(InvulnerableReset), invulnerableTimer);
        }
        if (!IsInvoking(nameof(ResetWalk)))
        {
            Invoke(nameof(ResetWalk), stunTime);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(heightOffset.transform.position, attackRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
