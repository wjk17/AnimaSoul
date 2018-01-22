using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyMaterials : MonoBehaviour
{
    public Transform from;
    public Transform to;
    [ContextMenu("Copy")]
    void Copy()
    {
        foreach (Transform t in from)
        {
            var rend = t.GetComponent<Renderer>();
            if (rend != null)
            {
                foreach (Transform t2 in to)
                {
                    if (t2.name == t.name)
                    {
                        var rend2 = t2.GetComponent<Renderer>();
                        if (rend2 != null)
                        {
                            rend2.sharedMaterials = rend.sharedMaterials;
                        }
                        break;
                    }
                }
            }
        }
    }
}
