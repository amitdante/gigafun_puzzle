using System;
using UnityEngine;

/// <summary>
/// Manages audio playback for puzzle interactions.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static Action<SoundType> OnPlayAudio;

    [SerializeField] private AudioClip dragClip;
    [SerializeField] private AudioClip dropIncorrectClip;
    [SerializeField] private AudioClip dropCorrectClip;
    [SerializeField] private AudioClip winClip;

    private AudioSource _audioSource;

    private void Awake()
    {
        OnPlayAudio += PlaySound;
        _audioSource = GetComponent<AudioSource>();
    }

    

    /// <summary>
    /// Plays the appropriate sound based on the provided sound type.
    /// </summary>
    public void PlaySound(SoundType soundType)
    {
        if (_audioSource == null) return;

        switch (soundType)
        {
            case SoundType.Drag:
                if (!_audioSource.isPlaying)
                {
                    _audioSource.loop = true;
                    _audioSource.PlayOneShot(dragClip);
                }
                break;

            case SoundType.DropIncorrect:
                _audioSource.loop = false;
                _audioSource.PlayOneShot(dropIncorrectClip);
                break;

            case SoundType.DropCorrect:
                _audioSource.loop = false;
                _audioSource.PlayOneShot(dropCorrectClip);
                break;

            case SoundType.Win:
                _audioSource.loop = false;
                _audioSource.PlayOneShot(winClip);
                break;

            default:
                Debug.LogWarning("Invalid Sound Type.");
                break;
        }
    }

    private void OnDisable()
    {
        OnPlayAudio -= PlaySound;
    }
}

public enum SoundType
{
    Drag,
    DropIncorrect,
    DropCorrect,
    Win
}