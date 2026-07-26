using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AccuracyCone : MonoBehaviour
{
    LineRenderer lineRenderer;
    bool mouseHold = false;
    readonly float defaultAngle = 45;
    float currentAngle = 0f;
    float minAngle = 0f;
    float baseAngle = 0f;
    public float AimSpeed = 0.1f;
    public Camera camera;

    public float CurrentAngle => currentAngle;
    public float MinAngle => minAngle;
    public float BaseAngle => baseAngle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Custom/InvertUnlit"));
        lineRenderer.sortingLayerID = SortingLayer.NameToID("VFX");
        lineRenderer.sortingOrder = short.MaxValue;

        currentAngle = defaultAngle;
    }

    public void OnLeftClick(InputAction.CallbackContext ctx)
    {
        switch (ctx.phase)
        {
            case InputActionPhase.Started:
                lineRenderer.positionCount = 3;
                mouseHold = true;
                currentAngle = defaultAngle;
                break;
            case InputActionPhase.Canceled:
                lineRenderer.positionCount = 0;
                mouseHold = false;
                break;
        }
    }

    // Force the accuracy cone off (e.g. when the revolver is holstered).
    public void Hide()
    {
        mouseHold = false;
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }

    public void SetBaseAngle(float angle)
    {
        baseAngle = angle;
    }

    public void SetMinAimAngle(float angle)
    {
        minAngle = angle;
    }

    public void SetAimSpeed(float speed)
    {
        AimSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseHold)
        {
            float radians = currentAngle * Mathf.Deg2Rad;
            Vector3 mouseScreen = Mouse.current.position.ReadValue();
            mouseScreen.z = Mathf.Abs(camera.transform.position.z - transform.position.z);
            var mousePos = camera.ScreenToWorldPoint(mouseScreen);
            Vector3 apex = transform.position;

            baseAngle = Mathf.Atan2(mousePos.y - apex.y, mousePos.x - apex.x);

            Vector3 upperDir = new(
                Mathf.Cos(baseAngle + radians),
                Mathf.Sin(baseAngle + radians),
                0
            );
            Vector3 lowerDir = new(
                Mathf.Cos(baseAngle - radians),
                Mathf.Sin(baseAngle - radians),
                0
            );

            lineRenderer.SetPosition(0, apex + upperDir * 5);
            lineRenderer.SetPosition(1, apex);
            lineRenderer.SetPosition(2, apex + lowerDir * 5);
            Debug.Log(AimSpeed);
            currentAngle = (currentAngle <= minAngle) ? minAngle : currentAngle - AimSpeed;
        }
    }
}
