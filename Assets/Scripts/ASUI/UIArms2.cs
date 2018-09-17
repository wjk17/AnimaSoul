using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(UIArms2))]
public class UIArms2Editor : Editor
{
    public override void OnInspectorGUI()
    {
        var o = (UIArms2)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Go Iteration (and Clear)"))
        {
            o.doIter = true;
            o.ClearBest();
            o.ResetFs();
        }
        if (GUILayout.Button("Set Bone to Best"))
        {
            o.iterBest();
        }
        if (GUILayout.Button("Clear Best"))
        {
            o.ClearBest();
        }
    }
}
#endif
public class UIArms2 : MonoBehaviour
{
    public Button buttonSetArmTarget;
    public Toggle toggleTowardArmTarget;

    public Vector3 leftHandPos;
    public Vector3 leftForearmPos;
    public Vector3 rightHandPos;
    public Vector3 rightForearmPos;

    public Transform leftHand;
    public Transform leftForearm;
    public Transform leftUpperarm;

    public Transform rightHand;
    public Transform rightForearm;
    public Transform rightUpperarm;
    public Transform gun;
    public Avator ava;

    void Start()
    {
        ResetFs();
        buttonSetArmTarget.onClick.AddListener(SetArmTarget);
        toggleTowardArmTarget.isOn = false;
        toggleTowardArmTarget.onValueChanged.AddListener(LockTrans);

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
        //if (!arg0) UIClip.I.UpdateAllCurve();
    }
    private void SetArmTarget()
    {
        if (absPos)
        {
            leftHandPos = leftHand.position;
            leftForearmPos = leftForearm.position;
            rightHandPos = rightHand.position;
            rightForearmPos = rightForearm.position;
        }
        else
        {
            leftForearmPos = leftHand.InverseTransformPoint(leftForearm.position);
            rightForearmPos = rightHand.InverseTransformPoint(rightForearm.position);
            handTargetDistance = Vector3.Distance(rightHand.position, rightUpperarm.position);
        }
        Debug.Log("设置双手的目标位置。");
    }
    void UpdateBone(Transform t)
    {
        var curve = UIClip.clip.GetCurve(ava[t]);
        var tran2e = curve.Tran2E(UITimeLine.I.frameIdx);
        t.localPosition = tran2e.pos;
        var ast = ava[t];
        ast.euler = tran2e.rot;
        ast.Update();
    }
    public void ResetFs()
    {
        i6 = new Iter(length);
        i5 = new Iter(length, i6);
        i4 = new Iter(length, i5);
        i3 = new Iter(length, i4);
        i2 = new Iter(length, i3);
        i1 = new Iter(length, i2);
    }
    public Iter i1, i2, i3, i4, i5, i6;
    public float bestDistance;
    public int iterPerFrame = 100;
    public int length = 100;
    public bool doIter;
    public bool absPos;
    public Transform rightHandTarget;
    public float handTargetDistance;

    [Serializable]
    public class Iter
    {
        public static implicit operator float(Iter i)
        {
            return i.value;
        }
        public Iter(int length, Iter child = null)
        {
            this.child = child;
            this.length = length;
            step = 1f / length;
            best = defaultBest;
        }
        public float defaultBest = float.PositiveInfinity;
        //[HideInInspector]
        public float step;
        public Iter child;
        public int length;
        public int current; // 正在进行第几次迭代
        public float value;
        public float best; // 目前的理想值，最接近目标的值;
        public void run()
        {
            if (child != null && child.current < length)
            {
                child.run();
            }
            else if (current < length)
            {
                current++;
                value = current * step;
                resetChilds();
            }
        }
        private void resetChilds()
        {
            if (child != null)
            {
                child.resetChilds();
                child.current = 0;
            }
        }
        internal void SaveBest()
        {
            best = value;
            if (child != null) child.SaveBest();
        }
        internal void ClearBest()
        {
            best = defaultBest;
            if (child != null) child.ClearBest();
        }
    }
    private void LateUpdate()
    {
        if (!toggleTowardArmTarget.isOn) return;

        if (!doIter) return;
        else if (i1.current >= length) { iterBest(); doIter = false; return; }// 完成迭代

        int c = 0; // 当前帧执行了多少次迭代
        while (c < iterPerFrame)
        {
            c++;
            i1.run();
            iteration();
        }
    }
    public void iterBest()
    {
        iteration(i1.best, i2.best, i3.best, i4.best, i5.best, i6.best);
    }    
    void iteration()
    {
        iteration(i1, i2, i3, i4, i5, i6);

        Vector3 posForearmR, posHandR;
        float d1, d2;
        if (absPos)
        {
            posForearmR = rightForearm.position;
            posHandR = rightHand.position;
        }
        else
        {
            posForearmR = rightHand.InverseTransformPoint(rightForearm.position);
            posHandR = rightHand.position;
        }
        d1 = Vector3.Distance(rightForearmPos, posForearmR); // 肘部位置与目标的距离

        var rela = rightHandTarget.position - rightUpperarm.position;
        rightHandTarget.position = rightUpperarm.position + rela.normalized * handTargetDistance;

        d2 = Vector3.Distance(rightHandTarget.position, posHandR); // 手掌位置与目标的距离
        var dis = d1 + d2;

        if (dis < bestDistance) // 出现新的理想值
        {
            bestDistance = dis; // 更新（保存）相应数值
            i1.SaveBest();
        }
    }
    // 输入 6 个参数 , 分别对应是前臂的twist+swingXZ（TXZ）和上臂的 TXZ
    void iteration(float f1, float f2, float f3, float f4, float f5, float f6)
    {
        var ast = ava[rightUpperarm];

        ast.swingX = f1;
        ast.swingZ = f2;
        ast.twistT = f3;
        ast.Update();

        ast = ava[rightForearm];

        ast.swingX = f4;
        ast.swingZ = f5;
        ast.twistT = f6;
        ast.Update();
    }
    internal void ClearBest()
    {
        bestDistance = 1000;
        i1.ClearBest();
    }
}
