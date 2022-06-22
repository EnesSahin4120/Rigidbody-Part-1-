using UnityEngine;

public class RopeSegment
{
    public Vector3 currentPos;
    public Vector3 previousPos;

    public RopeSegment(Vector3 targetPos) 
    {
        currentPos = targetPos;
        previousPos = targetPos;
    }
}
