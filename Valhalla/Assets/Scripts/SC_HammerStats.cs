using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_HammerStats : MonoBehaviour
{
    SC_Attacks attacks;
    Rigidbody myRB;

    private void Awake()
    {
        attacks = GetComponentInParent<SC_Attacks>();
        myRB = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(attacks.hitObject != true)
        {
            myRB.isKinematic = false;
            myRB.useGravity = true;
            attacks.hitObject = true;
            if(collision.gameObject.tag == "Enemy")
            {
                collision.gameObject.GetComponent<SC_EnemyStats>().DealDamage(attacks.hammerDamageAmount);
            }
        }
    }
}
