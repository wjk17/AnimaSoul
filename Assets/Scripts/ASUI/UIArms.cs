using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIArms : MonoBehaviour
{
    public Button buttonSetTrans;
    public Toggle toggleLockTrans;
    public Tran2 leftRela;
    public Tran2 leftForearmRela;
    public Tran2 rightRela;
    public Tran2 rightForearmRela;
    public Transform leftHand;
    public Transform rightHand;
    public Transform leftForearm;
    public Transform rightForearm;
    public Transform leftUpperarm;
    public Transform rightUpperarm;
    public Transform gun;
    public Avator ava;

    public float weight = 0.35f;
    public bool lerp = true;
    public bool lookAt = true;

    void Start()
    {
        buttonSetTrans.onClick.AddListener(SetTrans);
        toggleLockTrans.isOn = false;
        toggleLockTrans.onValueChanged.AddListener(LockTrans);

        ava = UIDOFEditor.I.avatar;
        leftHand = ava[Bone.hand_l].transform;
        rightHand = ava[Bone.hand_r].transform;
        leftForearm = ava[Bone.forearm_l].transform;
        rightForearm = ava[Bone.forearm_r].transform;
        leftUpperarm = ava[Bone.upperarm_l].transform;
        rightUpperarm = ava[Bone.upperarm_r].transform;
        gun = ava[Bone.other].transform;
    }
    private void LockTrans(bool arg0)
    {
        if (!arg0) UIClip.I.UpdateAllCurve();
    }
    private void SetTrans()
    {
        leftRela = Tran2.Relative(gun, leftHand);
        leftForearmRela = Tran2.Relative(leftHand, leftForearm);
        rightRela = Tran2.Relative(gun, rightHand);
        rightForearmRela = Tran2.Relative(rightHand, rightForearm);
        Debug.Log("设置双手的相对变换。");
    }
    void UpdateBone(Transform t)
    {
        var curve = UIClip.clip.GetCurve(ava[t]);
        var tran2e =  curve.Tran2E(UITimeLine.I.frameIndex);
        t.localPosition = tran2e.pos;
        var ast = ava[t];
        ast.euler = tran2e.rot;
        ast.Update();
    }
    void SetLookAtChain(params Transform[] ts)
    {
        Quaternion q;
        Tran2 o;
        Transform ts0, ts1;
        for (int i = 0; i < ts.Length - 1; i++)// -1 因为最后一个骨骼没有看的目标
        {
            ts0 = ts[i];
            ts1 = ts[i + 1];
            o = ts1.Tran2();
            //var axis = ts0.up;
            var axis = ava[ts0].upWorld; // 世界轴
            q = Quaternion.FromToRotation(axis, (ts1.position - ts0.position).normalized); //计算朝向
            ts0.rotation = q * ts0.rotation;
            ts1.SetTran2(o);
        }
    }
    private void LateUpdate()
    {
        if (toggleLockTrans.isOn)
        {
            Tran2 orela, ot, ofa;

            //////
            leftHand.SetTran2(Tran2.Absolute(gun, leftRela));//根据手枪设置手的绝对位置
            ot = leftHand.Tran2();
            orela = Tran2.Absolute(leftHand, leftForearmRela); // 预先记录相对位置的绝对值，理由同上。
            if (lerp) UpdateBone(leftForearm); // 根据曲线设置 前臂 的本地位置
            ofa = leftForearm.Tran2(); // 记录 前臂 的本地变换绝对值
            leftForearm.SetTran2(Tran2.Lerp(ofa, orela, lerp ? weight : 1)); // 与相对变换插值
            leftHand.SetTran2(ot); // 还原手的绝对位置

            //////
            rightHand.SetTran2(Tran2.Absolute(gun, rightRela));
            ot = rightHand.Tran2();
            orela = Tran2.Absolute(rightHand, rightForearmRela);
            if (lerp) UpdateBone(rightForearm);
            ofa = rightForearm.Tran2();
            rightForearm.SetTran2(Tran2.Lerp(ofa, orela, lerp ? weight : 1));
            rightHand.SetTran2(ot);

            if (lookAt)
            {
                SetLookAtChain(leftUpperarm, leftForearm, leftHand);
                SetLookAtChain(rightUpperarm, rightForearm, rightHand);
                //SetLookAtChain(rightForearm, rightHand);
            }
        }
    }
}
