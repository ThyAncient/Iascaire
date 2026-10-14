using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LoadingScreenScript : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private TMP_Text quoteText;
    [SerializeField] private TMP_Text quoteeText;

    [Header("Loading")]
    [SerializeField] private float fadeTime = 2f;
    [SerializeField] private float quoteDelay = 0.5f;
    [SerializeField] private float quoteDisplayTime = 1f;

    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip anticipationSFX;
    [SerializeField] private float sfxVolume = 1f;

    [Header("Scene")]
    [SerializeField] private string sceneToLoad;

    private bool videoFinished = false;

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }

        StartCoroutine(LoadingSequence());
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    void OnVideoFinished(VideoPlayer player)
    {
        videoFinished = true;
    }
    IEnumerator LoadingSequence()
    {
        // Start loading the next scene in the background
        AsyncOperation loadingScene =
            SceneManager.LoadSceneAsync(sceneToLoad);

        loadingScene.allowSceneActivation = false;

        // Hide quote and author
        SetAlpha(quoteText, 0f);
        SetAlpha(quoteeText, 0f);

        // Keep black screen visible initially
        SetAlpha(blackScreen, 1f);

        // Fade out black screen
        yield return StartCoroutine(
            Fade(blackScreen, 1f, 0f)
        );

        yield return new WaitForSeconds(quoteDelay);

        // Show quote and author
        StartCoroutine(Fade(quoteText, 0f, 1f));

        yield return StartCoroutine(
            Fade(quoteeText, 0f, 1f)
        );

        // Wait until the scene is loaded and ready
        while (loadingScene.progress < 0.9f)
        {
            yield return null;
        }
        // Ensure the quote stays visible for its minimum duration
        yield return new WaitForSeconds(quoteDisplayTime);
        // Play dramatic sound
        if (audioSource != null && anticipationSFX != null)
        {
            audioSource.volume = sfxVolume;
            audioSource.PlayOneShot(anticipationSFX);
        }
        
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(Fade(blackScreen, 1f, 1f));
        // Switch scenes immediately after everything is ready
        loadingScene.allowSceneActivation = true;
    }

    IEnumerator Fade(
        GameObject objectToFade,
        float startAlpha,
        float targetAlpha)
    {
        CanvasGroup canvasGroup =
            objectToFade.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup =
                objectToFade.AddComponent<CanvasGroup>();
        }

        float time = 0f;
        canvasGroup.alpha = startAlpha;

        while (time < fadeTime)
        {
            time += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                time / fadeTime
            );

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    IEnumerator Fade(
        TMP_Text text,
        float startAlpha,
        float targetAlpha)
    {
        float time = 0f;
        Color color = text.color;

        color.a = startAlpha;
        text.color = color;

        while (time < fadeTime)
        {
            time += Time.deltaTime;

            color.a = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                time / fadeTime
            );

            text.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        text.color = color;
    }

    void SetAlpha(TMP_Text text, float alpha)
    {
        if (text == null) return;

        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }

    void SetAlpha(GameObject obj, float alpha)
    {
        if (obj == null) return;

        CanvasGroup canvasGroup =
            obj.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = alpha;
    }
}