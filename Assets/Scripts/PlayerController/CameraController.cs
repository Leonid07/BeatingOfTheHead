using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;
    public Transform objectHolder;
    public Transform cameraHolder;
    public Transform handBase;
    public Transform[] handHolders;

    [Header("Settings")]
    public float mouseSensitivity = 100f;
    public float minX = -90f;
    public float maxX = 90f;

    private float xRotation = 0f;
    private Quaternion handBaseDefaultLocal;
    private Quaternion[] handDefaultLocalRots;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (handBase != null)
            handBaseDefaultLocal = handBase.localRotation;

        if (handHolders != null && handHolders.Length > 0)
        {
            handDefaultLocalRots = new Quaternion[handHolders.Length];
            for (int i = 0; i < handHolders.Length; i++)
                handDefaultLocalRots[i] = handHolders[i].localRotation;
        }
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        playerBody.Rotate(Vector3.up * mouseX);
        if (objectHolder != null)
            objectHolder.Rotate(Vector3.up * mouseX, Space.World);

        xRotation = Mathf.Clamp(xRotation - mouseY, minX, maxX);

        float camAngle = Mathf.Max(0f, xRotation);
        cameraHolder.localRotation = Quaternion.Euler(camAngle, 0f, 0f);

        if (handBase != null)
            handBase.localRotation = handBaseDefaultLocal * Quaternion.Euler(xRotation, 0f, 0f);

        if (handHolders != null && handDefaultLocalRots != null)
        {
            for (int i = 0; i < handHolders.Length; i++)
                handHolders[i].localRotation = handDefaultLocalRots[i];
        }
    }
}
