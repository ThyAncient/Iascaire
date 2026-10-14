using System.Collections;
using TMPro;
using UnityEngine;

public class FishingMinigame : MonoBehaviour
{
    [Header("Player")]
    public PlayerCamera playerCameraController;
    public PlayerMovement playerMovementController;
    public SceneCameraScript playerSceneCameraController;

    [Header("Objective")]
    public ObjectiveManager objectiveManager;

    [Header("McKinnely Fishing")]
    public GameObject mckinnelyFishing;
    public GameObject murphFishing;
    public GameObject murphSmoking;

    [Header("Fishing Camera")]
    public FishingCamera fishingCamera;

    [Header("Fishing UI")]
    public GameObject fishingUI;

    [Header("Fishing Bar")]
    public RectTransform fishingBar;
    public RectTransform movingLine;
    public RectTransform greenZone;

    [Header("Fish")]
    public Transform fish;
    public Transform fishOriginalTarget;
    public Transform fishEndTarget;

    [Header("Fish Settings")]
    public int FishesCaught = 5;
    public int reelsPerFish = 5;

    [Header("Line Settings")]
    public float lineSpeed = 400f;

    [Header("Green Zone")]
    public float startingGreenWidth = 180f;
    public float greenShrinkAmount = 20f;
    public float minimumGreenWidth = 60f;

    [Header("Dialogue")]
    public TMP_Text dialogueText;
    public float dialogueDuration = 3f;

    [TextArea(2, 3)]
    public string[] fishingDialogue =
    {
        "You scared of fish, mate?",
        "That's one.",
        "Keep going.",
        "You're getting the hang of it.",
        "One more.",
        "There we go."
    };

    [Header("Sounds")]
    public AudioSource audioSource;

    public AudioClip fishingStartSound;
    public AudioClip successfulHitSound;
    public AudioClip missSound;
    public AudioClip fishCaughtSound;
    public AudioClip finalCatchSound;

    private bool minigameActive = false;
    private bool returningCamera = false;

    private float linePosition;
    private float direction = 1f;

    private int fishesCaught = 0;
    private int currentReels = 0;

    private float barWidth;

    private Collider fishingCollider;


    private void Start()
    {
        if (fishingUI != null)
            fishingUI.SetActive(false);

        if (fish != null)
            fish.gameObject.SetActive(false);

        if (murphFishing != null)
            murphFishing.SetActive(false);

        if (murphSmoking != null)
            murphSmoking.SetActive(false);

        fishingCollider = GetComponent<Collider>();

        ResetFishPositions();
    }


