using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private AudioSource BeamShot;
    [SerializeField] private AudioSource RocketShot;
    [SerializeField] private AudioSource RiddlerTalk;
    public void PlayDeathSound()
    {
        deathSound.Play();
    }

    public void PlayBeamShotSound()
    {
        BeamShot.Play();
    }

    public void PlayRocketShotSound()
    {
        RocketShot.Play();
    }

    public void RiddlerSpeech()
    {
        RiddlerTalk.Play();
    }
}
