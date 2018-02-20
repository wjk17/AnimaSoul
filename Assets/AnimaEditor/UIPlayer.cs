using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    public static UIPlayer I
    {
        get
        {
            if (_i == null) _i = FindObjectOfType<UIPlayer>(); return _i;
        }
    }
    private static UIPlayer _i;
    public UIButtonSwitch buttonPlayPuase;
    public InputField inputfieldFps;
    public Toggle toggleLoop;
    public Toggle toggleFlip;
    public bool play;
    void Start()
    {
        buttonPlayPuase.Reg(Play, Pause);
        inputfieldFps.onValueChanged.AddListener(OnFpsChange);
        inputfieldFps.contentType = InputField.ContentType.DecimalNumber;
        inputfieldFps.text = UITimeLine.I.fps.ToString();
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
    void Update()
    {
        if (play)
        {
            UITimeLine.I.frameIndexF += Time.deltaTime * UITimeLine.Fps;
            float end;
            if(!toggleFlip.isOn)
            {
                end = UIClip.clip.frameRange.y;
            }
            else
            {
                end = UIClip.clip.frameRange.y * 2.5f;
            }
            if (toggleLoop.isOn && UITimeLine.I.frameIndexF > end)
            {
                UITimeLine.I.frameIndex = UIClip.clip.frameRange.x;
            }
        }
    }
}
