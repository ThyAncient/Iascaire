using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectiveManager : MonoBehaviour
{
    public enum Objective
    {
        MakeMush,
        FishWithMurph,
        InvestigateNoises
    }

    [Header("Objective UI")]
    public TMP_Text objectiveText;

    [Header("Current Objective")]
    public Objective currentObjective = Objective.MakeMush;

    [Header("Strange Noise Task")]
    public StrangeNoiseTask strangeNoiseTask;

    [Header("Settings")]
    public float fadeSpeed = 3f;
    public float completedDisplayTime = 1.5f;

    private Coroutine fadeCoroutine;

    private void Start()
    {
        HideObjectives();
    }

    // =========================================================
    // SHOW OBJECTIVES
    // =========================================================

    public void ShowObjectives()
    {
        if (objectiveText == null)
            return;

        UpdateObjectiveText();

        Color color = objectiveText.color;
        color.a = 0f;
        objectiveText.color = color;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeText(1f));
    }

    // =========================================================
    // HIDE OBJECTIVES
    // =========================================================

    public void HideObjectives()
    {
        if (objectiveText == null)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        Color color = objectiveText.color;
        color.a = 0f;
        objectiveText.color = color;
    }

    // =========================================================
    // UPDATE OBJECTIVE TEXT
    // =========================================================

    private void UpdateObjectiveText()
    {
        if (objectiveText == null)
            return;

        switch (currentObjective)
        {
            case Objective.MakeMush:

                SetObjectiveText(
                    "Make mush from the fish"
                );

                break;

            case Objective.FishWithMurph:

                SetObjectiveText(
                    "Fish with Murph"
                );

                break;

            case Objective.InvestigateNoises:

                SetObjectiveText(
                    "Investigate the strange noises around the ship"
                );

                // Tell the noise task that it is now active.
                if (strangeNoiseTask != null)
                {
                    strangeNoiseTask.StartTask();
                }

                break;
        }
    }

    private void SetObjectiveText(string text)
    {
        objectiveText.text = text;
    }

    // =========================================================
    // MUSH FINISHED
    // =========================================================

    public void MushFinished()
    {
        if (currentObjective != Objective.MakeMush)
            return;

        currentObjective = Objective.FishWithMurph;

        ShowCompletedAndChangeObjective(
            "Make mush from the fish"
        );
    }

    // =========================================================
    // FISHING FINISHED
    // =========================================================

    public void FishingFinished()
    {
        if (currentObjective != Objective.FishWithMurph)
            return;

        currentObjective = Objective.InvestigateNoises;

        ShowCompletedAndChangeObjective(
            "Fish with Murph"
        );
    }

    // =========================================================
    // NOISE INVESTIGATION FINISHED
    // =========================================================

    public void NoisesFinished()
    {
        if (currentObjective != Objective.InvestigateNoises)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(
            CompleteNoiseObjectiveRoutine()
        );
    }

    private IEnumerator CompleteNoiseObjectiveRoutine()
    {
        if (objectiveText == null)
            yield break;

        objectiveText.text =
            "Investigate the strange noises around the ship";

        Color color = objectiveText.color;
        color.a = 1f;
        objectiveText.color = color;

        yield return new WaitForSeconds(
            completedDisplayTime
        );

        yield return StartCoroutine(
            FadeText(0f)
        );
    }

    // =========================================================
    // COMPLETE OBJECTIVE AND CHANGE TO NEXT
    // =========================================================

    private void ShowCompletedAndChangeObjective(
        string completedText
    )
    {
        if (objectiveText == null)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(
            CompleteObjectiveRoutine(completedText)
        );
    }

    private IEnumerator CompleteObjectiveRoutine(
        string completedText
    )
    {
        objectiveText.text = "✓ " + completedText;

        yield return new WaitForSeconds(
            completedDisplayTime
        );

        yield return StartCoroutine(
            FadeText(0f)
        );

        UpdateObjectiveText();

        yield return StartCoroutine(
            FadeText(1f)
        );
    }

    // =========================================================
    // FADE
    // =========================================================

    private IEnumerator FadeText(float targetAlpha)
    {
        if (objectiveText == null)
            yield break;

        Color color = objectiveText.color;

        while (!Mathf.Approximately(
            color.a,
            targetAlpha
        ))
        {
            color.a = Mathf.MoveTowards(
                color.a,
                targetAlpha,
                fadeSpeed * Time.deltaTime
            );

            objectiveText.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        objectiveText.color = color;
    }
}