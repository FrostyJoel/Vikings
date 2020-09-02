using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Attacks : MonoBehaviour
{
    public enum Hammer {LiniearHammer,CurveHammer}

    SC_AttackManager attackMan;
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

    [Range(5f, 20f)]
    public float hammerDamageAmount = 5f;
    [Range(20f, 40f)]
    public float maxForce = 20f;
    public float forceAmount = 0f;
    [Range(1f,5f)]
    [SerializeField] float minforceAmount = 1f;
    [Range(15f,40f)]
    public float minFlyingForceReq = 20f;

    public bool hitObject = false;
    public bool inHand = true;

    [Range(5f, 20f)]
    [SerializeField] float hammerRangeOfFloating = 30f;

    private Vector3 oldPos;

    private bool isReturning = false;

    private float time = 0.0f;

    private void Awake()
    {
        forceAmount = minforceAmount;
        attackMan = FindObjectOfType<SC_AttackManager>();
    }

    private void Update()
    {
        if(isReturning == false && inHand == false && hitObject == false && !attackMan.isAttacking && !hammerRB.GetComponent<SC_HammerStats>().aboveGround)
        {
            float dis = Vector3.Distance(targetHand.position, hammerRB.position);

            if(dis >= hammerRangeOfFloating)
            {
                attackMan.player.charAnimator.PullBack();
                attackMan.isAttacking = true;
            }
        }

        if(isReturning && inHand == false)
        {
            if (time < 1.0f)
            {
                hammerRB.position = getBQCPoint(time, oldPos, curvePoint.position, targetHand.position);
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
        GameObject lightningCircle = Instantiate(lightningCircleVisual, attackMan.attackPos, lightningCircleVisual.transform.rotation);
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
        if(forceAmount <= minFlyingForceReq)
        {
            hammerRB.useGravity = true;
        }
        hammerRB.AddForce(attackMan.player.transform.forward * forceAmount, ForceMode.Impulse);
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
        hammerRB.isKinematic = true;
        hammerRB.useGravity = false;
    }

    //Reset Hammer
    void ResetHammer()
    {
        ReattachToHand();
        ResetPostionHammer();
        ResetAttack();
    }

    private void ReattachToHand()
    {
        attackMan.player.charAnimator.Caught();
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

    Vector3 getBQCPoint(float t, Vector3 p0,Vector3 p1,Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return p;
    }

    public void ResetAttack()
    {
        attackMan.isAttacking = false;
    }
}
