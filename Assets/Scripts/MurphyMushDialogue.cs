using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class MurphMushTrigger : MonoBehaviour
{
    [Header("References")]
    public InteractableObject mushBarrel;
    public TMP_Text dialogueText;

    [Header("Player")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (mushBarrel == null || dialogueText == null)
            return;

        int mush = mushBarrel.currentMush;

        if (mush == 0)
        {
            StartCoroutine(PlayText("We don't have any mush yet."));
        }
        else if (mush <= 2)
        {
            StartCoroutine(PlayText("That's not much..."));
        }
        else if (mush <= 4)
        {
            StartCoroutine(PlayText("We're almost done."));
        }
        else
        {
            StartCoroutine(PlayText("Thats enough, lets go take a smoke"));
        }
    }

    IEnumerator PlayText(string text)
    {
        dialogueText.text = text;
        yield return new WaitForSeconds(2.5f);
        dialogueText.text = " ";
    }
}