using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StrangeNoiseSequence : MonoBehaviour
{
    [Header("References")]
    public StrangeNoiseTask strangeNoiseTask;

    [Header("Player")]
    public Transform player;
    public PlayerMovement playerMovement;
    public PlayerCamera playerCamera;
    public SceneCameraScript playerSceneCamera;

    [Header("Camera")]
    public Camera mainCamera;

    [Header("Murphy #1")]
    public GameObject murphyOne;
    public Animator murphyOneAnimator;
    public Transform murphyOneLookPoint;

    [Header("Murphy #1 Animation")]
    public string murphyOneBoolName = "IsScaring";
    public float murphyOneAnimationDuration = 3f;

    [Header("After Murphy #1")]
    public float waitTime = 30f;

    [Header("Murphy #2")]
    public GameObject murphyTwo;
    public Animator murphyTwoAnimator;
    public Transform murphyTwoLookPoint;

    [Header("Murphy #2 Animation")]
    public string murphyTwoBoolName = "IsScaring";
    public float murphyTwoAnimationDuration = 3f;

    [Header("Final Encounter")]
    public GameObject finalEncounterTrigger;

    [Header("Monster")]
    public GameObject monster;
    public float monsterSpeed = 15f;

    [Header("Final Camera")]
    public Transform lookBehindTarget;
    public float cameraLookTime = 1f;

    [Header("Black Screen")]
    public CanvasGroup blackScreen;
    public float blackScreenFadeTime = 3f;

    [Header("Title")]
    public TMP_Text gameTitle;
    public float titleFadeTime = 3f;

    [Header("Credits")]
    public TMP_Text creditsText;

    [Header("Menu")]
    public string menuSceneName = "Menu";

    [Header("Strange Noise Sound")]
    public AudioSource strangeNoiseSource;

    [Header("Murphy Look Sound")]
    public AudioSource murphyLookSoundSource;

    [Header("Music")]
    public AudioSource musicOne;
    public AudioSource musicTwo;

    [Header("Music Volume")]
    public float normalMusicVolume = 0.5f;
    public float loudMusicVolume = 1f;
    public float musicFadeTime = 3f;

    [Header("Monster Sound")]
    public AudioSource monsterSoundSource;

    private int secondTriggerCount = 0;
    private bool sequenceRunning = false;
    private bool finalSequenceStarted = false;

    private void Start()
    {
        // Murphy starts hidden.
        if (murphyOne != null)
            murphyOne.SetActive(false);

        if (murphyTwo != null)
            murphyTwo.SetActive(false);

        // Final trigger starts hidden.
        if (finalEncounterTrigger != null)
            finalEncounterTrigger.SetActive(false);

        // Monster starts hidden.
        if (monster != null)
            monster.SetActive(false);

        // Reset Murphy animations.
        ResetMurphyAnimator(
            murphyOneAnimator,
            murphyOneBoolName
        );

        ResetMurphyAnimator(
            murphyTwoAnimator,
            murphyTwoBoolName
        );

        // Strange sound starts OFF.
        if (strangeNoiseSource != null)
        {
            strangeNoiseSource.Stop();
        }

        // Murphy look sound starts OFF.
        if (murphyLookSoundSource != null)
        {
            murphyLookSoundSource.Stop();
        }

        // Monster sound starts OFF.
        if (monsterSoundSource != null)
        {
            monsterSoundSource.Stop();
        }

        // Set normal music volume.
        if (musicOne != null)
            musicOne.volume = normalMusicVolume;

        if (musicTwo != null)
            musicTwo.volume = normalMusicVolume;

        // Black screen starts invisible.
        if (blackScreen != null)
        {
            blackScreen.alpha = 0f;
            blackScreen.interactable = false;
            blackScreen.blocksRaycasts = false;
        }

        // Title starts invisible.
        if (gameTitle != null)
        {
            Color color = gameTitle.color;
            color.a = 0f;
            gameTitle.color = color;
        }

        // Credits start invisible.
        if (creditsText != null)
        {
            Color color = creditsText.color;
            color.a = 0f;
            creditsText.color = color;
        }
    }

    public void SecondTriggerCompleted()
    {
        if (sequenceRunning)
            return;

        secondTriggerCount++;

        Debug.Log(
            "Second trigger completed. Count: " +
            secondTriggerCount
        );

        // First time.
        if (secondTriggerCount == 1)
        {
            StartCoroutine(
                FirstMurphySequence()
            );
        }

        // Second time.
        else if (secondTriggerCount == 2)
        {
            StartCoroutine(
                SecondMurphySequence()
            );
        }
    }

    // ============================================================
    // MURPHY #1
    // ============================================================

    IEnumerator FirstMurphySequence()
    {
        sequenceRunning = true;

        Debug.Log(
            "Murphy #1 appearing."
        );

        // Start strange sound.
        PlayStrangeNoise();

        // Make sure animation is OFF.
        ResetMurphyAnimator(
            murphyOneAnimator,
            murphyOneBoolName
        );

        if (murphyOne != null)
            murphyOne.SetActive(true);

        // Wait until player looks at Murphy.
        yield return StartCoroutine(
            WaitForPlayerToLookAtMurphy(
                murphyOne,
                murphyOneLookPoint,
                murphyOneAnimator,
                murphyOneBoolName,
                murphyOneAnimationDuration
            )
        );

        // Hide Murphy.
        if (murphyOne != null)
            murphyOne.SetActive(false);

        Debug.Log(
            "Murphy #1 disappeared."
        );

        Debug.Log(
            "Waiting " +
            waitTime +
            " seconds before reactivating trigger."
        );

        // Make music louder during the 30 second wait.
        yield return StartCoroutine(
            FadeMusicVolume(
                loudMusicVolume
            )
        );

        // Wait 30 seconds.
        yield return new WaitForSeconds(
            waitTime
        );

        // Return music to normal.
        yield return StartCoroutine(
            FadeMusicVolume(
                normalMusicVolume
            )
        );

        // Stop strange sound.
        StopStrangeNoise();

        // Reactivate second trigger.
        if (strangeNoiseTask != null)
        {
            strangeNoiseTask.ReactivateSecondTrigger();
        }

        sequenceRunning = false;

        Debug.Log(
            "Second trigger is available again."
        );
    }

    // ============================================================
    // MURPHY #2
    // ============================================================

    IEnumerator SecondMurphySequence()
    {
        sequenceRunning = true;

        Debug.Log(
            "Murphy #2 appearing."
        );

        // Start the SAME strange sound again.
        PlayStrangeNoise();

        // Make sure animation is OFF.
        ResetMurphyAnimator(
            murphyTwoAnimator,
            murphyTwoBoolName
        );

        if (murphyTwo != null)
            murphyTwo.SetActive(true);

        // Wait until player looks at Murphy.
        yield return StartCoroutine(
            WaitForPlayerToLookAtMurphy(
                murphyTwo,
                murphyTwoLookPoint,
                murphyTwoAnimator,
                murphyTwoBoolName,
                murphyTwoAnimationDuration
            )
        );

        // Hide Murphy.
        if (murphyTwo != null)
            murphyTwo.SetActive(false);

        Debug.Log(
            "Murphy #2 disappeared."
        );

        // Stop strange sound when second sequence ends.
        StopStrangeNoise();

        // Enable final encounter trigger.
        if (finalEncounterTrigger != null)
        {
            finalEncounterTrigger.SetActive(true);
        }

        sequenceRunning = false;

        Debug.Log(
            "Final encounter trigger enabled."
        );
    }

    // ============================================================
    // MURPHY LOOK DETECTION
    // ============================================================

    IEnumerator WaitForPlayerToLookAtMurphy(
        GameObject murphy,
        Transform lookPoint,
        Animator animator,
        string boolName,
        float animationDuration
    )
    {
        if (murphy == null)
            yield break;

        Debug.Log(
            "Waiting for player to look at Murphy."
        );

        while (true)
        {
            if (player == null ||
                mainCamera == null)
            {
                yield return null;
                continue;
            }

            Transform target =
                lookPoint != null
                    ? lookPoint
                    : murphy.transform;

            Vector3 direction =
                target.position -
                mainCamera.transform.position;

            direction.Normalize();

            float dot =
                Vector3.Dot(
                    mainCamera.transform.forward,
                    direction
                );

            // Player must look fairly directly at Murphy.
            if (dot > 0.85f)
            {
                break;
            }

            yield return null;
        }

        Debug.Log(
            "Player looked at Murphy!"
        );

        // Play look sound.
        if (murphyLookSoundSource != null)
        {
            murphyLookSoundSource.Play();
        }

        // Start animation.
        if (animator != null)
        {
            animator.SetBool(
                boolName,
                true
            );
        }

        Debug.Log(
            "Murphy animation started."
        );

        // Wait for animation.
        yield return new WaitForSeconds(
            animationDuration
        );

        // Turn animation back off.
        if (animator != null)
        {
            animator.SetBool(
                boolName,
                false
            );
        }
    }

    // ============================================================
    // STRANGE NOISE
    // ============================================================

    void PlayStrangeNoise()
    {
        if (strangeNoiseSource == null)
            return;

        strangeNoiseSource.Play();

        Debug.Log(
            "Strange noise started."
        );
    }

    void StopStrangeNoise()
    {
        if (strangeNoiseSource == null)
            return;

        strangeNoiseSource.Stop();

        Debug.Log(
            "Strange noise stopped."
        );
    }

    // ============================================================
    // MUSIC
    // ============================================================

    IEnumerator FadeMusicVolume(
        float targetVolume
    )
    {
        float startVolume = normalMusicVolume;

        if (musicOne != null)
            startVolume = musicOne.volume;
        else if (musicTwo != null)
            startVolume = musicTwo.volume;

        float elapsed = 0f;

        while (elapsed < musicFadeTime)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / musicFadeTime
                );

            float volume =
                Mathf.Lerp(
                    startVolume,
                    targetVolume,
                    t
                );

            if (musicOne != null)
                musicOne.volume = volume;

            if (musicTwo != null)
                musicTwo.volume = volume;

            yield return null;
        }

        if (musicOne != null)
            musicOne.volume = targetVolume;

        if (musicTwo != null)
            musicTwo.volume = targetVolume;
    }

    // ============================================================
    // MURPHY ANIMATION RESET
    // ============================================================

    void ResetMurphyAnimator(
        Animator animator,
        string boolName
    )
    {
        if (animator == null)
            return;

        animator.SetBool(
            boolName,
            false
        );
    }

    // ============================================================
    // FINAL ENCOUNTER
    // ============================================================

    public void StartFinalEncounter()
    {
        if (finalSequenceStarted)
            return;

        finalSequenceStarted = true;

        if (finalEncounterTrigger != null)
            finalEncounterTrigger.SetActive(false);

        StartCoroutine(
            FinalEncounterSequence()
        );
    }

    IEnumerator FinalEncounterSequence()
    {
        Debug.Log("FINAL ENCOUNTER STARTED.");

        // =========================================================
        // STOP ALL PLAYER CAMERA / MOVEMENT CONTROL
        // =========================================================

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (playerCamera != null)
        {
            playerCamera.enabled = false;
        }

        if (playerSceneCamera != null)
        {
            playerSceneCamera.enabled = false;
        }

        Debug.Log("Player movement and camera control disabled.");

        // =========================================================
        // MOVE CAMERA TO LOOK BEHIND TARGET
        // =========================================================

        if (lookBehindTarget != null)
        {
            Vector3 startPosition =
                mainCamera.transform.position;

            Quaternion startRotation =
                mainCamera.transform.rotation;

            Vector3 targetPosition =
                lookBehindTarget.position;

            Quaternion targetRotation =
                lookBehindTarget.rotation;

            yield return StartCoroutine(
                MoveCameraToTarget(
                    startPosition,
                    targetPosition,
                    startRotation,
                    targetRotation
                )
            );
        }

        // =========================================================
        // MONSTER
        // =========================================================

        if (monster != null)
        {
            monster.SetActive(true);

            Debug.Log(
                "Monster appeared. Waiting before attack."
            );

            // Start monster movement.
            StartCoroutine(
                MoveMonsterToLookTarget()
            );
        }

        // =========================================================
        // BLACK SCREEN
        // =========================================================

        if (blackScreen != null)
        {
            yield return StartCoroutine(
                FadeBlackScreen(1f)
            );
        }

        // =========================================================
        // TITLE
        // =========================================================

        if (gameTitle != null)
        {
            yield return StartCoroutine(
                FadeText(
                    gameTitle,
                    1f,
                    titleFadeTime
                )
            );
        }

        yield return new WaitForSeconds(2f);

        // =========================================================
        // CREDITS
        // =========================================================

        if (creditsText != null)
        {
            yield return StartCoroutine(
                FadeText(
                    creditsText,
                    1f,
                    titleFadeTime
                )
            );
        }

        // =========================================================
        // RETURN TO MENU
        // =========================================================

        yield return new WaitForSeconds(10f);

        if (!string.IsNullOrEmpty(menuSceneName))
        {
            SceneManager.LoadScene(
                menuSceneName
            );
        }
    }

    // ============================================================
    // MONSTER MOVEMENT
    // ============================================================

    private bool monsterMovementStarted = false;

    IEnumerator MoveMonsterToLookTarget()
    {
        if (monster == null ||
            lookBehindTarget == null)
            yield break;

        // Prevent this coroutine from starting twice.
        if (monsterMovementStarted)
            yield break;

        monsterMovementStarted = true;

        Debug.Log(
            "Monster continuously moving toward Look Behind Target."
        );

        // Play monster sound ONCE.
        if (monsterSoundSource != null)
        {
            monsterSoundSource.Play();
        }

        while (
            monster != null &&
            lookBehindTarget != null &&
            finalSequenceStarted
        )
        {
            Vector3 targetLocalPosition = lookBehindTarget.localPosition;

            Vector3 currentPosition = monster.transform.localPosition;

            currentPosition.z = Mathf.MoveTowards(
                currentPosition.z,
                targetLocalPosition.z,
                monsterSpeed * Time.deltaTime
            );

            monster.transform.localPosition = currentPosition;

            yield return null;
        }
    }

    // ============================================================
    // CAMERA
    // ============================================================

    IEnumerator MoveCameraToTarget(
        Vector3 startPosition,
        Vector3 targetPosition,
        Quaternion startRotation,
        Quaternion targetRotation
    )
    {
        float elapsed = 0f;

        while (elapsed < cameraLookTime)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed /
                    cameraLookTime
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            mainCamera.transform.position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );

            mainCamera.transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );

            yield return null;
        }

        mainCamera.transform.position =
            targetPosition;

        mainCamera.transform.rotation =
            targetRotation;
    }

    IEnumerator RotateCameraTo(
        Quaternion targetRotation
    )
    {
        Quaternion startRotation =
            mainCamera.transform.rotation;

        float elapsed = 0f;

        while (elapsed < cameraLookTime)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed /
                    cameraLookTime
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            mainCamera.transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );

            yield return null;
        }

        mainCamera.transform.rotation =
            targetRotation;
    }

    // ============================================================
    // BLACK SCREEN
    // ============================================================

    IEnumerator FadeBlackScreen(
        float targetAlpha
    )
    {
        float startAlpha =
            blackScreen.alpha;

        float elapsed = 0f;

        while (
            elapsed <
            blackScreenFadeTime
        )
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed /
                    blackScreenFadeTime
                );

            blackScreen.alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );

            yield return null;
        }

        blackScreen.alpha =
            targetAlpha;
    }

    // ============================================================
    // TEXT
    // ============================================================

    IEnumerator FadeText(
        TMP_Text text,
        float targetAlpha,
        float duration
    )
    {
        if (text == null)
            yield break;

        Color color =
            text.color;

        float startAlpha =
            color.a;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed /
                    duration
                );

            color.a =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );

            text.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        text.color = color;
    }
}