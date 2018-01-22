using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RematchSkinnedMeshBones : MonoBehaviour
{
    public SkinnedMeshRenderer to;
    public SkinnedMeshRenderer smr;
    private void Reset()
    {
        to = transform.GetComponent<SkinnedMeshRenderer>();
    }
    [ContextMenu("Rematch")]
    void Rematch()
    {
        //指定好新的rootbone，在新root下匹配名字相同的bone
        var list = new List<Transform>();
        foreach (var bone in smr.bones)
        {
            list.Add(to.rootBone.Search(bone.name));
        }
        to.bones = list.ToArray();
    }
}
