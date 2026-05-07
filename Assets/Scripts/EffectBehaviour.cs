using System.Collections;
using UnityEngine;

public class EffectBehaviour : MonoBehaviour
{
    public ParticleSystem Particle;
    public AudioClip SFX;
    [Range(0,1f)]public float SFXVolume = 1f;

    private Coroutine _CurrentFXCoroutine;
   
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void PlayFX()
    {
        if (_CurrentFXCoroutine != null)
            return;

        gameObject.SetActive(true);
        _CurrentFXCoroutine = StartCoroutine(FXCoroutine());

    }

    IEnumerator FXCoroutine()
    {
        Particle.Play();

        if (SFX != null)
            AudioManager.Instance.PlaySfx(SFX,SFXVolume);

        float time = Particle.main.duration;

        yield return new WaitForSeconds(time);

        _CurrentFXCoroutine = null;
        gameObject.SetActive(false);
    }
}
