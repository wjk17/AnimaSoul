using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Animatar : MonoBehaviour
{
    public static Clip current
    {
        get
        {
            return _current;
        }
        set { _current = value; }
    }
    private static Clip _current;
    public bool play;
    public string path;

    public List<Clip> clips;
    public string folder = "Chars/Feiqizi/Clips/";

    public float playTime;
    public float timeMulty;
    // Use this for initialization
    void Start()
    {
        playTime = 0;
        LoadClipsInPath();
    }
    public void SetClip(string clipName)
    {
        foreach (var clip in clips)
        {
            if (clip.clipName == clipName)
            {
                current = clip;
            }
        }
    }
    private void LoadClipsInPath()
    {
        var dataPath = Application.dataPath;
        var path = dataPath + "/" + folder;
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] fis = dir.GetFiles("*.clip", SearchOption.TopDirectoryOnly);
        clips = new List<Clip>();
        foreach (var fi in fis)
        {
            var c = Serializer.XMLDeSerialize<Clip>(fi.FullName);
            c.clipName = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);

            ClipTool.GetPairs(c.curves);
            ClipTool.GetFrameRange(c);

            clips.Add(c);
        }
    }
    private void PlayEveryFrame()
    {
        if (play)
        {
            playTime += Time.deltaTime * timeMulty;
            //if (playTime > clip.curves.max)
            if (playTime > 120) playTime = 0;
            foreach (var curve in current.curves)
            {
                if (curve.ast != null) curve.ast.euler = curve.Rot(playTime);
            }
        }
    }
    void Update()
    {
        PlayEveryFrame();
    }
}
