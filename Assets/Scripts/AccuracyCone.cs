using UnityEngine;
using UnityEngine.InputSystem;

public class AccuracyCone : MonoBehaviour
{
    LineRenderer lineRenderer;
    bool mouseHold = false;
    readonly float defaultAngle = 45;
    float currentAngle = 0f;
    float baseAngle = 0f;
    public float AimSpeed = 0.1f;

    public float CurrentAngle => currentAngle;
    public float BaseAngle => baseAngle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.sortingLayerID = SortingLayer.NameToID("Player");
        lineRenderer.sortingOrder = 10;

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

    // Update is called once per frame
    void Update()
    {
        if (mouseHold)
        {
            float radians = currentAngle * Mathf.Deg2Rad;
            var mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
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
            currentAngle = (currentAngle <= 0) ? 0 : currentAngle - AimSpeed;
        }
    }
}
