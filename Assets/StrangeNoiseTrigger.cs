using System.Collections;
using UnityEngine;

public class StrangeNoiseTrigger : MonoBehaviour
{
    [Header("Player")]
    public PlayerInteraction playerInteraction;
    public PlayerCamera playerCamera;
    public PlayerMovement playerMovement;

    [Header("Scene Camera")]
    public SceneCameraScript sceneCameraScript;

    [Header("Camera")]
    public Camera mainCamera;

    [Header("Investigation Camera")]
    public Transform investigationTarget;

    [Header("Camera Movement")]
    public float moveForwardDistance = 1.5f;
    public float moveDownDistance = 0.3f;
    public float moveTime = 0.8f;

    [Header("Look Around")]
    public float lookDownAngle = 15f;
    public float lookSideAngle = 35f;
    public float lookTime = 0.6f;
    public float pauseTime = 0.2f;

    [Header("Task")]
    public StrangeNoiseTask strangeNoiseTask;

    [Header("Hold E")]
    public float holdTime = 1f;

    private bool investigating = false;
    private bool completed = false;

    private float holdTimer = 0f;

    private void Update()
    {
        if (completed || investigating)
            return;

        if (playerInteraction == null)
            return;

        if (playerInteraction.IsLookingAtObject(gameObject))
        {
            if (Input.GetKey(KeyCode.E))
            {
                holdTimer += Time.deltaTime;

                if (holdTimer >= holdTime)
                {
                    holdTimer = 0f;

                    StartCoroutine(
                        Investigate()
                    );
                }
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                holdTimer = 0f;
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }

    // =========================================================
    // CAN INTERACT
    // =========================================================

    public bool CanInteract()
    {
        if (completed)
            return false;

        if (investigating)
            return false;

        if (strangeNoiseTask == null)
            return false;

        return strangeNoiseTask.IsTriggerActive(this);
    }

    // =========================================================
    // INVESTIGATE
    // =========================================================

    IEnumerator Investigate()
    {
        investigating = true;

        // =====================================================
        // DISABLE SCENE CAMERA CONTROL
        // =====================================================

        if (sceneCameraScript != null)
        {
            sceneCameraScript.enabled = false;
        }

        // Disable normal player camera control
        if (playerCamera != null)
            playerCamera.enabled = false;

        // Disable player movement
        if (playerMovement != null)
            playerMovement.enabled = false;

        // =====================================================
        // SAVE ORIGINAL CAMERA POSITION
        // =====================================================

        Vector3 originalPosition =
            mainCamera.transform.position;

        Quaternion originalRotation =
            mainCamera.transform.rotation;

        Vector3 targetPosition =
            investigationTarget.position;

        Quaternion targetRotation =
            investigationTarget.rotation;

        // =====================================================
        // MOVE TO INVESTIGATION LOCATION
        // =====================================================

        yield return StartCoroutine(
            MoveCamera(
                originalPosition,
                targetPosition,
                originalRotation,
                targetRotation
            )
        );

        yield return new WaitForSeconds(
            pauseTime
        );

        // =====================================================
        // LOOK LEFT / RIGHT
        // =====================================================

        Quaternion lookLeft =
            targetRotation *
            Quaternion.Euler(
                lookDownAngle,
                -lookSideAngle,
                0f
            );

        Quaternion lookRight =
            targetRotation *
            Quaternion.Euler(
                lookDownAngle,
                lookSideAngle,
                0f
            );

        yield return StartCoroutine(
            RotateCamera(
                mainCamera.transform.rotation,
                lookLeft
            )
        );

        yield return new WaitForSeconds(
            pauseTime
        );

        yield return StartCoroutine(
            RotateCamera(
                mainCamera.transform.rotation,
                lookRight
            )
        );

        yield return new WaitForSeconds(
            pauseTime
        );

        yield return StartCoroutine(
            RotateCamera(
                mainCamera.transform.rotation,
                lookLeft
            )
        );

        yield return new WaitForSeconds(
            pauseTime
        );

        yield return StartCoroutine(
            RotateCamera(
                mainCamera.transform.rotation,
                targetRotation
            )
        );

        // =====================================================
        // RETURN CAMERA
        // =====================================================

        yield return StartCoroutine(
            MoveCamera(
                mainCamera.transform.position,
                originalPosition,
                mainCamera.transform.rotation,
                originalRotation
            )
        );

        // =====================================================
        // GIVE PLAYER CONTROL BACK
        // =====================================================

        if (playerCamera != null)
            playerCamera.enabled = true;

        if (playerMovement != null)
            playerMovement.enabled = true;

        // =====================================================
        // COMPLETE TRIGGER
        // =====================================================

        completed = true;
        investigating = false;

        if (strangeNoiseTask != null)
        {
            strangeNoiseTask.TriggerInvestigated(
                this
            );
        }

        // Hide this trigger
        gameObject.SetActive(false);

        // =====================================================
        // RE-ENABLE SCENE CAMERA SCRIPT
        // =====================================================

        if (sceneCameraScript != null)
        {
            sceneCameraScript.enabled = true;
        }
    }

    // =========================================================
    // MOVE CAMERA
    // =========================================================

    IEnumerator MoveCamera(
        Vector3 startPosition,
        Vector3 endPosition,
        Quaternion startRotation,
        Quaternion endRotation
    )
    {
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / moveTime
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            mainCamera.transform.position =
                Vector3.Lerp(
                    startPosition,
                    endPosition,
                    t
                );

            mainCamera.transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    endRotation,
                    t
                );

            yield return null;
        }

        mainCamera.transform.position =
            endPosition;

        mainCamera.transform.rotation =
            endRotation;
    }

    // =========================================================
    // ROTATE CAMERA
    // =========================================================

    IEnumerator RotateCamera(
        Quaternion startRotation,
        Quaternion endRotation
    )
    {
        float elapsed = 0f;

        while (elapsed < lookTime)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / lookTime
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            mainCamera.transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    endRotation,
                    t
                );

            yield return null;
        }

        mainCamera.transform.rotation =
            endRotation;
    }

    public void ResetTrigger()
    {
        completed = false;
        investigating = false;
        holdTimer = 0f;

        Debug.Log(
            gameObject.name +
            " has been reset and can be investigated again."
        );
    }
}