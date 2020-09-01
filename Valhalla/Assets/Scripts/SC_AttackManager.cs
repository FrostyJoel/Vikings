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
        player.charAnimator.anime.SetBool("InHand", playerAttacks.inHand);
    }

    public void ButtonSelect()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            attackPos = new Vector3(player.GetAimTargetPos().x, player.GetAimTargetPos().y + 0.2f, player.GetAimTargetPos().z);
            isAttacking = true;
            if (playerAttacks.inHand)
            {
                player.charAnimator.Attack1();
                playerAttacks.inHand = false;
            }
        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (playerAttacks.hitObject && !playerAttacks.inHand)
            {
                player.charAnimator.PullBack();
                isAttacking = true;
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (playerAttacks.inHand)
            {
                if (!IsInvoking())
                {
                    player.charAnimator.Attack2();
                    attackPos = new Vector3(player.GetAimTargetPos().x, player.GetAimTargetPos().y + 0.2f, player.GetAimTargetPos().z);
                    isAttacking = true;
                }
            }
            else { return; }
        }
    }
}
