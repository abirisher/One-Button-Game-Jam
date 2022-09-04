using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectorySim : MonoBehaviour
{
    public PlayerController target;
    public float velocity;
    public float angle = 90;
    public int resolution = 10;

    private float g;
    private float radianAngle;
    LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        target = GameObject.Find("Player").GetComponent<PlayerController>();
        g = Mathf.Abs(Physics2D.gravity.y);
        velocity = target.jumpForce + target.speed;
        angle = 90;
    }
    // Start is called before the first frame update
    private void Start()
    {
        renderArc();
    }

    private void renderArc()
    {
        lineRenderer.positionCount = resolution + 1;
        lineRenderer.SetPositions(calculateArcArray());
    }

    private Vector3[] calculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];
        radianAngle = Mathf.Deg2Rad * angle;
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / (float)resolution;
            arcArray[i] = calculateArcPoint(t, maxDistance)+ new Vector3(target.transform.position.x, target.transform.position.y);
        }

        return arcArray;
    }

    private Vector3 calculateArcPoint(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(radianAngle) - ((g * x * x)/(2 * (velocity * Mathf.Cos(radianAngle)) * (velocity * Mathf.Cos(radianAngle))));
        return new Vector3(x, y);
    }
}
