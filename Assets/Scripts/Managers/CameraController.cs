using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform baseCameraPosition;
    private Transform focusPointTransform;

    // Transition
    public float transitionSmoothSpeed = 5f;
    private bool isTransitioning = false;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    public Transform targetParent;

    public float panSpeed = 20f;
    public float smoothSpeed = 5f;

    // Zoom and deadzones
    [Range(0f, 0.5f)]
    public float deadZonePercentage = 0.2f;
    public float zoomSensitivity = 10f;

    [Range(0f, 1f)]
    public float currentZoomLevel = 0.5f;

    // Angle and height
    public float minHeight = 5f;
    public float maxHeight = 30f;
    public float minAngle = 30f;
    public float maxAngle = 75f;


    // Map bounds
    public Vector2 mapBoundsX = new Vector2(-50f, 50f);
    public Vector2 mapBoundsZ = new Vector2(-50f, 50f);

    public bool isFocusing = false;

    void Update()
    {
        if (isTransitioning)
        {
            PerformTransition();
        }
        else
        {
            if (!isFocusing)
            {
                HandlePanMovement();
                HandleZoomAndPitch();
            }
        }
    }


    // Function for the transition from base to unit head
    private void PerformTransition()
    {
        if (isFocusing && focusPointTransform != null)
        {
            targetPosition = focusPointTransform.position;
            targetRotation = focusPointTransform.rotation;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime * transitionSmoothSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.unscaledDeltaTime * transitionSmoothSpeed);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f && Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            transform.position = targetPosition;
            transform.rotation = targetRotation;

            transform.SetParent(targetParent);
            isTransitioning = false;
        }
    }

    // Functions handling camera movement (up down left right etc)
    private void HandlePanMovement()
    {
        Vector3 move = Vector3.zero;

        float screenCenterY = Screen.height * 0.5f;
        float screenCenterX = Screen.width * 0.5f;

        float deadZoneY = Screen.height * deadZonePercentage;
        float deadZoneX = Screen.width * deadZonePercentage;

        if (Input.mousePosition.y > screenCenterY + deadZoneY)
        {
            move.z = 1f;
        }
        else if (Input.mousePosition.y < screenCenterY - deadZoneY)
        {
            move.z = -1f;
        }

        if (Input.mousePosition.x < screenCenterX - deadZoneX)
        {
            move.x = -1f;
        }
        else if (Input.mousePosition.x > screenCenterX + deadZoneX)
        {
            move.x = 1f;
        }

        move.Normalize();

        transform.position += move * panSpeed * Time.unscaledDeltaTime;

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, mapBoundsX.x, mapBoundsX.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, mapBoundsZ.x, mapBoundsZ.y);
        transform.position = clampedPosition;
    }
    private void HandleZoomAndPitch()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoomLevel = Mathf.Clamp01(currentZoomLevel - scrollInput * zoomSensitivity * Time.deltaTime);

        float targetY = Mathf.Lerp(minHeight, maxHeight, currentZoomLevel);

        Vector3 newPos = transform.position;
        newPos.y = Mathf.Lerp(newPos.y, targetY, Time.deltaTime * smoothSpeed);
        transform.position = newPos;

        float targetXAngle = Mathf.Lerp(maxAngle, minAngle, currentZoomLevel);

        Quaternion targetRotation = Quaternion.Euler(targetXAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.unscaledDeltaTime * smoothSpeed);
    }


    // Camera focus on unit head
    public void Focus(GameObject go)
    {
        focusPointTransform = go.GetComponent<Unit>().focusPoint.transform;

        targetPosition = focusPointTransform.position;
        targetRotation = focusPointTransform.rotation;
        targetParent = go.transform; 
        GetComponent<Camera>().fieldOfView = 110;

        isTransitioning = true;
        isFocusing = true;
    }
    public void ExitFocus()
    {
        targetPosition = baseCameraPosition.position;
        targetRotation = baseCameraPosition.rotation;
        targetParent = baseCameraPosition;

        focusPointTransform = null;

        GetComponent<Camera>().fieldOfView = 60;

        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
        isTransitioning = true;
        isFocusing = false;
    }
}