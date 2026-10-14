using UnityEngine;
using System.Collections;

public class CreepyWhaleSound : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] whaleSounds;

    [Header("Whale Positions")]
    public Transform[] whalePositions;

    [Header("Timing")]
    public float minDelay = 20f;
    public float maxDelay = 60f;

    [Header("Volume")]
    [Range(0f, 1f)]
    public float minVolume = 0.2f;

    [Range(0f, 1f)]
    public float maxVolume = 0.4f;

    private void Start()
    {
        StartCoroutine(WhaleRoutine());
    }

    private IEnumerator WhaleRoutine()
    {
        while (true)
        {
            // Wait before making the next sound
            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            PlayWhaleSound();
        }
    }

    private void PlayWhaleSound()
    {
        if (whaleSounds.Length == 0)
            return;

        if (whalePositions.Length == 0)
            return;

        // Pick a random position around the ship
        Transform randomPosition =
            whalePositions[Random.Range(0, whalePositions.Length)];

        // Move the sound source there
        transform.position = randomPosition.position;

        // Pick a random whale sound
        AudioClip randomSound =
            whaleSounds[Random.Range(0, whaleSounds.Length)];

        // Random volume
        audioSource.volume =
            Random.Range(minVolume, maxVolume);

        // Play
        audioSource.clip = randomSound;
        audioSource.Play();
    }
}