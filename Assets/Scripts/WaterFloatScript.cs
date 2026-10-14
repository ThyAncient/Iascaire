using UnityEngine;
using UnityEngine.Rendering.HighDefinition; // Required for HDRP Water APIs

public class WaterFloater : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaterSurface targetWaterSurface;
    [SerializeField] private Rigidbody rb;

    [Header("Buoyancy Settings")]
    [SerializeField] private float floatingPower = 15f;
    [SerializeField] private float waterDrag = 3f;
    [SerializeField] private float waterAngularDrag = 1f;
    [SerializeField] private float surfaceOffset = 0f; // Offset adjustment for visual immersion

    private void FixedUpdate()
    {
        if (targetWaterSurface == null || rb == null) return;

        // 1. Initialize the target search parameters
        WaterSearchParameters searchParameters = new WaterSearchParameters
        {
            startPositionWS = transform.position,
            error = 0.01f,       // Precision threshold
            maxIterations = 8    // Iteration limits for convergence
        };

        // 2. Query the exact project point on the water surface
        if (targetWaterSurface.ProjectPointOnWaterSurface(searchParameters, out WaterSearchResult searchResult))
        {
            // searchResult.projectedPosition holds the exact structural Vector3 on the water
            float waterHeight = searchResult.projectedPositionWS.y + surfaceOffset;

            // 3. Compute forces if the object is submerged
            if (transform.position.y < waterHeight)
            {
                // Calculate displacement ratio based on depth
                float displacementMultiplier = Mathf.Clamp01((waterHeight - transform.position.y) / 1.0f);

                // Apply upward buoyant force resisting gravity
                Vector3 buoyantForce = Vector3.up * floatingPower * displacementMultiplier;
                rb.AddForce(buoyantForce, ForceMode.Acceleration);

                // Apply dampening to prevent endless bobbing
                rb.linearDamping = waterDrag;
                rb.angularDamping = waterAngularDrag;
            }
            else
            {
                // Reset to standard air drag thresholds when above water
                rb.linearDamping = 0.05f;
                rb.angularDamping = 0.05f;
            }
        }
    }
}