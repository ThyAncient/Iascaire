using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast")]
    public Camera playerCamera;
    public float interactDistance = 3f;

    [Header("Interaction UI")]
    public GameObject interactionUI;
    public TMP_Text interactionText;

    [Header("Player Hand")]
    public Transform handPosition;

    [Header("Interaction Sounds")]
    public AudioSource audioSource;

    public AudioClip fishPickupSound;
    public AudioClip fishPlaceSound;
    public AudioClip fishProcessSound;
    public AudioClip mushPickupSound;
    public AudioClip mushPlaceSound;

    [HideInInspector]
    public GameObject heldItem;

    private InteractableObject currentObject;
    private StrangeNoiseTrigger currentNoiseTrigger;

    void Update()
    {
        CheckForObject();

        // Normal InteractableObject interaction
        if (currentObject != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentObject.Interact(this);
            }

            if (Input.GetKey(KeyCode.E))
            {
                currentObject.HoldInteract(this);
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                currentObject.StopHoldInteract(this);
            }
        }
    }

    // =========================================================
    // CHECK WHAT PLAYER IS LOOKING AT
    // =========================================================

    void CheckForObject()
    {
        currentObject = null;
        currentNoiseTrigger = null;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(
            ray,
            out hit,
            interactDistance
        ))
        {
            // =================================================
            // STRANGE NOISE TRIGGER
            // =================================================

            StrangeNoiseTrigger noiseTrigger =
                hit.collider.GetComponent<StrangeNoiseTrigger>();

            if (noiseTrigger != null)
            {
                // Only show the trigger interaction if the
                // Strange Noise objective is currently active.
                if (noiseTrigger.CanInteract())
                {
                    currentNoiseTrigger = noiseTrigger;

                    if (interactionUI != null)
                        interactionUI.SetActive(true);

                    if (interactionText != null)
                    {
                        interactionText.text =
                            "[Hold E]";
                    }

                    return;
                }
            }

            // =================================================
            // NORMAL INTERACTABLE OBJECT
            // =================================================

            InteractableObject interactable =
                hit.collider.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                currentObject = interactable;

                if (interactionUI != null)
                    interactionUI.SetActive(true);

                if (interactionText != null)
                {
                    interactionText.text =
                        interactable.GetInteractionText(this);
                }

                return;
            }
        }

        // Nothing found
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    // =========================================================
    // SOUND
    // =========================================================

    public void PlaySound(AudioClip clip)
    {
        if (audioSource == null)
            return;

        if (clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }

    // =========================================================
    // PICK UP ITEM
    // =========================================================

    public void PickUpItem(GameObject itemPrefab)
    {
        PickUpItem(
            itemPrefab,
            Vector3.zero,
            Vector3.one
        );
    }

    public void PickUpItem(
        GameObject itemPrefab,
        Vector3 rotationOffset,
        Vector3 scaleMultiplier
    )
    {
        if (heldItem != null)
            return;

        heldItem = Instantiate(
            itemPrefab,
            handPosition
        );

        heldItem.transform.localPosition =
            Vector3.zero;

        heldItem.transform.localRotation =
            Quaternion.Euler(rotationOffset);

        heldItem.transform.localScale =
            Vector3.Scale(
                heldItem.transform.localScale,
                scaleMultiplier
            );

        Rigidbody rb =
            heldItem.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Collider[] colliders =
            heldItem.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
    }

    // =========================================================
    // DROP ITEM
    // =========================================================

    public void DropHeldItem()
    {
        if (heldItem == null)
            return;

        Destroy(heldItem);
        heldItem = null;
    }

    // =========================================================
    // HOLDING ITEM
    // =========================================================

    public bool IsHoldingItem()
    {
        return heldItem != null;
    }

    // =========================================================
    // LOOKING AT OBJECT
    // =========================================================

    public bool IsLookingAtObject(GameObject target)
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(
            ray,
            out hit,
            interactDistance
        ))
        {
            return hit.collider.gameObject == target;
        }

        return false;
    }
}