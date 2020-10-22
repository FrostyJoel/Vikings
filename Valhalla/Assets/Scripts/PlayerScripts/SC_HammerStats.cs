using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_HammerStats : MonoBehaviour
{
    public bool aboveGround;
    public bool melee;
    public Animator myHammerAnimation;
    //[Range(0.1f,40f)]
    [SerializeField] float meleeForceAmount = 5f;

    SC_Attacks attacks;
    Rigidbody myRB;
    

    private void Awake()
    {
        attacks = GetComponentInParent<SC_Attacks>();
        myRB = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!attacks.inHand)
        {
            if (!Physics.Raycast(transform.position, -transform.up * 1000f,out RaycastHit downHit))
            {
                aboveGround = false;
            }
            else
            {
                if (downHit.transform.CompareTag("Ground"))
                {
                    float dis = Vector3.Distance(transform.position, downHit.point);
                    if(dis <= 0.1f)
                    {
                        attacks.hitObject = true;
                        myRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                        myRB.velocity = Vector3.zero;
                        myRB.isKinematic = true;
                        myRB.useGravity = false;
                    }
                }
            }
            if(Physics.Raycast(transform.position,transform.forward * 1000f,out RaycastHit forwardHit))
            {
                if (forwardHit.transform.CompareTag("Wall") && attacks.forceAmount >= attacks.minFlyingForceReq)
                {
                    Debug.Log("HitWall");
                    float dis = Vector3.Distance(transform.position, forwardHit.point);
                    if(dis <= 1f)
                    {
                        transform.rotation = Quaternion.LookRotation(-forwardHit.normal);
                        attacks.hitObject = true;
                        Debug.Log("StuckToWall");
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
                }
            }
        }
    }
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(transform.position, transform.forward * 1000f);
    //}

    public void ResetPos()
    {
        Debug.Log("Reset Pos");
        transform.localPosition = Vector3.zero;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (melee)
        {
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
        enemyHit.ForceRot();
        enemyHit.myRB.AddForce(-other.transform.forward * forceAmount, ForceMode.Impulse);
    }
}
