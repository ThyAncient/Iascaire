using System.Collections;
using UnityEngine;

public class CutsceneCamera : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCamera;
    public Transform cutsceneTarget;

    [Header("Engine Camera Shake")]
    public float shakeDuration = 1.5f;
    public float shakeStrength = 0.08f;
    public float shakeSpeed = 18f;

    private bool following = false;
    private bool shaking = false;

    private Coroutine cameraCoroutine;
    private Coroutine shakeCoroutine;

    public void StartCamera()
    {
        following = true;

        if (cameraCoroutine != null)
            StopCoroutine(cameraCoroutine);

        cameraCoroutine = StartCoroutine(FollowCamera());
    }

    public void StopCamera()
    {
        following = false;
        shaking = false;

        if (cameraCoroutine != null)
        {
            StopCoroutine(cameraCoroutine);
            cameraCoroutine = null;
        }

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }
    }

    public void StartShake()
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(CameraShake());
    }

    IEnumerator FollowCamera()
    {
        while (following)
        {
            if (playerCamera != null &&
                cutsceneTarget != null)
            {
                playerCamera.transform.position =
                    cutsceneTarget.position;

                playerCamera.transform.rotation =
                    cutsceneTarget.rotation;

                if (shaking)
                {
                    float x =
                        (Mathf.PerlinNoise(
                            Time.time * shakeSpeed,
                            0f
                        ) - 0.5f) * 2f;

                    float y =
                        (Mathf.PerlinNoise(
                            0f,
                            Time.time * shakeSpeed
                        ) - 0.5f) * 2f;

                    float z =
                        (Mathf.PerlinNoise(
                            Time.time * shakeSpeed,
                            Time.time * shakeSpeed
                        ) - 0.5f) * 2f;

                    Vector3 shake =
                        new Vector3(x, y, z)
                        * shakeStrength;

                    playerCamera.transform.position += shake;

                    playerCamera.transform.rotation *=
                        Quaternion.Euler(
                            shake * 10f
                        );
                }
            }

            yield return null;
        }
    }

    IEnumerator CameraShake()
    {
        shaking = true;

        Debug.Log("Camera shake started.");

        yield return new WaitForSeconds(
            shakeDuration
        );

        shaking = false;

        Debug.Log("Camera shake finished.");

        shakeCoroutine = null;
    }
}