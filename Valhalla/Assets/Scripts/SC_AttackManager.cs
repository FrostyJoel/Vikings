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
        if (player.charAnimator.anime.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) { return; }
        if (isAttacking) { return; }

        if (Input.GetButtonDown("Fire1"))
        {
            isAttacking = true;
            if (playerAttacks.inHand == true)
            {
                attackPos = new Vector3(player.GetAimTargetPos().x, player.GetAimTargetPos().y + 0.2f, player.GetAimTargetPos().z);
                player.charAnimator.HammerThrow();
                return;
            }
            else
            {
                if (playerAttacks.hitObject == true)
                {
                    player.charAnimator.PullBack();
                }
                return;
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (playerAttacks.inHand)
            {
                if (!IsInvoking())
                {
                    isAttacking = true;
                    player.charAnimator.LightningAttack();
                    attackPos = new Vector3(player.GetAimTargetPos().x, player.GetAimTargetPos().y + 0.2f, player.GetAimTargetPos().z);
                }
            }
            else { return; }
        }
    }
}
