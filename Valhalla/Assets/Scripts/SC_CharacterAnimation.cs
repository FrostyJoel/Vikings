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
        anime.SetFloat("MovingZ", zDir);
    }

    public void Update()
    {
        if(xDir > -0.1 && xDir < 0.1 && zDir > -0.1 && zDir < 0.1)
        {
            anime.SetTrigger("Reset");
        }
    }

    public void Attack1()
    {
        anime.SetTrigger("Attack0");
    }
    public void Attack2()
    {
        anime.SetTrigger("Attack1");
    }
}
