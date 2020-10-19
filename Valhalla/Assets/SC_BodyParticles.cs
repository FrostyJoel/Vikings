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
        //Add Sound
    }

    public void LeftFoot()
    {
        if(leftFoot != null)
        {
            leftFoot.Play();
        }
        //Add Sound
    }

}
