using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UILocalDistance : MonoBehaviour
{
    public Transform target;
    public float distance;
    public bool lockDistance;
    public float distanceToLock;
    public bool drawRelationLine = true;
    public Color lineColor = Color.red;
    public bool depthTest = false;
    void LateUpdate()
    {
        if (lockDistance) target.localPosition = target.localPosition.normalized * distanceToLock;
        distance = Vector3.Distance(target.parent.position, target.position);

        if (drawRelationLine)
        {
            Debug.DrawLine(target.parent.position, target.position, lineColor, 0, depthTest);
        }
    }
}
