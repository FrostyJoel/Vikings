using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_EnemyAnimator : MonoBehaviour
{
    public void AttackPlayer()
    {
        GetComponentInParent<SC_EnemyStats>().DealDamage();
    }
}
