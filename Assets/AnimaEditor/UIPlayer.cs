using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoSingleton<UIPlayer>
{
    public UIButtonSwitch buttonPlayPuase;
    public InputField inputfieldFps;
    public Toggle toggleLoop;
    public Toggle toggleFlip;
    public Toggle togglePingPong;
    public Button buttonApplyTo60Fps;
    public Toggle toggleMirrorPv;
    public bool play;
    void Start()
    {
        buttonPlayPuase.Reg(Play, Pause);
        inputfieldFps.onValueChanged.AddListener(OnFpsChange);
        inputfieldFps.contentType = InputField.ContentType.DecimalNumber;
        inputfieldFps.text = UITimeLine.I.fps.ToString();
        buttonApplyTo60Fps.onClick.AddListener(ApplyTo60Fps);
        toggleMirrorPv.onValueChanged.AddListener(MirrorPv);
    }
    void MirrorPv(bool value)
    {
        Mirror(true);
    }
    public void Mirror(bool force = false)
    {
        if (I.toggleMirrorPv.isOn || force)//镜像翻转
        {
            var handledList = new List<ASObjectCurve>();
            foreach (var curve in UIClip.clip.curves)
            {
                if (handledList.Contains(curve)) continue;
                // 互换对称骨骼的euler值，因为使用了“向内”、“向外”描述，因此不需要翻转值。
                if (curve.pair != null)
                {
                    var ex = curve.ast.euler;
                    curve.ast.euler = curve.pair.ast.euler;
                    curve.pair.ast.euler = ex;
                    handledList.Add(curve);
                    handledList.Add(curve.pair);
                }
                else
                {
                    // 没有.r或.l后缀的骨骼，因此需要翻转两条轴。
                    if (curve == null || curve.ast == null)
                    {
                        continue;
                    }
                    Vector3 mirrorOn = new Vector3(0, 1, 1);
                    if (mirrorOn.x > 0) curve.ast.euler.x = -curve.ast.euler.x;
                    if (mirrorOn.y > 0) curve.ast.euler.y = -curve.ast.euler.y;
                    if (mirrorOn.z > 0) curve.ast.euler.z = -curve.ast.euler.z;
                }
            }
        }
    }
    void ApplyTo60Fps()
    {
        var ratio = 60f / UITimeLine.Fps;
        foreach (var curve in UIClip.clip.curves)
        {
            foreach (var key in curve.eulerAngles[0].keys)
            {
                key.frameIndex = Mathf.RoundToInt(key.frameIndex * ratio);
            }
            foreach (var key in curve.eulerAngles[1].keys)
            {
                key.frameIndex = Mathf.RoundToInt(key.frameIndex * ratio);
            }
            foreach (var key in curve.eulerAngles[2].keys)
            {
                key.frameIndex = Mathf.RoundToInt(key.frameIndex * ratio);
            }
            foreach (var key in curve.localPosition[0].keys)
            {
                key.frameIndex = Mathf.RoundToInt(key.frameIndex * ratio);
            }
            foreach (var key in curve.localPosition[1].keys)
            {
                key.frameIndex = Mathf.RoundToInt(key.frameIndex * ratio);
            }
            foreach (var key in curve.localPosition[2].keys)
            {
                key.frameIndex = Mathf.RoundToInt(key.frameIndex * ratio);
            }
            foreach (var key in curve.timeCurve.keys)
            {
                key.frameIndex = Mathf.RoundToInt(key.frameIndex * ratio);
            }
        }
        ASClipTool.GetFrameRange(UIClip.clip);
        UITimeLine.I.frameIndexF *= ratio;
        inputfieldFps.text = "60";
    }
    void OnFpsChange(string s)
    {
        float v;
        var success = float.TryParse(s, out v);
        if (success) UITimeLine.Fps = v;
    }
    public void Pause()
    {
        play = false;
    }
    public void Play()
    {
        play = true;
    }
    public float speed = 1f;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            play = !play;
        }
        if (play)
        {
            UITimeLine.I.frameIndexF += Time.deltaTime * UITimeLine.Fps * speed;
            float end;
            if (!toggleFlip.isOn)
            {
                end = UIClip.clip.frameRange.y;
            }
            else
            {
                end = UIClip.clip.frameRange.y * 2.5f;
            }
            if (toggleLoop.isOn && UITimeLine.I.frameIndexF > end)
            {
                if (togglePingPong.isOn)
                {
                    speed = -Mathf.Abs(speed);
                }
                else
                {
                    UITimeLine.I.frameIndex = UIClip.clip.frameRange.x;
                }
            }
            else if(togglePingPong.isOn && UITimeLine.I.frameIndexF < UIClip.clip.frameRange.x)
            {
                speed = Mathf.Abs(speed);
            }
        }
    }
}
