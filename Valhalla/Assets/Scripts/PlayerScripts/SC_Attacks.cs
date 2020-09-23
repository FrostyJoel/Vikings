﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Attacks : MonoBehaviour
{
    public enum Hammer {LiniearHammer,CurveHammer}

    public static SC_Attacks single;

    [Header("LightnigCircleAttack")]
    [Range(0.5f,1f)]
    public float delay = 1.5f;
    public GameObject lightningCircleVisual;

    [Range(2f, 10f)]
    public float damageOverTimeAmount = 3f;
    [Range(2f, 10f)]
    [SerializeField] float duration = 2f;

    [Header("HammerAttack")]
    public Hammer ReturnType = Hammer.LiniearHammer;
    public Rigidbody hammerRB;
    public Transform targetHand, curvePoint;

    [Range(5f, 40f)]
    public float meleeHammerDamageAmount;
    [Range(5f, 40f)]
    public float maxhammerDamageAmount;
    [Range(20f, 40f)]
    public float maxForce = 20f;
    [Range(15f,40f)]
    public float minFlyingForceReq = 20f;

    [HideInInspector]
    public float hammerDamageAmount = 0f;
    [HideInInspector]
    public float forceAmount = 0f;

    public bool hitObject = false;
    public bool inHand = true;

    [Range(1f,5f)]
    [SerializeField] float minforceAmount = 1f;

    [Range(5f, 20f)]
    [SerializeField] float hammerRangeOfFloating = 30f;

    private Vector3 oldPos;
    private bool isReturning = false;
    private float time = 0.0f;

    private void Awake()
    {
        forceAmount = minforceAmount;
        single = this;
    }

    private void Update()
    {
        if(isReturning == false && inHand == false && hitObject == false && !SC_AttackManager.single.isAttacking && !hammerRB.GetComponent<SC_HammerStats>().aboveGround)
        {
            float dis = Vector3.Distance(targetHand.position, hammerRB.position);

            if(dis >= hammerRangeOfFloating)
            {
                SC_CharacterAnimation.single.PullBack();
                SC_AttackManager.single.isAttacking = true;
            }
        }

        if(isReturning && inHand == false)
        {
            if (time < 1.0f)
            {
                hammerRB.position = GetBQCPoint(time, oldPos, curvePoint.position, targetHand.position);
                hammerRB.rotation = Quaternion.Slerp(hammerRB.transform.rotation, targetHand.rotation, 7.5f * Time.deltaTime);
                time += Time.deltaTime;
            }
            else
            {
                ResetHammer();
            }
        }
    }

    public void LightningCircle()
    {
        GameObject lightningCircle = Instantiate(lightningCircleVisual, SC_AttackManager.single.attackPos, lightningCircleVisual.transform.rotation);
        lightningCircle.GetComponent<SC_LightningAttack>().attack = this;
        //Todo Add Fading Effect to Particle
        Destroy(lightningCircle, duration);
        
    }

    //Hammer Throw
    public void HammerThrow()
    {
        isReturning = false;
        inHand = false;
        HammerAddingForce();
        HammerRotation();
    }

    //Adding force to Hammer
    private void HammerAddingForce()
    {
        hammerRB.transform.parent = null;
        hammerRB.isKinematic = false;
        hammerRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
        if (forceAmount <= minFlyingForceReq)
        {
            hammerRB.useGravity = true;
        }
        hammerRB.AddForce(SC_TopDownController.single.transform.forward * forceAmount, ForceMode.Impulse);
    }

    //Rotate in the correct Rotation
    private void HammerRotation()
    {
        Quaternion lookRotation = Quaternion.LookRotation(hammerRB.transform.forward);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10f).eulerAngles;
        hammerRB.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    //Returning Of The Hammer
    public void HammerReturn()
    {
        time = 0.0f;
        oldPos = hammerRB.position;
        isReturning = true;
        hammerRB.velocity = Vector3.zero;
        hammerRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        hammerRB.isKinematic = true;
        hammerRB.useGravity = false;
    }

    //Reset Hammer
    void ResetHammer()
    {
        ReattachToHand();
        ResetPostionHammer();
        TakeOffHammerCollider();
        ResetAttack();
    }

    private void ReattachToHand()
    {
        SC_CharacterAnimation.single.Caught();
        isReturning = false;
        inHand = true;
        hitObject = false;
    }

    private void ResetPostionHammer()
    {
        forceAmount = minforceAmount;
        hammerRB.transform.parent = targetHand;
        hammerRB.position = targetHand.position;
        hammerRB.rotation = targetHand.rotation;
    }

    Vector3 GetBQCPoint(float t, Vector3 p0,Vector3 p1,Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return p;
    }

    public void TakeOffHammerCollider()
    {
        if (hammerRB.GetComponent<Collider>().enabled == true)
        {
            hammerRB.GetComponent<Collider>().enabled = false;
        }
        else
        {
            return;
        }
    }

    public void TurnOnHammerCollider()
    {
        if(hammerRB.GetComponent<Collider>().enabled == false)
        {
            hammerRB.GetComponent<Collider>().enabled = true;
        }
        else
        {
            return;
        }
    }

    public void ResetAttack()
    {
        SC_AttackManager.single.isAttacking = false;
        if (hammerRB.GetComponent<SC_HammerStats>().melee)
        {
            hammerRB.GetComponent<SC_HammerStats>().melee = false;
        }
    }
}
