using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class RopeController : MonoBehaviour
{
    private LineRenderer rope;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    [SerializeField] private Transform connectedBody;

    [Header("User Parameters")]
    [Range(0,5)]
    [SerializeField] private float gravityFactor;
    [SerializeField] private float rangeBetweenSegments;
    [SerializeField] private int segmentCount; 
    [SerializeField] private float ropeThickness;

    private void Awake()
    {
        rope = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        AlignRopeSegments();
    }

    private void AlignRopeSegments()
    {
        Vector3 segmentStartPos = connectedBody.position;
        for(int i = 0; i < segmentCount; i++)
        {
            RopeSegment currentSegment = new RopeSegment(segmentStartPos);
            ropeSegments.Add(currentSegment);
            segmentStartPos.y -= rangeBetweenSegments;
        }
    }

    private void Update()
    {
        SetLineRenderer();
    }

    private void FixedUpdate()
    {
        ControllingRope();
        Constraint();
    }

    private void ControllingRope()   
    {
        Vector3 gravityForce = new Vector3(0, -gravityFactor, 0);
        for(int i = 1; i < segmentCount; i++)
        {
            RopeSegment startSegment = ropeSegments[i];
            Vector3 velocityDir = startSegment.currentPos - startSegment.previousPos;
            startSegment.previousPos = startSegment.currentPos;
            startSegment.currentPos += velocityDir;
            startSegment.currentPos += gravityForce * Time.deltaTime;
            ropeSegments[i] = startSegment;
        }
    }

    private void Constraint()
    {
        RopeSegment connectedSegment_to_Body = ropeSegments[0];
        connectedSegment_to_Body.currentPos = connectedBody.position;
        ropeSegments[0] = connectedSegment_to_Body;

        for(int i = 0; i < segmentCount - 1; i++)
        {
            RopeSegment startSegment = ropeSegments[i];
            RopeSegment endSegment = ropeSegments[i + 1];

            float currentDistance = (startSegment.currentPos - endSegment.currentPos).magnitude;
            float fallibility = Mathf.Abs(rangeBetweenSegments - currentDistance);
            Vector3 direction = (endSegment.currentPos - startSegment.currentPos).normalized;

            direction = rangeBetweenSegments > currentDistance ? direction : -direction;
            direction *= fallibility;
            direction *= 0.5f;

            if (i != 0)
            {
                startSegment.currentPos -= direction;
                ropeSegments[i] = startSegment;
            }
            endSegment.currentPos += direction;
            ropeSegments[i + 1] = endSegment;
        }
    }

    private void SetLineRenderer()
    {
        rope.startWidth = ropeThickness;
        rope.endWidth = ropeThickness;

        Vector3[] ropePositions = new Vector3[segmentCount];
        for (int i = 0; i < segmentCount; i++)
            ropePositions[i] = ropeSegments[i].currentPos;

        rope.positionCount = ropePositions.Length;
        rope.SetPositions(ropePositions);
    }
}
