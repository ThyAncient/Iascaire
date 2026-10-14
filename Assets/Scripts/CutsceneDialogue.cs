using System.Collections;
using UnityEngine;
using TMPro;

public class CutsceneDialogue : MonoBehaviour
{
    [Header("Dialogue")]
    public TMP_Text cutsceneText;

    [TextArea(2, 5)]
    public string[] dialogues;

    [Header("Timing")]
    public float dialogueDuration = 3f;
    public float dialogueDelay = 0.5f;
    public float dialogueStartDelay = 0f;

    private Coroutine dialogueCoroutine;

    public void StartDialogue()
    {
        if (dialogueCoroutine != null)
            StopCoroutine(dialogueCoroutine);

        dialogueCoroutine =
            StartCoroutine(DialogueSequence());
    }

    public void StopDialogue()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }

        if (cutsceneText != null)
        {
            cutsceneText.text = "";
            cutsceneText.gameObject.SetActive(false);
        }
    }

    IEnumerator DialogueSequence()
    {
        if (cutsceneText == null)
            yield break;

        if (dialogueStartDelay > 0f)
        {
            yield return new WaitForSeconds(
                dialogueStartDelay
            );
        }

        for (int i = 0; i < dialogues.Length; i++)
        {
            cutsceneText.text = dialogues[i];

            cutsceneText.gameObject.SetActive(true);

            yield return new WaitForSeconds(
                dialogueDuration
            );

            cutsceneText.gameObject.SetActive(false);

            if (i < dialogues.Length - 1)
            {
                yield return new WaitForSeconds(
                    dialogueDelay
                );
            }
        }

        dialogueCoroutine = null;
    }
}