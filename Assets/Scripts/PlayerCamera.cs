using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Player Target")]
    public Transform cameraTarget;

    [Header("Mouse Look")]
    public float mouseSensitivity = 150f;

    [Header("Look Limits")]
    public float minLookAngle = -80f;
    public float maxLookAngle = 80f;

    private float verticalRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (cameraTarget == null)
            return;

        float mouseX =
            Input.GetAxis("Mouse X") *
            mouseSensitivity *
            Time.deltaTime;

        float mouseY =
            Input.GetAxis("Mouse Y") *
            mouseSensitivity *
            Time.deltaTime;

        if (cameraTarget.parent != null)
        {
            cameraTarget.parent.Rotate(
                Vector3.up * mouseX
            );
        }

        verticalRotation -= mouseY;

        verticalRotation = Mathf.Clamp(
            verticalRotation,
            minLookAngle,
            maxLookAngle
        );

        cameraTarget.localRotation =
            Quaternion.Euler(
                verticalRotation,
                0f,
                0f
            );
    }

    private void LateUpdate()
    {
        if (cameraTarget == null)
            return;

        transform.position =
            cameraTarget.position;

        transform.rotation =
            cameraTarget.rotation;
    }
}