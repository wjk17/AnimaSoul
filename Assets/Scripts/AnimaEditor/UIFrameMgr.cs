using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Esa.UI
{
    public class frame
    {
        public List<key> keys;
    }
    public class key
    {
        public Tran2E t2;
        public Bone bone;
    }
    public class UIFrameMgr : MonoBehaviour
    {
        public UnityEngine.UI.Button insertMissButton;
        public UnityEngine.UI.Button deleteAllCurveButton;
        public UnityEngine.UI.Button pasteAllFrameButton;
        public UnityEngine.UI.Button pasteToArmButton;
        public UnityEngine.UI.Button pasteToGunButton;
        public UnityEngine.UI.Button pasteLeftHandToAllFrameBtn;
        private void Start()
        {
            this.AddInputCB();
            insertMissButton.onClick.AddListener(InsertMissCurve);
            deleteAllCurveButton.onClick.AddListener(DeleteAllCurve);
            pasteAllFrameButton.onClick.AddListener(PasteAllFrame);
            pasteToArmButton.onClick.AddListener(PasteToArm);
            pasteToGunButton.onClick.AddListener(PasteToGun);
            pasteLeftHandToAllFrameBtn.onClick.AddListener(pasteLeftHandToAllFrame);
        }
        private void pasteLeftHandToAllFrame()
        {
            UIDOFEditor.I.PasteFrameAllFrame(new Bone[] { Bone.shoulder_r, Bone.upperarm_r, Bone.forearm_r, Bone.hand_r });
        }

        void PasteToGun()
        {
            UIDOFEditor.I.PasteFrame(Bone.other);
        }
        void PasteToArm()
        {
            //UIDOFEditor.I.PasteFrame(ASBoneTool.arms);
            var list = new List<Bone>();
            list.Add(Bone.thumb1_l);
            list.Add(Bone.thumb2_l);
            list.Add(Bone.thumb3_l);

            list.Add(Bone.index1_l);
            list.Add(Bone.index2_l);
            list.Add(Bone.index3_l);

            list.Add(Bone.middle1_l);
            list.Add(Bone.middle2_l);
            list.Add(Bone.middle3_l);

            list.Add(Bone.ring1_l);
            list.Add(Bone.ring2_l);
            list.Add(Bone.ring3_l);

            list.Add(Bone.pinky1_l);
            list.Add(Bone.pinky2_l);
            list.Add(Bone.pinky3_l);

            //list.Add(ASBone.hand_l);

            var list2 = new List<Bone>();
            foreach (var i in list)
            {
                list2.Add(i + 1);
            }
            list.AddRange(list2);
            UIDOFEditor.I.PasteFrame(list);
        }
        void PasteAllFrame()
        {
            UIDOFEditor.I.PasteFrameAllFrame(BoneTool.arms);
        }
        void DeleteAllCurve()
        {
            foreach (var curve in UIClip.I.clip.curves)
            {
                curve.RemoveAtTime(UITimeLine.I.frameIdx);
            }
        }
        bool MissAst(TransDOF t) // ast是否存在于当前clip
        {
            foreach (var curve in UIClip.I.clip.curves)
            {
                if (curve.ast == t) return false;
            }
            return true;
        }
        void InsertMissCurve()
        {
            foreach (var ast in UIDOFEditor.I.avatar.data.asts)
            {
                if (MissAst(ast)) // 插入新增的（化身ast表里有，clip里却没有的）曲线
                {
                    UIClip.I.clip.curves.Add(new CurveObj(ast));
                }
            }
        }
    }
}