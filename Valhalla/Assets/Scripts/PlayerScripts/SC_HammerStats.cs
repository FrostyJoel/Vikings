﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_HammerStats : MonoBehaviour
{
    public bool aboveGround;
    public bool melee;
    public Animator myHammerAnimation;

    [Range(5f,40f)]
    [SerializeField] float meleeForceAmount = 5f;

    SC_Attacks attacks;
    Rigidbody myRB;
    RaycastHit hit;

    private void Awake()
    {
        attacks = GetComponentInParent<SC_Attacks>();
        myRB = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!attacks.inHand)
        {
            if (Physics.Raycast(transform.position, transform.forward * 1000f, out hit))
            {
                aboveGround = true;
            }
            else
            {
                aboveGround = false;
            }
        }
    }

    public void ResetPos()
    {
        Debug.Log("Reset Pos");
        transform.localPosition = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (melee)
        {
            Debug.Log("Melee");
            if (collision.gameObject.CompareTag("Enviorment"))
            {
                Debug.Log("HittingWall");
                attacks.GetComponentInParent<SC_CharacterAnimation>().ResetMeleeAttack();
                attacks.ResetAttack();
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (melee)
        {
            Debug.Log("Melee");
            if (other.gameObject.CompareTag("Enemy"))
            {
                SC_EnemyStats enemyHit = other.gameObject.GetComponent<SC_EnemyStats>();
                enemyHit.DealDamageToSelf(attacks.meleeHammerDamageAmount);
                HitEnemy(other, enemyHit,meleeForceAmount);
            }
        }
        else
        {
            if (!attacks.hitObject && !attacks.inHand)
            {
                if (other.gameObject.CompareTag("Wall") && attacks.forceAmount >= attacks.minFlyingForceReq)
                {
                    attacks.hitObject = true;   
                    Debug.Log("HitWall");
                    myRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    myRB.isKinematic = true;
                    myRB.useGravity = false;
                    myRB.velocity = Vector3.zero;
                    Collider[] myColliders = GetComponents<Collider>();
                    for (int i = 0; i < myColliders.Length; i++)
                    {
                        if (!myColliders[i].enabled)
                        {
                            myColliders[i].enabled = true;
                        }
                    }
                }
                if (other.gameObject.CompareTag("Enemy"))
                {
                    SC_EnemyStats enemyHit = other.gameObject.GetComponent<SC_EnemyStats>();
                    enemyHit.DealDamageToSelf(attacks.hammerDamageAmount);
                    HitEnemy(other, enemyHit,attacks.forceAmount);
                }
            }
        }
    }

    private void HitEnemy(Collider other, SC_EnemyStats enemyHit,float forceAmount)
    {
        if (enemyHit.myAgent.enabled)
        {
            enemyHit.myAgent.updatePosition = false;
            enemyHit.myRB.isKinematic = false;
        }
        enemyHit.myRB.AddForce(-other.transform.forward * forceAmount, ForceMode.Impulse);
    }
}
