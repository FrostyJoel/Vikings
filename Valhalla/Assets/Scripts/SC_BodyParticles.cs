using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_BodyParticles : MonoBehaviour
{
    public ParticleSystem leftFoot;
    public ParticleSystem rightFoot;

    public void RightFoot()
    {
        if(rightFoot != null)
        {
            rightFoot.Play();
        }
        SC_AudioManager.single.PlaySound(AudioType.RightFoot);
    }

    public void LeftFoot()
    {
        if(leftFoot != null)
        {
            leftFoot.Play();
        }
        SC_AudioManager.single.PlaySound(AudioType.LeftFoot);
    }

    public void PlayerAttack()
    {
        SC_AudioManager.single.PlaySound(AudioType.PlayerAttack);
    }

    public void EnemyAttack()
    {
        SC_AudioManager.single.PlaySound(AudioType.EnemyAttack);
    }

}
