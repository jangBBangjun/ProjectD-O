using System;
using System.Collections;
using UnityEngine;

public class Sub_EffectPlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particleSystems;
    [SerializeField] private bool gun;

    public void Play(int index, float delay = 0)
    {
        if(particleSystems == null && index >= particleSystems.Length && particleSystems[index] == null)
            return;

        StartCoroutine(delayPlay(index, delay));
    }
    private IEnumerator delayPlay(int index, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (gun && index == 0)
            particleSystems[index].Emit(1);
        else
            particleSystems[index].Play();
    }
}
