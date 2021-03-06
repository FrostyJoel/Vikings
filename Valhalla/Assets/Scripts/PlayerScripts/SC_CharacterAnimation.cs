﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CharacterAnimation : MonoBehaviour
{
    public static SC_CharacterAnimation single;

    public Animator anime;
    float xDir, zDir;
    private void Awake()
    {
        if (single != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            single = this;
        }

        anime = GetComponentInChildren<Animator>();
    }

    public void SetHorizontalAnime(float dir)
    {
        xDir = dir;
        anime.SetFloat("MovingX",xDir);
    }

    public void SetVerticalAnime(float dir)
    {
        zDir = dir;
        anime.SetFloat("MovingY", zDir);
    }

    public void MeleeAttack()
    {
        anime.SetFloat("MeleeAttackSpeed", SC_Attacks.single.hammerAttackSpeed);
        Debug.Log("Triggerd: MeleeAttack");
        anime.SetTrigger("Attack(Melee)");
    }

    public void ResetMeleeAttack()
    {
        Debug.Log("Triggerd: ResetMeleeAttack");
        anime.SetTrigger("HitWall");
    }

    public void HammerThrow()
    {
        Debug.Log("Triggerd: HammerThrow");
        anime.SetTrigger("Attack(Throw)");
    }

    public void LightningAttack()
    {
        Debug.Log("Triggerd: LightningAttack");
        anime.SetTrigger("Attack(Lightning)");
    }

    public void PullBack()
    {
        Debug.Log("Triggerd: PullBack");
        anime.SetTrigger("HammerPull");
    }

    public void Caught()
    {
        Debug.Log("Triggerd: Caught");
        anime.SetTrigger("Caught");
    }

    public void Die()
    {
        Debug.Log("Triggerd: Die");
        anime.SetTrigger("Death");
        SC_AudioManager.single.PlaySound(AudioType.PlayerDeath);
    }

}
