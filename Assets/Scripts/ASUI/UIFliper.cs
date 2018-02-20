﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFliper : MonoBehaviour
{
    public Button buttonL2R;
    public Button buttonR2L;
    public Button buttonSwitchLR;
    public Button buttonSwitchAll;
    public Button buttonSwitchAllExHips;
    private void Awake()
    {
        buttonL2R.onClick.AddListener(FlipLeft2Right);
        buttonR2L.onClick.AddListener(FlipRight2Left);
        buttonSwitchLR.onClick.AddListener(SwitchLeftAndRight);
        buttonSwitchAll.onClick.AddListener(SwitchAll);
        buttonSwitchAllExHips.onClick.AddListener(SwitchAllExHips);
    }
    public void SwitchAllExHips()
    {
        SwitchAllEx(ASBone.root);
    }
    public void SwitchAll()
    {
        SwitchAllEx();
    }
    bool ArrayInclude<T>(ICollection<T> array, T item)
    {
        foreach (var t in array)
        {
            if (t.Equals(item)) return true;
        }
        return false;
    }
    public void SwitchAllEx(params ASBone[] bones)
    {
        foreach (var t in UIDOFEditor.I.avatar.setting.asts)
        {
            if (ASClipTool.IsLeftBone(t.dof.bone))
            {
                var rightBone = ASClipTool.GetPairBone(t.dof.bone);
                if (rightBone > 0)
                {
                    var right = GetAstFromAvatar(rightBone);
                    if (right != null)
                    {
                        var left = t.euler;
                        t.euler = right.euler;
                        right.euler = left;
                    }
                }
            }
            else
            {
                if (ArrayInclude(bones, t.dof.bone) || ASClipTool.IsRightBone(t.dof.bone))
                {
                    continue;
                }
                else//middle
                {
                    t.euler.y = -t.euler.y;
                    t.euler.z = -t.euler.z;
                }
            }
        }
    }
    public void SwitchLeftAndRight()
    {
        foreach (var t in UIDOFEditor.I.avatar.setting.asts)
        {
            if (!ASClipTool.IsLeftBone(t.dof.bone)) continue;
            var rightBone = ASClipTool.GetPairBone(t.dof.bone);
            if (rightBone > 0)
            {
                var right = GetAstFromAvatar(rightBone);
                if (right != null)
                {
                    var left = t.euler;
                    t.euler = right.euler;
                    right.euler = left;
                }
            }
        }
    }
    public void FlipLeft2Right()
    {
        foreach (var t in UIDOFEditor.I.avatar.setting.asts)
        {
            if (!ASClipTool.IsLeftBone(t.dof.bone)) continue;
            var rightBone = ASClipTool.GetPairBone(t.dof.bone);
            if (rightBone > 0)
            {
                var right = GetAstFromAvatar(rightBone);
                if (right != null) right.euler = t.euler;
            }
        }
    }
    public void FlipRight2Left()
    {
        foreach (var t in UIDOFEditor.I.avatar.setting.asts)
        {
            if (!ASClipTool.IsRightBone(t.dof.bone)) continue;
            var leftBone = ASClipTool.GetPairBone(t.dof.bone);
            if (leftBone > 0)
            {
                var left = GetAstFromAvatar(leftBone);
                if (left != null) left.euler = t.euler;
            }
        }
    }
    ASTransDOF GetAstFromAvatar(ASBone bone)
    {
        foreach (var t in UIDOFEditor.I.avatar.setting.asts)
        {
            if (t.dof.bone == bone) return t;
        }
        return null;
    }
}
