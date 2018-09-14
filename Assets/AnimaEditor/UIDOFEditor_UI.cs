using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class UIDOFEditor
{
    public void InitUI()
    {
        f = GetComponent<UIDOFEditor_Fields>();

        this.SingleCheck(f.toggleIK, f.toggleIKSingle, f.toggleLockMirror, f.toggleLockOneSide);

        f.inputTwistMin.Init(360, OnInputTMinChanged); // 自转  向内        
        f.inputTwistMax.Init(360, OnInputTMaxChanged); // 自转  向外        
        f.buttonTwistReset.Init(OnTwistReset); // R按钮  重置

        f.sliderTwist.Init(OnTwistSliderChanged);
        f.inputTwist.Init(0, OnTwistInputChanged);

        f.inputSwingXMin.Init(OnInputXMinChanged); // 摆动  向前        
        f.inputSwingXMax.Init(OnInputXMaxChanged); // 摆动  向后        
        f.buttonSwingXReset.Init(OnSwingXReset); // R按钮  重置

        f.sliderSwingX.Init(OnSwingXSliderChanged);
        f.inputSwingX.Init(OnSwingXInputChanged);

        f.inputSwingZMin.Init(OnInputZMinChanged); // 摆动  向内        
        f.inputSwingZMax.Init(OnInputZMaxChanged); // 摆动  向外        
        f.buttonSwingZReset.Init(OnSwingZReset); // R按钮  重置

        f.sliderSwingZ.Init(OnSwingZSliderChanged);
        f.inputSwingZ.Init(OnSwingZInputChanged);

        // 骨骼
        f.dropBone.Init((int)Bone.root, ASUI.Combine(BoneTool.names, "手枪"), OnDropdownChanged);
        f.dropBone.gameObject.AddComponent<DropDownLocateSelectedItem>();
        // 保存DOF
        f.buttonSaveDOF.Init(SaveAvatarSetting);
        // 保存Clip
        f.buttonSaveClip.Init(SaveClip);
        // 文件名
        var cname = PlayerPrefs.GetString("LastOpenClipName", "New");
        f.inputFileName.Init(cname, OnFileNameChanged);
        // 打开文件
        f.buttonLoadClip.Init(LoadClip);
        // 新建Clip
        f.buttonNewClip.Init(NewClip);
        // 插入关键帧
        f.buttonInsertKeyFrame.Init(InsertKeyToAllCurves);
        // IK目标
        f.dropIKTarget.Init(0, ikTargetNames, OnIKTargetChanged, true);
        f.dropIKTarget.gameObject.AddComponent<DropDownLocateSelectedItem>();
        // 使用IK
        f.toggleIK.Init(OnIKToggle);
        // Snap
        f.buttonIKSnap.Init(OnIKSnap); //IK目标设为当前位置
        // IK目标(单关节)
        f.dropIKSingleTarget.Init(0, ikTargetSingleNames, IKSingleTargetChange, true);
        // 使用IK(单关节)
        f.toggleIKSingle.Init(OnIKSingleToggle);
        // Snap
        f.buttonIKSingleSnap.Init(OnIKSingleSnap);
        // 武器IK
        f.toggleWeaponIK.Init(OnWeaponIK);
        // 锁定骨骼位置
        f.toggleLockOneSide.Init(null);
        f.dropLockOneSide.Init(0, ikTargetNames, OnLockTargetChange, true);
        // 锁定骨骼位置(对称)
        f.toggleLockMirror.Init(null);
        f.dropLockMirror.Init(0, ikTargetMirrorNames, OnLockMirrorTargetChange, true);

        f.buttonEulerReset.Init(() => { OnSwingXReset(); OnSwingZReset(); OnTwistReset(); });

        GizmosAxis.I.controlObj = target;
    }
    public void OnWeaponIK(bool on)
    {
        Debug.Log("OnWeaponIK");
        if (on)
        {
            exBone2LeftHand = avatar[Bone.hand_l].transform.position - exBone.position;
            exBone2RightHand = avatar[Bone.hand_r].transform.position - exBone.position;

            GizmosAxis.I.gameObject.SetActive(true);
            GizmosAxis.T.position = exBone.position;
            target.position = exBone.position;//只有拖动gizmosaxis时才更新，所以这里手动更新

            f.toggleIK.isOn = false;
            f.toggleIKSingle.isOn = false;
        }
    }
    private void OnIKSnap()
    {
        //GizmosAxis.I.gameObject.SetActive(true);
        GizmosAxis.T.position = end.position;
        target.position = end.position;
    }
    private void OnIKSingleSnap()
    {
        //GizmosAxis.I.gameObject.SetActive(true);
        GizmosAxis.T.position = end.position;
        target.position = end.position;
    }
    void OnIKBoneLengthChanged(float v, SliderWrapper slider)
    {

    }
    void OnIKToggle(bool on)
    {
        GizmosAxis.I.gameObject.SetActive(on);
    }
    void OnIKSingleToggle(bool on)
    {
        GizmosAxis.I.gameObject.SetActive(on);
    }
}
