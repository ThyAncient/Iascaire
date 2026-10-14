using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class CutsceneEffects : MonoBehaviour
{
    [Header("Engine")]
    public AudioSource engineAudio;
    public float engineDelay = 2f;

    [Header("Clouds")]
    public Volume globalVolume;

    public float cloudTransitionTime = 3f;

    [Header("Final Cloud Opacity")]
    [Range(0f, 1f)]
    public float finalOpacityR = 1f;

    [Range(0f, 1f)]
    public float finalOpacityG = 1f;

    [Range(0f, 1f)]
    public float finalOpacityB = 1f;

    [Range(0f, 1f)]
    public float finalOpacityA = 1f;

    private CloudLayer cloudLayer;

    private float startA_R;
    private float startA_G;
    private float startA_B;
    private float startA_A;

    private float startB_R;
    private float startB_G;
    private float startB_B;
    private float startB_A;

    private Coroutine effectsCoroutine;

    private CutsceneCamera cutsceneCamera;

    private void Start()
    {
        cutsceneCamera =
            FindFirstObjectByType<CutsceneCamera>();

        FindCloudLayer();

        SaveCloudValues();
    }

    void FindCloudLayer()
    {
        if (globalVolume == null)
            return;

        if (globalVolume.profile.TryGet(
            out CloudLayer clouds))
        {
            cloudLayer = clouds;
        }
        else
        {
            Debug.LogWarning(
                "Cloud Layer was not found in Global Volume!"
            );
        }
    }

    void SaveCloudValues()
    {
        if (cloudLayer == null)
            return;

        startA_R =
            cloudLayer.layerA.opacityR.value;

        startA_G =
            cloudLayer.layerA.opacityG.value;

        startA_B =
            cloudLayer.layerA.opacityB.value;

        startA_A =
            cloudLayer.layerA.opacityA.value;

        startB_R =
            cloudLayer.layerB.opacityR.value;

        startB_G =
            cloudLayer.layerB.opacityG.value;

        startB_B =
            cloudLayer.layerB.opacityB.value;

        startB_A =
            cloudLayer.layerB.opacityA.value;
    }

    public void StartEffects()
    {
        if (effectsCoroutine != null)
            StopCoroutine(effectsCoroutine);

        effectsCoroutine =
            StartCoroutine(EffectsSequence());
    }

    public void StopEffects()
    {
        if (effectsCoroutine != null)
        {
            StopCoroutine(effectsCoroutine);
            effectsCoroutine = null;
        }
    }

    IEnumerator EffectsSequence()
    {
        // Engine delay
        yield return new WaitForSeconds(
            engineDelay
        );

        // Engine sound
        if (engineAudio != null)
        {
            engineAudio.Play();
        }

        Debug.Log("Engine sound started.");

        // Camera shake
        if (cutsceneCamera != null)
        {
            cutsceneCamera.StartShake();
        }

        // Clouds
        yield return StartCoroutine(
            TransitionClouds()
        );

        effectsCoroutine = null;
    }

    IEnumerator TransitionClouds()
    {
        if (cloudLayer == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < cloudTransitionTime)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / cloudTransitionTime
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            // Layer A
            cloudLayer.layerA.opacityR.value =
                Mathf.Lerp(
                    startA_R,
                    finalOpacityR,
                    t
                );

            cloudLayer.layerA.opacityG.value =
                Mathf.Lerp(
                    startA_G,
                    finalOpacityG,
                    t
                );

            cloudLayer.layerA.opacityB.value =
                Mathf.Lerp(
                    startA_B,
                    finalOpacityB,
                    t
                );

            cloudLayer.layerA.opacityA.value =
                Mathf.Lerp(
                    startA_A,
                    finalOpacityA,
                    t
                );

            // Layer B
            cloudLayer.layerB.opacityR.value =
                Mathf.Lerp(
                    startB_R,
                    finalOpacityR,
                    t
                );

            cloudLayer.layerB.opacityG.value =
                Mathf.Lerp(
                    startB_G,
                    finalOpacityG,
                    t
                );

            cloudLayer.layerB.opacityB.value =
                Mathf.Lerp(
                    startB_B,
                    finalOpacityB,
                    t
                );

            cloudLayer.layerB.opacityA.value =
                Mathf.Lerp(
                    startB_A,
                    finalOpacityA,
                    t
                );

            yield return null;
        }

        // Make absolutely sure they reach 1
        cloudLayer.layerA.opacityR.value =
            finalOpacityR;

        cloudLayer.layerA.opacityG.value =
            finalOpacityG;

        cloudLayer.layerA.opacityB.value =
            finalOpacityB;

        cloudLayer.layerA.opacityA.value =
            finalOpacityA;

        cloudLayer.layerB.opacityR.value =
            finalOpacityR;

        cloudLayer.layerB.opacityG.value =
            finalOpacityG;

        cloudLayer.layerB.opacityB.value =
            finalOpacityB;

        cloudLayer.layerB.opacityA.value =
            finalOpacityA;
    }
}