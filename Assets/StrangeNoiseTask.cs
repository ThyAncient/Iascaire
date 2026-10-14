using UnityEngine;

public class StrangeNoiseTask : MonoBehaviour
{
    [Header("Noise Triggers")]
    public StrangeNoiseTrigger triggerOne;
    public StrangeNoiseTrigger triggerTwo;

    [Header("Objective")]
    public ObjectiveManager objectiveManager;

    [Header("Noise Sequence")]
    public StrangeNoiseSequence sequence;

    private bool taskStarted = false;

    private bool triggerOneDone = false;
    private bool triggerTwoDone = false;

    private void Start()
    {
        // Make sure both triggers start hidden.
        if (triggerOne != null)
            triggerOne.gameObject.SetActive(false);

        if (triggerTwo != null)
            triggerTwo.gameObject.SetActive(false);
    }

    public void StartTask()
    {
        if (taskStarted)
            return;

        taskStarted = true;

        triggerOneDone = false;
        triggerTwoDone = false;

        if (triggerOne != null)
            triggerOne.gameObject.SetActive(false);

        if (triggerTwo != null)
            triggerTwo.gameObject.SetActive(false);

        // First trigger becomes active.
        if (triggerOne != null)
            triggerOne.gameObject.SetActive(true);

        Debug.Log(
            "Strange noise task started. Trigger 1 enabled."
        );
    }

    public void TriggerInvestigated(
        StrangeNoiseTrigger trigger
    )
    {
        if (!taskStarted)
            return;

        // -------------------------
        // TRIGGER ONE
        // -------------------------

        if (trigger == triggerOne)
        {
            // Prevent Trigger 1 from being completed twice.
            if (triggerOneDone)
                return;

            triggerOneDone = true;

            // Hide Trigger 1.
            triggerOne.gameObject.SetActive(false);

            // Enable Trigger 2.
            if (triggerTwo != null)
                triggerTwo.gameObject.SetActive(true);

            Debug.Log(
                "Trigger 1 completed. Trigger 2 enabled."
            );

            return;
        }

        // -------------------------
        // TRIGGER TWO
        // -------------------------

        if (trigger == triggerTwo)
        {
            // Make sure Trigger 1 was completed first.
            if (!triggerOneDone)
                return;

            // Prevent duplicate completion.
            if (triggerTwoDone)
                return;

            triggerTwoDone = true;

            // Hide Trigger 2 while Murphy sequence happens.
            triggerTwo.gameObject.SetActive(false);

            Debug.Log(
                "Trigger 2 completed."
            );

            // Tell the sequence controller.
            if (sequence != null)
            {
                sequence.SecondTriggerCompleted();
            }

            return;
        }
    }

    // Called after Murphy #1 disappears
    // and the 30 second timer finishes.
    public void ReactivateSecondTrigger()
    {
        if (!taskStarted)
            return;

        // Trigger 1 stays completed.
        triggerOneDone = true;

        // Reset Trigger 2's task state.
        triggerTwoDone = false;

        // Reset the actual StrangeNoiseTrigger.
        if (triggerTwo != null)
        {
            triggerTwo.ResetTrigger();
            triggerTwo.gameObject.SetActive(true);
        }

        // Make sure Trigger 1 stays hidden.
        if (triggerOne != null)
            triggerOne.gameObject.SetActive(false);

        Debug.Log(
            "Trigger 2 has been reset and is active again."
        );
    }

    public bool IsTriggerActive(
        StrangeNoiseTrigger trigger
    )
    {
        if (!taskStarted)
            return false;

        // Trigger 1 is only valid during the first part
        // of the sequence.
        if (trigger == triggerOne)
        {
            return !triggerOneDone;
        }

        // Trigger 2 is valid:
        // - after Trigger 1 is complete
        // - while Trigger 2 has not been completed
        if (trigger == triggerTwo)
        {
            return triggerOneDone && !triggerTwoDone;
        }

        return false;
    }
}