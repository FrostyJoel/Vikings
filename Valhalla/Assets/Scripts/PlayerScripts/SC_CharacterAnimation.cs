using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CharacterAnimation : MonoBehaviour
{
    public static SC_CharacterAnimation single;

    public Animator anime;
    float xDir, zDir;
    private void Awake()
    {
        single = this;
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
        Debug.Log("Triggerd" + "MeleeAttack");
        anime.SetTrigger("Attack(Melee)");
    }

    public void ResetMeleeAttack()
    {
        Debug.Log("Triggerd" + "ResetMeleeAttack");
        anime.SetTrigger("HitWall");
    }

    public void HammerThrow()
    {
        Debug.Log("Triggerd: " + "HammerThrow");
        anime.SetTrigger("Attack(Throw)");
    }

    public void LightningAttack()
    {
        Debug.Log("Triggerd: " + "LightningAttack");
        anime.SetTrigger("Attack(Lightning)");
    }

    public void PullBack()
    {
        Debug.Log("Triggerd: " + "PullBack");
        anime.SetTrigger("HammerPull");
    }

    public void Caught()
    {
        Debug.Log("Triggerd: " + "Caught");
        anime.SetTrigger("Caught");
    }
}
