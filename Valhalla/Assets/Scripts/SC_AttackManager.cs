using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_AttackManager : MonoBehaviour
{
    [SerializeField] SC_Attacks playerAttacks;
    public SC_TopDownController player;
    public bool isAttacking;
    [HideInInspector]
    public Vector3 attackPos;

    private void Update()
    {
        ButtonSelect();
    }

    public void ButtonSelect()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            player.charAnimator.Attack1();
            attackPos = new Vector3(player.GetAimTargetPos().x, player.GetAimTargetPos().y + 0.2f, player.GetAimTargetPos().z);
            isAttacking = true;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (!IsInvoking())
            {
                player.charAnimator.Attack2();
                attackPos = new Vector3(player.GetAimTargetPos().x, player.GetAimTargetPos().y + 0.2f, player.GetAimTargetPos().z);
                isAttacking = true;
            }
        }
    }
}
