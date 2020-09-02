using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CharacterAnimation : MonoBehaviour
{
    public Animator anime;
    float xDir, zDir;
    private void Awake()
    {
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

    public void HammerThrow()
    {
        Debug.Log("Triggerd: " + "HammerThrow");
        anime.SetTrigger("Attack0");
    }

    public void LightningAttack()
    {
        Debug.Log("Triggerd: " + "LightningAttack");
        anime.SetTrigger("Attack1");
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
