using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public enum InteractionType
    {
        FishBarrel,
        FishTable,
        Mush,
        MushBarrel
    }

    [Header("Interaction")]
    public InteractionType interactionType;
    [Header("FishBarrel")]
    public CapsuleCollider fishbarrel;
    public GameObject fishbarrelNoScript;
    [Header("Objective")]
    public ObjectiveManager objectiveManager;

    [Header("Fish Barrel")]
    public GameObject fishPrefab;

    [Header("Fish Table")]
    public GameObject mushPrefab;

    public Vector3 fishPlacementOffset;
    public Vector3 mushPlacementOffset;

    [Header("Fish Rotation")]
    public Vector3 fishRotationOffset;

    [Header("Fish Scale Offset")]
    public float fishScaleOffsetX = 0f;
    public float fishScaleOffsetY = 0f;
    public float fishScaleOffsetZ = 0f;

    [Header("Mush Rotation")]
    public Vector3 mushRotationOffset;

    [Header("Mush Scale Offset")]
    public float mushScaleOffsetX = 0f;
    public float mushScaleOffsetY = 0f;
    public float mushScaleOffsetZ = 0f;

    [Header("Mush Hand")]
    public Vector3 mushHandRotationOffset;
    public Vector3 mushHandScaleOffset = Vector3.one;

    [Header("Processing")]
    public float holdTime = 1f;

    [Header("Mush Barrel")]
    public int maxMush = 5;
    public int currentMush = 0;

    [Header("Mush Barrel Fill")]
    public Transform mushFillCylinder;
    public float fillHeight = 1f;
    public float fillStartY = 0f;

    private float holdTimer = 0f;

    private GameObject placedFish;
    private GameObject placedMush;

    [HideInInspector]
    public InteractableObject parentTable;
    private void Update()
    {

        if (currentMush == 5)
        {
            fishbarrel.enabled = false;
            fishbarrelNoScript.SetActive(true);
        }
    }
    public string GetInteractionText(PlayerInteraction player)
    {
        switch (interactionType)
        {
            case InteractionType.FishBarrel:

                if (player.IsHoldingItem())
                    return "?";

                return "[E]";

            case InteractionType.FishTable:

                if (player.IsHoldingItem() &&
                    placedFish == null)
                {
                    return "[E]";
                }

                if (placedFish != null)
                    return "[Hold E]";

                return "?";

            case InteractionType.Mush:

                if (!player.IsHoldingItem())
                    return "[E]";

                return "?";

            case InteractionType.MushBarrel:

                if (!player.IsHoldingItem())
                    return "?";

                if (currentMush >= maxMush)
                    
                    return "Barrel Full";

                return "[Hold E]";
        }

        return "?";
    }

    public void Interact(PlayerInteraction player)
    {
        switch (interactionType)
        {
            case InteractionType.FishBarrel:

                TakeFish(player);

                break;

            case InteractionType.FishTable:

                if (player.IsHoldingItem() &&
                    placedFish == null)
                {
                    PlaceFish(player);
                }

                break;

            case InteractionType.Mush:

                PickUpMush(player);

                break;
        }
    }

    public void HoldInteract(PlayerInteraction player)
    {
        switch (interactionType)
        {
            case InteractionType.FishTable:

                if (placedFish != null)
                {
                    ProcessFish(player);
                }

                break;

            case InteractionType.MushBarrel:

                FillBarrel(player);

                break;
        }
    }

    public void StopHoldInteract(PlayerInteraction player)
    {
        holdTimer = 0f;
    }

    void TakeFish(PlayerInteraction player)
    {
        if (player.IsHoldingItem())
            return;

        if (fishPrefab == null)
        {
            Debug.LogWarning("Fish Prefab is missing!");
            return;
        }

        player.PickUpItem(fishPrefab);

        player.PlaySound(
            player.fishPickupSound
        );
    }

    void PlaceFish(PlayerInteraction player)
    {
        if (!player.IsHoldingItem())
            return;

        if (fishPrefab == null)
        {
            Debug.LogWarning("Fish Prefab is missing!");
            return;
        }

        string heldItemName = player.heldItem.name.Replace("(Clone)", "").Trim();

        string fishName = fishPrefab.name.Replace("(Clone)", "").Trim();

        if (heldItemName != fishName)
        {
            holdTimer = 0f;
            return;
        }

        Vector3 originalWorldScale =
            fishPrefab.transform.lossyScale;

        player.DropHeldItem();

        placedFish =
            Instantiate(fishPrefab);

        placedFish.transform.SetParent(transform);

        placedFish.transform.localPosition =
            fishPlacementOffset;

        placedFish.transform.localRotation =
            Quaternion.Euler(
                fishRotationOffset
            );

        placedFish.transform.localScale =
            new Vector3(
                originalWorldScale.x /
                transform.lossyScale.x +
                fishScaleOffsetX,

                originalWorldScale.y /
                transform.lossyScale.y +
                fishScaleOffsetY,

                originalWorldScale.z /
                transform.lossyScale.z +
                fishScaleOffsetZ
            );

        Rigidbody rb =
            placedFish.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Collider[] colliders =
            placedFish.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        player.PlaySound(
            player.fishPlaceSound
        );
    }

    void ProcessFish(PlayerInteraction player)
    {
        if (placedFish == null)
            return;

        if (mushPrefab == null)
        {
            Debug.LogWarning("Mush Prefab is missing!");
            return;
        }

        holdTimer += Time.deltaTime;

        if (holdTimer < holdTime)
            return;

        holdTimer = 0f;

        Destroy(placedFish);
        placedFish = null;

        Vector3 originalWorldScale =
            mushPrefab.transform.lossyScale;

        placedMush =
            Instantiate(mushPrefab);

        placedMush.transform.SetParent(transform);

        placedMush.transform.localPosition =
            mushPlacementOffset;

        placedMush.transform.localRotation =
            Quaternion.Euler(
                mushRotationOffset
            );

        placedMush.transform.localScale =
            new Vector3(
                originalWorldScale.x /
                transform.lossyScale.x +
                mushScaleOffsetX,

                originalWorldScale.y /
                transform.lossyScale.y +
                mushScaleOffsetY,

                originalWorldScale.z /
                transform.lossyScale.z +
                mushScaleOffsetZ
            );

        Rigidbody rb =
            placedMush.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Collider[] colliders =
            placedMush.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        InteractableObject mushInteraction =
            placedMush.GetComponent<InteractableObject>();

        if (mushInteraction == null)
        {
            mushInteraction =
                placedMush.AddComponent<InteractableObject>();
        }

        mushInteraction.interactionType =
            InteractionType.Mush;

        mushInteraction.parentTable =
            this;

        player.PlaySound(
            player.fishProcessSound
        );
    }

    void PickUpMush(PlayerInteraction player)
    {
        if (player.IsHoldingItem())
            return;

        if (mushPrefab == null)
        {
            Debug.LogWarning("Mush Prefab is missing!");
            return;
        }

        if (parentTable != null)
        {
            parentTable.placedMush = null;
        }

        player.PickUpItem(
            mushPrefab,
            mushHandRotationOffset,
            mushHandScaleOffset
        );

        player.PlaySound(
            player.mushPickupSound
        );

        Destroy(gameObject);
    }

    void FillBarrel(PlayerInteraction player)
    {
        if (!player.IsHoldingItem())
            return;

        if (currentMush >= maxMush)
            return;

        if (player.heldItem == null || mushPrefab == null)
        {
            holdTimer = 0f;
            return;
        }

        string heldItemName = player.heldItem.name.Replace("(Clone)", "").Trim();
        string mushName = mushPrefab.name.Replace("(Clone)", "").Trim();

        if (heldItemName != mushName)
        {
            holdTimer = 0f;
            return;
        }

        holdTimer += Time.deltaTime;

        if (holdTimer < holdTime)
            return;

        
        holdTimer = 0f;

        currentMush++;

        player.DropHeldItem();

        player.PlaySound(
            player.mushPlaceSound
        );

        UpdateMushFill();

        Debug.Log(
            "Barrel Mush: " +
            currentMush +
            "/" +
            maxMush
        );

        if (currentMush >= maxMush)
        {
            Debug.Log("BARREL FULL!");
            objectiveManager.MushFinished();
        }
    }

    void UpdateMushFill()
    {
        if (mushFillCylinder == null)
            return;

        float fillPercentage =
            (float)currentMush /
            maxMush;

        Vector3 newPosition =
            mushFillCylinder.localPosition;

        newPosition.y =
            fillStartY +
            (fillHeight * fillPercentage);

        mushFillCylinder.localPosition =
            newPosition;
    }

    void Start()
    {
        if (interactionType ==
            InteractionType.MushBarrel)
        {
            UpdateMushFill();
        }
    }
}