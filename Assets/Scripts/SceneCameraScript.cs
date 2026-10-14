using System.Collections;
using TMPro;
using UnityEngine;

public class SceneCameraScript : MonoBehaviour
{
    [Header("Murphy")]
    public GameObject murphyFishing;

    [Header("Objective")]
    public ObjectiveManager objectiveManager;

    [Header("Camera")]
    public Camera cameraMain;

    [Header("Scene Targets")]
    public Transform[] targets;

    [Header("Text")]
    public TMP_Text quoteText;

    [TextArea(2, 5)]
    public string[] quotes;

    [Header("Settings")]
    public float textDisplayTime = 3f;

    [Header("End Scene")]
    public Transform playerTarget;
    public GameObject murph;
    public GameObject mckinnely;
    public GameObject player;
    public float endMoveTime = 2f;

    private Coroutine cameraSceneCoroutine;

    private bool playerCameraActive = false;

    private void Start()
    {
        murphyFishing.SetActive(false);
        // Find camera automatically
        if (cameraMain == null)
        {
            GameObject cameraObj = GameObject.Find("Camera");

            if (cameraObj != null)
            {
                cameraMain = cameraObj.GetComponent<Camera>();
            }
        }

        // Start cutscene
        cameraSceneCoroutine = StartCoroutine(PlayCameraScene());
    }

    private void LateUpdate()
    {
        if (!playerCameraActive || playerTarget == null)
            return;

        // Follow CameraTarget position
        cameraMain.transform.position = playerTarget.position;

        // Find PlayerMovement
        PlayerMovement playerMovement =
            playerTarget.GetComponentInParent<PlayerMovement>();

        if (playerMovement != null)
        {
            // Get camera rotation directly from mouse-look values
            float yaw = playerMovement.GetYaw();
            float pitch = playerMovement.GetPitch();

            cameraMain.transform.rotation =
                Quaternion.Euler(
                    pitch,
                    yaw,
                    0f
                );
        }
    }

    IEnumerator PlayCameraScene()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null)
                continue;

            Transform currentTarget = targets[i];

            // Immediately teleport to target
            cameraMain.transform.position = currentTarget.position;
            cameraMain.transform.rotation = currentTarget.rotation;

            // Display text
            if (quoteText != null && i < quotes.Length)
            {
                quoteText.text = quotes[i];
            }

            // Continuously follow target
            float timer = 0f;

            while (timer < textDisplayTime)
            {
                timer += Time.deltaTime;

                cameraMain.transform.position = currentTarget.position;
                cameraMain.transform.rotation = currentTarget.rotation;

                yield return null;
            }
        }

        // Clear text
        if (quoteText != null)
        {
            quoteText.text = "";
        }

        // End cutscene
        EndCameraScene();
    }

    public void EndCameraScene()
    {
        if (cameraSceneCoroutine != null)
        {
            StopCoroutine(cameraSceneCoroutine);
        }

        StartCoroutine(MoveToPlayer());

        murphyFishing.SetActive(true);
    }

    IEnumerator MoveToPlayer()
    {
        // Activate player
        if (player != null)
        {
            player.SetActive(true);
        }

        // Wait one frame for Player and CameraTarget to activate
        yield return null;

        if (playerTarget == null)
        {
            Debug.LogWarning("Player Target has not been assigned!");
            yield break;
        }

        // Starting camera position
        Vector3 startPosition = cameraMain.transform.position;
        Quaternion startRotation = cameraMain.transform.rotation;

        float timer = 0f;

        // Smoothly move toward Player CameraTarget
        while (timer < endMoveTime)
        {
            timer += Time.deltaTime;

            float progress = timer / endMoveTime;

            cameraMain.transform.position = Vector3.Lerp(
                startPosition,
                playerTarget.position,
                progress
            );

            cameraMain.transform.rotation = Quaternion.Slerp(
                startRotation,
                playerTarget.rotation,
                progress
            );

            yield return null;
        }

        // Make sure camera reaches target
        cameraMain.transform.position = playerTarget.position;
        cameraMain.transform.rotation = playerTarget.rotation;

        // Hide characters
        if (murph != null)
        {
            murph.SetActive(false);
        }

        if (mckinnely != null)
        {
            mckinnely.SetActive(false);
        }

        // =========================
        // RESET CAMERA PROJECTION
        // =========================

        cameraMain.orthographic = false;

        cameraMain.fieldOfView = 60f;

        cameraMain.nearClipPlane = 0.01f;

        cameraMain.farClipPlane = 1000f;

        // NOW continuously follow Player CameraTarget
        // NOW continuously follow Player CameraTarget
        playerCameraActive = true;

        // Show the first objective AFTER the entire cutscene
        // and camera transition are finished.
        if (objectiveManager != null)
        {
            objectiveManager.ShowObjectives();
        }

        Debug.Log("Player camera control enabled!");
        Debug.Log("Player camera control enabled!");
    }
}