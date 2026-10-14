using UnityEngine;

public class MurphyFishingCutscene : MonoBehaviour
{
    [Header("Mush Barrel")]
    public InteractableObject mushBarrel;

    public int requiredMush = 5;

    [Header("Fishing Trigger")]
    public Collider fishingTrigger;

    [Header("Smoking Trigger")]
    public Collider murphySmokingTrigger;

    private bool taskFinished = false;

    private void Start()
    {
        // Fishing is unavailable at the beginning
        if (fishingTrigger != null)
            fishingTrigger.enabled = false;

        // Smoking is also unavailable at the beginning
        if (murphySmokingTrigger != null)
            murphySmokingTrigger.enabled = false;
    }

    private void Update()
    {
        if (taskFinished)
            return;

        if (mushBarrel == null)
            return;

        if (mushBarrel.currentMush >= requiredMush)
        {
            FinishMushTask();
        }
    }

    void FinishMushTask()
    {
        taskFinished = true;

        Debug.Log(
            "Mush task complete! Fishing is now available."
        );

        // Enable fishing
        if (fishingTrigger != null)
            fishingTrigger.enabled = true;
    }
}