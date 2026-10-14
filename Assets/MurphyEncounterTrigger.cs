using UnityEngine;
using System.Collections;
public class MurphyEncounterTrigger : MonoBehaviour
{
    [Header("Sequence")]
    public StrangeNoiseSequence strangeNoiseSequence;
    public GameObject monster;
    private bool triggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        if (strangeNoiseSequence != null)
        {
            strangeNoiseSequence.StartFinalEncounter();
            monster.SetActive(true);
        }
    }
}