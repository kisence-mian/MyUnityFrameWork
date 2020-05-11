using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTool_Stop : MonoBehaviour
{
    public ParticleSystem particleSystem;

    private void OnEnable()
    {
        particleSystem.Play(true);
    }

    private void OnDisable()
    {
        StopParticle();
    }

    public void StopParticle()
    {
        if (particleSystem != null)
        {
            particleSystem.Stop(true);
        }
    }

}
