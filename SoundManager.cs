using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum SoundType {Cardflip = 1, Chip = 2, Returnchips = 3}
    [SerializeField] private AudioClip[] clipArr;
    [SerializeField] private float[] volumeArr;
    [SerializeField] private float pitch;
    [SerializeField] private bool loop;
    [SerializeField] private float spatialblend;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.pitch = pitch;
        audioSource.loop = loop;
        audioSource.spatialBlend = spatialblend;
        audioSource.playOnAwake = false;
    }

    public void PlaySound(SoundType sound)
    {
        audioSource.volume = volumeArr[(int)sound - 1];
        audioSource.PlayOneShot(clipArr[(int)sound - 1]);
    }






}
