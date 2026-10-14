using System.Collections;
using UnityEngine;

public class FishingCamera : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCamera;

    [Header("Fishing Camera Target")]
    public Transform fishingCameraTarget;

    [Header("Movement")]
    public float cameraMoveTime = 0.75f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool followingFishingTarget = false;

    private Coroutine cameraCoroutine;


    public void MoveToFishingPosition()
    {
        if (playerCamera == null ||
            fishingCameraTarget == null)
            return;

        // Save the player's current camera position
        originalPosition =
            playerCamera.transform.position;

        originalRotation =
            playerCamera.transform.rotation;

        followingFishingTarget = true;

        if (cameraCoroutine != null)
            StopCoroutine(cameraCoroutine);

        cameraCoroutine =
            StartCoroutine(
                MoveToFishingCamera()
            );
    }


    public void ReturnToPlayer()
    {
        followingFishingTarget = false;

        if (cameraCoroutine != null)
            StopCoroutine(cameraCoroutine);

        cameraCoroutine =
            StartCoroutine(
                ReturnCamera()
            );
    }


    IEnumerator MoveToFishingCamera()
    {
        Vector3 startPosition =
            playerCamera.transform.position;

        Quaternion startRotation =
            playerCamera.transform.rotation;

        float elapsed = 0f;

        // First move smoothly to the fishing camera
        while (elapsed < cameraMoveTime)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / cameraMoveTime
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            if (fishingCameraTarget != null)
            {
                playerCamera.transform.position =
                    Vector3.Lerp(
                        startPosition,
                        fishingCameraTarget.position,
                        t
                    );

                playerCamera.transform.rotation =
                    Quaternion.Slerp(
                        startRotation,
                        fishingCameraTarget.rotation,
                        t
                    );
            }

            yield return null;
        }

        // Now continuously follow the target
        while (followingFishingTarget)
        {
            if (playerCamera != null &&
                fishingCameraTarget != null)
            {
                playerCamera.transform.position =
                    fishingCameraTarget.position;

                playerCamera.transform.rotation =
                    fishingCameraTarget.rotation;
            }

            yield return null;
        }

        cameraCoroutine = null;
    }


    IEnumerator ReturnCamera()
    {
        Vector3 startPosition =
            playerCamera.transform.position;

        Quaternion startRotation =
            playerCamera.transform.rotation;

        float elapsed = 0f;

        while (elapsed < cameraMoveTime)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / cameraMoveTime
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            playerCamera.transform.position =
                Vector3.Lerp(
                    startPosition,
                    originalPosition,
                    t
                );

            playerCamera.transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    originalRotation,
                    t
                );

            yield return null;
        }

        playerCamera.transform.position =
            originalPosition;

        playerCamera.transform.rotation =
            originalRotation;

        cameraCoroutine = null;
    }
}