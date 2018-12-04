using System.Collections.Generic;
using UnityEngine;
using System;
namespace Esa.UI
{
    public partial class UIDOFEditor : MonoSingleton<UIDOFEditor>
    {
        public Bone bone = Bone.chest; // 给出个初始值

        public Vector3 lockPos1;
        public Vector3 lockPos2;

        public DOF dof;
        public DOFMgr dofSet
        {
            get
            {
                if (_dofSet == null)
                    _dofSet = FindObjectOfType<DOFMgr>(); return _dofSet;
            }
        }
        DOFMgr _dofSet;
        public Avator avatar
        {
            get
            {
                if (_avatar == null)
                    _avatar = FindObjectOfType<Avator>(); return _avatar;
            }
        }
        Avator _avatar;

        public Transform target;
        public Transform end;
        public int iter;
        public TransDOF ast;
        public TransDOF astIK;

        public float alpha; // 逼近的步长
        public float theta0;
        public float theta1;

        public List<Bone> joints;
        public List<Bone> joints2;
        public int jointIterCount = 10;
        public int axisIterCount = 20;

        public Transform exBone;
        public frame frameClipBoard;

        public Vector3 exBone2LeftHand;
        public Vector3 exBone2RightHand;

        [HideInInspector]
        public UIDOFEditor_Fields f;

        public int drawOrder = -1;
        public int CB_Order = 1;

        void Start()
        {
            this.AddInputCB(null, CB_Order);

            foreach (var curve in UIClip.I.clip.curves)
            {
                curve.ast.coord.originPos = curve.ast.transform.localPosition;
            }

            dofSet.Load(); // 强制从文件里读取
                           //avatar.LoadFromDOFMgr(); // 从内存里读取
            dof = dofSet[bone];
            ast = avatar[bone];

            ASUI.parent = transform.Search("Area");

            InitUI();

            UITimeLine.I.onFrameIdxChanged = OnFrameIdxChanged;

            UpdateDOF();
            exBone = avatar[Bone.other].transform;
        }
        void OnFrameIdxChanged(int frameIdx)
        {
            foreach (var oc in UIClip.I.clip.curves)
            {
                if (!oc.Empty())
                    oc.Update(UITimeLine.I.frameIdx);
            }
            UpdateDOF();
            //curve
        }
        //void GetInput()
        //{
        //    //拦截点击事件，防止穿透
        //    if (Events.Click && ASUI.MouseOver(transform.Search("Area") as RectTransform))
        //        Events.Use();
        //}
        public void InsertKeyToAllCurves()
        {
            UITimeLine.I.InsertKey();
        }
        private void OnFileNameChanged(string v)
        {

        }
        private void NewClip()
        {
            UIClip.I.New(f.inputFileName.text);
            f.labelFileName.text = f.inputFileName.text;
            UIClipList.I.GetClipNamesInPath();
            Debug.Log("新建 " + f.labelFileName.text);
        }
        void LoadClip()
        {
            UIClip.I.Load(f.inputFileName.text);
            f.labelFileName.text = f.inputFileName.text;
            UIClip.I.UpdateAllCurve();
            Debug.Log("读取 " + f.labelFileName.text);
        }
        void SaveClip()
        {
            UIClip.I.Save(f.inputFileName.text);
            UIClipList.I.GetClipNamesInPath();
            Debug.Log("保存 " + f.inputFileName.text);
        }
        void SaveAvatarSetting()
        {
            // 先修改DOF集，再引用到当前化身（Avatar）。不同化身，如不同化身（但骨架形状类似）的人物，生物可能使用相同的DOF集。
            dofSet.Save();
            avatar.LoadFromDOFMgr();
            avatar.Save();
        }
        void Update()
        {
            var hover = ASUI.MouseOver(UICurve.I.transform as RectTransform)
                || ASUI.MouseOver(UICamera.I.rectView as RectTransform);

            if (hover && Input.GetKeyDown(KeyCode.C))
            {
                CopyFrame();
            }
            else if (hover && Input.GetKeyDown(KeyCode.V))
            {
                PasteFrame();
            }

            if (f.toggleIK.isOn || f.toggleIKSingle.isOn)
            {
                IKSolve(joints.ToArray());
            }
            else if (f.toggleLockOneSide.isOn || f.toggleLockMirror.isOn)
            {
                end = avatar[joints[0]].transform;
                IKSolve(lockPos1, joints.ToArray());
                if (f.toggleLockMirror.isOn)
                {
                    end = avatar[joints2[0]].transform;
                    IKSolve(lockPos2, joints2.ToArray());
                }
            }
            else if (f.toggleWeaponIK.isOn) ExBoneIK();
        }
    }
}