    private void Update()
    {
        if (!minigameActive)
            return;

        MoveLine();

        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckFishingHit();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (minigameActive)
            return;

        if (returningCamera)
            return;

        if (!other.CompareTag("Player"))
            return;

        StartFishing();

        if (mckinnelyFishing != null)
            mckinnelyFishing.SetActive(true);
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!minigameActive &&
            fishingCollider != null)
        {
            fishingCollider.enabled = true;
        }
    }


    // =========================================================
    // START FISHING
    // =========================================================

    private void StartFishing()
    {
        minigameActive = true;

        fishesCaught = 0;
        currentReels = 0;

        direction = 1f;
        linePosition = 0f;

        if (fishingBar != null)
            barWidth = fishingBar.rect.width;

        SetupGreenZone();

        if (movingLine != null)
        {
            Vector2 linePos =
                movingLine.anchoredPosition;

            linePos.x = 0f;

            movingLine.anchoredPosition =
                linePos;
        }

        ResetFishPositions();

        if (fish != null)
            fish.gameObject.SetActive(true);

        if (fishingUI != null)
            fishingUI.SetActive(true);

        if (murphFishing != null)
            murphFishing.SetActive(true);

        if (murphSmoking != null)
            murphSmoking.SetActive(false);

        PlaySound(fishingStartSound);

        ShowDialogue(0);

        if (playerCameraController != null)
            playerCameraController.enabled = false;

        if (playerMovementController != null)
            playerMovementController.enabled = false;

        if (playerSceneCameraController != null)
            playerSceneCameraController.enabled = false;

        if (fishingCamera != null)
            fishingCamera.MoveToFishingPosition();

        Debug.Log("Fishing started!");
    }


    // =========================================================
    // MOVE LINE
    // =========================================================

    private void MoveLine()
    {
        if (movingLine == null)
            return;

        linePosition +=
            direction *
            lineSpeed *
            Time.deltaTime;

        float halfBar =
            barWidth / 2f;

        if (linePosition >= halfBar)
        {
            linePosition = halfBar;
            direction = -1f;
        }

        if (linePosition <= -halfBar)
        {
            linePosition = -halfBar;
            direction = 1f;
        }

        Vector2 position =
            movingLine.anchoredPosition;

        position.x = linePosition;

        movingLine.anchoredPosition =
            position;
    }


    // =========================================================
    // CHECK FISHING HIT
    // =========================================================

    private void CheckFishingHit()
    {
        if (movingLine == null ||
            greenZone == null)
            return;

        Vector3[] lineCorners = new Vector3[4];
        Vector3[] greenCorners = new Vector3[4];

        movingLine.GetWorldCorners(lineCorners);
        greenZone.GetWorldCorners(greenCorners);

        float lineLeft = lineCorners[0].x;
        float lineRight = lineCorners[2].x;

        float greenLeft = greenCorners[0].x;
        float greenRight = greenCorners[2].x;

        bool hit =
            lineRight >= greenLeft &&
            lineLeft <= greenRight;

        if (hit)
        {
            SuccessfulReel();
        }
        else
        {
            FailedReel();
        }
    }


    // =========================================================
    // SUCCESSFUL REEL
    // =========================================================

    private void SuccessfulReel()
    {
        currentReels++;

        PlaySound(successfulHitSound);

        MoveCurrentFish();

        Debug.Log(
            "Successful reel: " +
            currentReels +
            "/" +
            reelsPerFish
        );

        if (currentReels >= reelsPerFish)
        {
            CatchFish();
        }
    }


    // =========================================================
    // FAILED REEL
    // =========================================================

    private void FailedReel()
    {
        PlaySound(missSound);

        Debug.Log("Fishing miss!");
    }


    // =========================================================
    // MOVE FISH
    // =========================================================

    private void MoveCurrentFish()
    {
        float progress =
            Mathf.Clamp01(
                (float)currentReels /
                reelsPerFish
            );

        if (fish != null &&
            fishOriginalTarget != null &&
            fishEndTarget != null)
        {
            fish.position =
                Vector3.Lerp(
                    fishOriginalTarget.position,
                    fishEndTarget.position,
                    progress
                );

            fish.rotation =
                Quaternion.Slerp(
                    fishOriginalTarget.rotation,
                    fishEndTarget.rotation,
                    progress
                );
        }
    }


    // =========================================================
    // CATCH FISH
    // =========================================================

    private void CatchFish()
    {
        if (fishesCaught >= FishesCaught - 1)
        {
            CatchFinalFish();
            return;
        }

        fishesCaught++;

        currentReels = 0;

        PlaySound(fishCaughtSound);

        ResetGreenZone();

        linePosition = 0f;
        direction = 1f;

        if (movingLine != null)
        {
            Vector2 linePos =
                movingLine.anchoredPosition;

            linePos.x = 0f;

            movingLine.anchoredPosition =
                linePos;
        }

        if (fish != null &&
            fishOriginalTarget != null)
        {
            fish.position =
                fishOriginalTarget.position;

            fish.rotation =
                fishOriginalTarget.rotation;
        }

        ShowDialogue(fishesCaught);

        Debug.Log(
            "Fish caught: " +
            fishesCaught +
            "/" +
            FishesCaught
        );
    }


    // =========================================================
    // FINAL FISH
    // =========================================================

    private void CatchFinalFish()
    {
        fishesCaught++;

        minigameActive = false;

        PlaySound(finalCatchSound);

        ShowDialogue(fishingDialogue.Length - 1);

        if (fish != null)
            fish.gameObject.SetActive(false);

        Invoke(
            nameof(EndFishing),
            dialogueDuration
        );
    }


    // =========================================================
    // RESET FISH
    // =========================================================

    private void ResetFishPositions()
    {
        if (fish != null &&
            fishOriginalTarget != null)
        {
            fish.position =
                fishOriginalTarget.position;

            fish.rotation =
                fishOriginalTarget.rotation;
        }
    }


    // =========================================================
    // GREEN ZONE
    // =========================================================

    private void SetupGreenZone()
    {
        if (greenZone == null)
            return;

        Vector2 size =
            greenZone.sizeDelta;

        size.x =
            startingGreenWidth;

        greenZone.sizeDelta =
            size;
    }


    private void ResetGreenZone()
    {
        SetupGreenZone();
    }


    // =========================================================
    // DIALOGUE
    // =========================================================

    private void ShowDialogue(int dialogueIndex)
    {
        if (dialogueText == null)
            return;

        if (fishingDialogue == null ||
            fishingDialogue.Length == 0)
            return;

        dialogueIndex =
            Mathf.Clamp(
                dialogueIndex,
                0,
                fishingDialogue.Length - 1
            );

        CancelInvoke(
            nameof(HideDialogue)
        );

        dialogueText.text =
            fishingDialogue[dialogueIndex];

        dialogueText.gameObject.SetActive(true);

        Invoke(
            nameof(HideDialogue),
            dialogueDuration
        );
    }


    private void HideDialogue()
    {
        if (dialogueText != null)
            dialogueText.gameObject.SetActive(false);
    }


    // =========================================================
    // END FISHING
    // =========================================================

    private void EndFishing()
    {
        CancelInvoke(
            nameof(EndFishing)
        );

        CancelInvoke(
            nameof(HideDialogue)
        );

        HideDialogue();

        if (fishingUI != null)
            fishingUI.SetActive(false);

        if (fish != null)
            fish.gameObject.SetActive(false);

        returningCamera = true;

        if (fishingCamera != null)
            fishingCamera.ReturnToPlayer();

        if (playerCameraController != null)
            playerCameraController.enabled = true;

        if (playerMovementController != null)
            playerMovementController.enabled = true;

        if (playerSceneCameraController != null)
            playerSceneCameraController.enabled = true;

        if (fishingCollider != null)
            fishingCollider.enabled = false;

        if (mckinnelyFishing != null)
            mckinnelyFishing.SetActive(false);

        if (murphFishing != null)
            murphFishing.SetActive(false);

        if (murphSmoking != null)
            murphSmoking.SetActive(false);

        returningCamera = false;

        // =====================================================
        // TELL OBJECTIVE MANAGER FISHING IS FINISHED
        // =====================================================

        if (objectiveManager != null)
        {
            objectiveManager.FishingFinished();
        }

        Debug.Log("Fishing complete!");
    }


    // =========================================================
    // SOUND
    // =========================================================

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null ||
            clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }
}