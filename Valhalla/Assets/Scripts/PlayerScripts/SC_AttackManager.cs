﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_AttackManager : MonoBehaviour
{
    [SerializeField] SC_Attacks playerAttacks;
    public SC_TopDownController player;
    public bool isAttacking;
    [SerializeField] float hammerAnimationTimeDelay;

    [HideInInspector]
    public Vector3 attackPos;

    private bool checkIfPressed = false;
    SC_AttackManager single;

    private void Awake()
    {
        single = this;
    }
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
            if (playerAttacks.inHand == true)
            {
                StartCoroutine(CheckButtonHold());
                return;
            }
            else
            {
                if (playerAttacks.hitObject == true)
                {
                    isAttacking = true;
                    player.charAnimator.PullBack();
                }
                return;
            }
        }

        if (Input.GetButton("Fire1"))
        {
            if (playerAttacks.inHand == true && checkIfPressed == false)
            {
                StartCoroutine(CheckButtonHold());
                return;
            }
        }

        if (Input.GetButtonUp("Fire1") && playerAttacks.inHand == true)
        {
            checkIfPressed = false;
            isAttacking = true;
            attackPos = new Vector3(player.GetAimTargetPos().x, player.GetAimTargetPos().y + 0.2f, player.GetAimTargetPos().z);
            Animator hammerAnime = playerAttacks.hammerRB.GetComponent<Animator>();
            if (hammerAnime.enabled == true)
            {
                hammerAnime.SetFloat("SpeedIncreasing", 1.0f);
                hammerAnime.SetTrigger("Reset");
                hammerAnime.enabled = false;
                player.charAnimator.HammerThrow();
            }
            else
            {
                player.charAnimator.MeleeAttack();
                playerAttacks.hammerRB.GetComponent<SC_HammerStats>().melee = true;
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

    IEnumerator CheckButtonHold()
    {
        checkIfPressed = true;
        float holdTimer = 0.0f;
        while (Input.GetButton("Fire1"))
        {
            if(holdTimer >= 0.5f)
            {
                StartCoroutine(HammerCharge());
                yield break;
            }
            holdTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator HammerCharge()
    {
        float forceAdd = 0.5f;
        float forceDelay = 0.5f;
        float speedIncrease = 1.0f;
        Animator hammerAnimator = playerAttacks.hammerRB.GetComponent<Animator>();
        hammerAnimator.enabled = true;
        yield return new WaitForSeconds(hammerAnimationTimeDelay);
        while (Input.GetButton("Fire1"))
        {
            playerAttacks.forceAmount += forceAdd;
            if(forceAdd <= 7.5f)
            {
                forceAdd *= 2f;
            }
            if(playerAttacks.forceAmount <= playerAttacks.maxForce)
            {
                if (playerAttacks.hammerDamageAmount <= playerAttacks.maxhammerDamageAmount)
                {
                    playerAttacks.hammerDamageAmount *= 2;
                }
                else
                {
                    playerAttacks.hammerDamageAmount = playerAttacks.maxhammerDamageAmount;
                }
                playerAttacks.hammerRB.GetComponent<SC_HammerStats>().myHammerAnimation.SetFloat("SpeedIncreasing", speedIncrease);
                speedIncrease++;
                yield return new WaitForSeconds(forceDelay/2f);
            }
            else
            {
                playerAttacks.forceAmount = playerAttacks.maxForce;
                yield break;
            }
        }
    }
}
