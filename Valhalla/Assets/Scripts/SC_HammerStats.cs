using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_HammerStats : MonoBehaviour
{
    SC_Attacks attacks;
    Rigidbody myRB;
    RaycastHit hit;
    public bool aboveGround;
    private void Awake()
    {
        attacks = GetComponentInParent<SC_Attacks>();
        myRB = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(Physics.Raycast(transform.position,transform.forward*Mathf.Infinity,out hit))
        {
            aboveGround = true;
        }
        else
        {
            aboveGround = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(attacks.hitObject != true)
        {
            myRB.isKinematic = false;
            myRB.useGravity = true;
            attacks.hitObject = true;
            if(collision.gameObject.tag == "Enviorment" && attacks.forceAmount >= attacks.minFlyingForceReq)
            {
                myRB.isKinematic = true;
                myRB.useGravity = false;
            }
            if(collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<SC_EnemyStats>().DealDamage(attacks.hammerDamageAmount);
            }
        }
    }
}
