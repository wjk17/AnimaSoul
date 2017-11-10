using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ASRigTransfer : MonoBehaviour
{
    // 添加到原模型对象上(骨架的父对象)，执行应用流程，两次Bake
    public ZClipContainer zPose;
    public Transform originRig;
    public Transform zPoseRig;
    public Transform finalRig;
    public string[] ignore;
    Transform GetRig()
    {
        foreach (Transform t in transform)
        {
            if (t.GetComponent<SkinnedMeshRenderer>() == null)
            {
                if (t.name.EndsWith("rig", StringComparison.InvariantCultureIgnoreCase))
                {
                    return t;
                }
            }
        }
        throw new Exception("no rig");
    }
    public void WorkFlow()
    {
        Flow1();
        Flow2();
        Flow3();
    }
    [ContextMenu("Flow0")]
    public void Flow0()
    {
        //删除忽视的物体，匹配材质，删除animator
        foreach (var ig in ignore)
        {
            ComTool.DestroyAuto(gameObject.Search(ig));
        }
        var UAnimator = GetComponent<Animator>();
        if (UAnimator != null) ComTool.DestroyAuto(UAnimator);
    }
    public void Flow1()
    {
        Flow0();
        //第一次bake 清空transform apply
        originRig = GetRig();
        zPoseRig = Instantiate(originRig, originRig.parent, true);
        originRig.gameObject.SetActive(false);
        zPoseRig.name = "ZPoseRig";
        SetActiveRig(zPoseRig, true);
        //
        ComTool.DestroyAuto(originRig.gameObject);
        var olds = GetComponentsInChildren<BakeSingleSMR>(true);
        foreach (var o in olds)
        {
            ComTool.DestroyAuto(o.gameObject);
        }
    }

    public void Flow2()
    {
        //播放ZPose动画
        var zp = zPose.clip.Clone();
        zp.Sample(zPoseRig);
    }
    public void Flow3()
    {
        //第二次bake+清空，重新绑定
        finalRig = Instantiate(zPoseRig, zPoseRig.parent, true);
        zPoseRig.gameObject.SetActive(false);
        finalRig.name = "FinalRig";
        SetActiveRig(finalRig, true);
        //
        ComTool.DestroyAuto(zPoseRig.gameObject);
        var olds = GetComponentsInChildren<BakeSingleSMR>(true);
        foreach (var o in olds)
        {
            ComTool.DestroyAuto(o.gameObject);
        }
    }

    // 烘焙蒙皮网格，设为活动骨骼，但不应用变换
    public void SetActiveRig(Transform rig, bool applyBonesTransform = false)
    {
        var smrs = rig.parent.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var t in smrs)
        {
            var bss = t.GetComOrAdd<BakeSingleSMR>();
            bss.BakeSMR();
        }
        if (applyBonesTransform) BakeTool.ApplyBonesTransform(rig);
        foreach (var t in smrs)
        {
            var bss = t.GetComponent<BakeSingleSMR>();
            if (bss.oldSMR == null) bss.oldSMR = t.GetComponent<SkinnedMeshRenderer>();
            bss.BakeStep2(rig);
        }
    }
}


