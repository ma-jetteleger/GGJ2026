using UnityEngine;

public class WhaleEllipseMotion : MonoBehaviour
{
    [Header("Ellipse Settings")]
    [Tooltip("The width of the ellipse (Semi-major axis)")]
    public float xAxis = 20f;

    [Tooltip("The length of the ellipse (Semi-minor axis)")]
    public float zAxis = 10f;

    [Header("Movement Settings")]
    [Tooltip("How fast the whale completes a loop")]
    public float speed = 1.0f;

    // The center point is now private, as it is determined by the object's start position
    private Vector3 centerPoint;
    private float angle;

    void Start()
    {
        // Capture the object's world position when the game starts
        centerPoint = transform.position;
    }

    void Update()
    {
        // 1. Increment angle
        angle += speed * Time.deltaTime;
        if (angle > Mathf.PI * 2) angle -= Mathf.PI * 2;

        // 2. Calculate Position (relative to the captured centerPoint)
        // x = center + (a * cos(t))
        // z = center + (b * sin(t))
        float x = Mathf.Cos(angle) * xAxis;
        float z = Mathf.Sin(angle) * zAxis;

        Vector3 newPosition = new Vector3(centerPoint.x + x, centerPoint.y, centerPoint.z + z);

        // 3. Calculate Tangent (Velocity Vector) for rotation
        // The derivative gives us the direction we are heading
        float dx = -Mathf.Sin(angle) * xAxis;
        float dz = Mathf.Cos(angle) * zAxis;

        Vector3 tangentDirection = new Vector3(dx, 0, dz);

        // 4. Apply
        transform.position = newPosition;

        if (tangentDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(tangentDirection);
        }
    }

    // Visualization
    void OnDrawGizmosSelected()
    {
        // If the game hasn't started, we visualize assuming the current position is the center
        Vector3 drawCenter = Application.isPlaying ? centerPoint : transform.position;

        Gizmos.color = Color.cyan;
        Vector3 previousPoint = drawCenter + new Vector3(xAxis, 0, 0);

        for (int i = 1; i <= 50; i++)
        {
            float t = (i / 50f) * 2 * Mathf.PI;
            float x = Mathf.Cos(t) * xAxis;
            float z = Mathf.Sin(t) * zAxis;
            Vector3 nextPoint = drawCenter + new Vector3(x, 0, z);

            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}