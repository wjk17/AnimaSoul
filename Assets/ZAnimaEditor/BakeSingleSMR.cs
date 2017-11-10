using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeSingleSMR : MonoBehaviour
{
    void Start()
    {
        BakeSMR();
    }
    public SkinnedMeshRenderer oldSMR;
    public SkinnedMeshRenderer newSMR;
    [ContextMenu("BakeSMR")]
    public void BakeSMR()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;


        oldSMR = GetComponent<SkinnedMeshRenderer>();
        if (oldSMR == null) return;
        var newGO = new GameObject(oldSMR.gameObject.name);// + "SMR");

        //先Bake
        var newMesh = new Mesh();
        oldSMR.BakeMesh(newMesh);

        //添加SMR物体
        newSMR = newGO.AddComponent<SkinnedMeshRenderer>();
        newSMR.sharedMesh = newMesh;
        newSMR.materials = oldSMR.sharedMaterials;

        //同步开关
        newGO.SetActive(gameObject.activeSelf);
        gameObject.SetActive(false); // 隐藏旧对象
        newGO.SetParent(transform.parent);
    }
    public void BakeStep2(Transform rig)
    {
        // 重新绑定和权重
        newSMR.rootBone = rig.Search(oldSMR.rootBone.name);
        var newBones = new List<Transform>();
        foreach (var b in oldSMR.bones)
        {
            newBones.Add(rig.Search(b.name));
        }
        newSMR.bones = newBones.ToArray();
        newSMR.sharedMesh.boneWeights = oldSMR.sharedMesh.boneWeights;
        ApplyBindPose(newSMR, rig);
    }
    public void ApplyBindPose(SkinnedMeshRenderer smr, Transform rig)
    {
        var mesh = smr.sharedMesh;

        var nl = new List<Matrix4x4>();
        var root = smr.rootBone;
        foreach (var bone in smr.bones)
        {
            //nl.Add(bone.worldToLocalMatrix * root.localToWorldMatrix);
            nl.Add(bone.worldToLocalMatrix * smr.transform.localToWorldMatrix * rig.parent.localToWorldMatrix);
        }
        mesh.bindposes = nl.ToArray();
    }
}
public static class BakeTool
{
    public static void SetRig(SkinnedMeshRenderer smr, Transform rig, Transform parent)
    {
        // 重新绑定和权重
        smr.rootBone = rig.Search(smr.rootBone.name);
        var newBones = new List<Transform>();
        foreach (var b in smr.bones)
        {
            newBones.Add(rig.Search(b.name));
        }
        smr.bones = newBones.ToArray();
        //重置BinsPose
        var nl = new List<Matrix4x4>();
        foreach (var bone in smr.bones)
        {
            nl.Add(bone.worldToLocalMatrix * smr.transform.localToWorldMatrix * parent.localToWorldMatrix);
        }
        smr.sharedMesh.bindposes = nl.ToArray();
    }
    public static void ApplyBonesTransform(Transform newRig)
    {
        foreach (var bone in newRig.GetComponentsInChildren<Transform>())
        {
            var origin = new List<Vector3>();
            var child = bone.GetComponentsInChildren<Transform>();
            for (int i = 1; i < child.Length; i++)
            {
                origin.Add(child[i].position);
            }
            bone.localRotation = Quaternion.identity;
            bone.localScale = Vector3.one; // 没有考虑骨骼缩放，仅消除 Blender 与 Unity 浮点数版本不同导致的误差。
            for (int i = 1; i < child.Length; i++)
            {
                child[i].position = origin[i - 1];
            }
        }
    }
}
