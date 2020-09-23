using System.Collections;
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
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<SC_EnemyStats>().DealDamageToSelf(attacks.meleeHammerDamageAmount);
                collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.transform.forward * meleeForceAmount, ForceMode.Impulse);
            }
        }
        else
        {
            if (!attacks.hitObject && !attacks.inHand)
            {
                myRB.isKinematic = false;
                myRB.useGravity = true;
                attacks.hitObject = true;
                if (collision.gameObject.CompareTag("Enviorment") && attacks.forceAmount >= attacks.minFlyingForceReq)
                {
                    myRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    myRB.isKinematic = true;
                    myRB.useGravity = false;
                }
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    collision.gameObject.GetComponent<SC_EnemyStats>().DealDamageToSelf(attacks.hammerDamageAmount);
                }
            }
        }
    }
}
