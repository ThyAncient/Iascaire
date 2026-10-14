using System.Collections;
using System.Net.NetworkInformation;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    buttonmanagerscript buttonmanagerscript;
    TextMeshProUGUI text;
    float moveRight = 30f;
    float lerpSpeed = 10f;
    float volume = 0.4f;

    [SerializeField] GameObject buttonswithtitle;
    [SerializeField] GameObject chapterbutton;
    [SerializeField] private AudioSource buttonsfx;
    [SerializeField] private AudioClip buttonsfxClip;
    Vector2 orignalPosition;
    Vector2 targetPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonmanagerscript = FindFirstObjectByType<buttonmanagerscript>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        GameObject buttonObj = GameObject.Find("ButtonSFX");
        buttonsfx = buttonObj.AddComponent<AudioSource>();
        orignalPosition = text.rectTransform.localPosition;
        targetPosition = orignalPosition;
    }

    // Update is called once per frame
    void Update()
    {
            text.transform.localPosition = Vector2.Lerp(text.rectTransform.localPosition, targetPosition, Time.deltaTime * lerpSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetPosition = orignalPosition + new Vector2(moveRight, 0f);
        buttonsfx.PlayOneShot(buttonsfxClip);
        buttonsfx.volume = volume;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetPosition = orignalPosition;
    }

    public void StartGame()
    {
        buttonmanagerscript.StartCameraChapter();
        buttonswithtitle.SetActive(false);
        chapterbutton.SetActive(true);
        ShowButtonGradually(true, chapterbutton);
    }

    public void Settings()
    {

    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Back()
    {
        buttonswithtitle.SetActive(true);
        chapterbutton.SetActive(false);
        buttonmanagerscript.StopCameraChapter();
    }

    public void StartChapter1()
    {
        SceneManager.LoadScene("Chap1Loading");
    }

    IEnumerator ShowButtonGradually(bool isTextAppear, GameObject emptyObject)
    {
        CanvasGroup canvasGroup = emptyObject.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = emptyObject.AddComponent<CanvasGroup>();
        }

        float startAlpha;
        float targetAlpha;

        if (isTextAppear)
        {
            startAlpha = 0f;
            targetAlpha = 1f;
        }
        else
        {
            startAlpha = 1f;
            targetAlpha = 0f;
        }

        canvasGroup.alpha = startAlpha;

        float time = 0f;

        while (time < 2f)
        {
            time += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                time / 2f
            );

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
