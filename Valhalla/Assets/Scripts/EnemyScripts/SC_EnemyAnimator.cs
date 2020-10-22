using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EnemyAnimator : MonoBehaviour
{
    public void AttackPlayer()
    {
        GetComponentInParent<SC_EnemyStats>().DealDamage();
    }

    public void EndHowl()
    {
        GetComponentInParent<SC_EnemyStats>().doneHowl = true;
    }

    public void Die()
    {
        GetComponentInParent<SC_EnemyStats>().poof.Play();
        GetComponentInParent<Rigidbody>().useGravity = true;
    }

}
