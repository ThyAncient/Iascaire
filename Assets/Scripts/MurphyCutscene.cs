using System.Collections;
using UnityEngine;

public class MurphyCutscene : MonoBehaviour
{
    [Header("Cutscene McKinnely")]
    public GameObject mckinnely;

    [Header("Murphy")]
    public GameObject murphySmoking;

    [Header("Smoking Trigger")]
    public GameObject murphySmokingTrigger;

    [Header("Player Scripts")]
    public PlayerCamera playerCameraController;
    public PlayerMovement playerMovementController;
    public SceneCameraScript playerSceneCameraController;

    [Header("Cutscene Systems")]
    public CutsceneCamera cutsceneCamera;
    public CutsceneDialogue cutsceneDialogue;
    public CutsceneEffects cutsceneEffects;

    [Header("Cutscene")]
    public float cutsceneDuration = 10f;

    private bool cutsceneStarted = false;

    private void Start()
    {
        // Keep the cutscene version of McKinnely hidden
        if (mckinnely != null)
            mckinnely.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cutsceneStarted)
            return;

        if (!other.CompareTag("Player"))
            return;

        StartCutscene();
    }

    void StartCutscene()
    {
        cutsceneStarted = true;

        Debug.Log("Murphy cutscene started.");

        // Turn on the separate McKinnely NPC
        if (mckinnely != null)
            mckinnely.SetActive(true);

        // Disable player control
        if (playerCameraController != null)
            playerCameraController.enabled = false;

        if (playerMovementController != null)
            playerMovementController.enabled = false;

        if (playerSceneCameraController != null)
            playerSceneCameraController.enabled = false;

        // Start camera
        if (cutsceneCamera != null)
            cutsceneCamera.StartCamera();

        // Start dialogue
        if (cutsceneDialogue != null)
            cutsceneDialogue.StartDialogue();

        // Start effects
        if (cutsceneEffects != null)
            cutsceneEffects.StartEffects();

        StartCoroutine(EndCutscene());
    }

    IEnumerator EndCutscene()
    {
        yield return new WaitForSeconds(cutsceneDuration);

        Debug.Log("Murphy cutscene ended.");

        // Stop camera
        if (cutsceneCamera != null)
            cutsceneCamera.StopCamera();

        // Stop dialogue
        if (cutsceneDialogue != null)
            cutsceneDialogue.StopDialogue();

        // Stop effects
        if (cutsceneEffects != null)
            cutsceneEffects.StopEffects();

        // Hide the cutscene McKinnely
        if (mckinnely != null)
            mckinnely.SetActive(false);

        murphySmokingTrigger.SetActive(false);
        murphySmoking.SetActive(false);

        // Restore player control
        if (playerCameraController != null)
            playerCameraController.enabled = true;

        if (playerMovementController != null)
            playerMovementController.enabled = true;

        if (playerSceneCameraController != null)
            playerSceneCameraController.enabled = true;

        cutsceneStarted = false;
    }
}