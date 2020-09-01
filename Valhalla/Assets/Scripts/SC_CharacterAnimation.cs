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

    public void Attack1()
    {
        anime.ResetTrigger("Attack0");
        anime.SetTrigger("Attack0");
    }

    public void PullBack()
    {
        anime.ResetTrigger("HammerPull");
        anime.SetTrigger("HammerPull");
    }

    public void Attack2()
    {
        anime.ResetTrigger("Attack1");
        anime.SetTrigger("Attack1");
    }
}
