using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(ScoreReader))]
public class ScoreReaderEditor : E_ShowButtons<ScoreReader> { }
#endif
public enum Note
{
    E0, E1, E2, E3, E4, E5, E6, E7, E8, E9, E10, E11, E12, E13, E14, E15, E16, E17, E18,
    A0, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15, A16, A17, A18,
    D0, D1, D2, D3, D4, D5, D6, D7, D8, D9, D10, D11, D12, D13, D14, D15, D16, D17, D18,
    G0, G1, G2, G3, G4, G5, G6, G7, G8, G9, G10, G11, G12, G13, G14, G15, G16, G17, G18,
    Empty,
    H0, H1, H2, H3, H4, H5, H6, H7, H8, H9, H10, H11, H12, H13, H14, H15, H16, H17, H18,
}
[Serializable]
public class Bar
{
    public List<Note> notes;
    public Bar() { notes = new List<Note>(); }
}
[Serializable]
public class Para
{
    public List<Bar> bars;
    public Para() { bars = new List<Bar>(); }
}
public class ScoreReader : MonoBehaviour
{
    public Para[] paras; // 重复使用的段落
    public List<Bar> bars;
    public TextAsset textAsset;
    int _int1;
    int _int2;
    int _int3; // repeat count
    int _int4; // repeat 0==note, 1==para
    public string _word;
    Para _para;
    Para _paraLast;
    Note _note;
    ReadSymbol _action;
    string _assign;
    delegate ReadSymbol ReadSymbol(string txt);
    ReadSymbol readSymbol;
    public bool logReadSymbol;

    public int beatTotal;

    public int bpb;
    public int npb;
    public int sbn;
    [ShowButton("Read")]
    public void Init()
    {
        _para = null;
        paras = new Para[7];
        //paras = new List<Para>();
        bars = new List<Bar>();
        bars.Add(new Bar());
        readSymbol = ReadNote;

        //var text = textAsset.text;
        foreach (var b in textAsset.bytes)
        //for (int i = 0; i < text.Length; i++)
        {
            var txt = System.Text.Encoding.ASCII.GetString(new byte[] { b });
            //var txt = text.Substring(i, 1);
            readSymbol = readSymbol(txt);
        }

        ClearEmptyBars();
        CountBeats();
    }
    public Note this[int beatIdx]
    {
        get
        {
            int i = 0;
            foreach (var bar in bars)
            {
                foreach (var n in bar.notes)
                {
                    if (beatIdx == i++)
                    {
                        beatIdx++;
                        return n;
                    }
                }
            }
            return Note.Empty;
        }
    }
    void CountBeats()
    {
        beatTotal = 0;
        foreach (var bar in bars)
        {
            foreach (var n in bar.notes)
            {
                beatTotal++;
            }
        }
    }
    private void ClearEmptyBars()
    {
        var list = new List<Bar>();
        foreach (var bar in bars)
        {
            if (bar.notes.Count > 0) list.Add(bar);
        }
        bars = list;
    }

    ReadSymbol ReadQuotePara(string txt)
    {
        if (logReadSymbol) Debug.Log("ReadQuotePara");
        switch (txt)
        {
            case "_": _int1 = 1; break;
            case "A": _int3 = 0; break;
            case "B": _int3 = 1; break;
            case "C": _int3 = 2; break;
            case "D": _int3 = 3; break;
            case "E": _int3 = 4; break;
            case "F": _int3 = 5; break;
            case "G": _int3 = 6; break;
            case ",":
                _action = ReadQuotePara;
                return ReadNum;
            case "]":
                int i = 1;
                foreach (var bar in paras[_int3].bars)
                {
                    if (i++ > _int2) break;
                    bars.Add(bar);
                }
                _int4 = 1;
                _paraLast = paras[_int3];
                return ReadNote;
            default: throw null;
        }
        return ReadQuotePara;
    }
    private ReadSymbol ReadNum(string txt)
    {
        if (logReadSymbol) Debug.Log("ReadNum");
        switch (txt)
        {
            case "0":
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "9":
                _int2 = int.Parse(txt);
                return _action;
            default: throw new Exception("Wrong Num.");
        }
    }

    ReadSymbol ReadDefPara(string txt)
    {
        if (logReadSymbol) Debug.Log("ReadDefPara");
        switch (txt)
        {
            case "_": _int1 = 1; break;
            case "A": _int3 = 0; break;
            case "B": _int3 = 1; break;
            case "C": _int3 = 2; break;
            case "D": _int3 = 3; break;
            case "E": _int3 = 4; break;
            case "F": _int3 = 5; break;
            case "G": _int3 = 6; break;
            case ")":
                if (_int1 == 0)
                {
                    _para = new Para();
                    _para.bars.Add(new Bar());
                    //_paras.Add(_para);
                    paras[_int3] = _para;
                }
                else // _int1 ==1
                    _para = null;
                return ReadNote;
            default: throw null;
        }
        return ReadDefPara;
    }
    ReadSymbol ReadPitch(string txt)
    {
        if (logReadSymbol) Debug.Log("ReadPitch");
        switch (txt)
        {
            case "0":
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "9":
                _int2 = _int2 * 10 + int.Parse(txt);
                return ReadPitch;
            case " ":
                AddNote(_int1 * 19 + _int2);
                _int4 = 0;
                return ReadNote;
            case ",":
                AddNote(_int1 * 19 + _int2);
                AddBar();
                _int4 = 0;
                return ReadNote;
            default: throw new Exception("Wrong Pitch.");
        }
    }
    private void AddBar()
    {
        if (_para != null)
            _para.bars.Add(new Bar());
        else
            bars.Add(new Bar());
    }
    void AddNote(int note)
    {
        AddNote((Note)note);
    }
    void AddNote(Note note)
    {
        _note = note;
        List<Note> ns;
        if (_para != null)
            ns = _para.bars.Last().notes;
        else
            ns = bars.Last().notes;
        ns.Add(note);
        for (int i = 0; i < sbn; i++)
        {
            ns.Add(Note.Empty);
        }
    }
    void AddBar(Para para)
    {
        //foreach (var bar in para.bars)
        //{
        //    bars.Add(bar.Clone());
        //}
        bars.AddRange(para.bars);
    }
    ReadSymbol ReadComment(string txt)
    {
        //if (logReadSymbol) Debug.Log("ReadComment");
        if (_int1 == 1) return ReadNote;
        switch (txt)
        {
            case "*": _action = ReadComment; _assign = "/"; return ReadNext;
            default: return ReadComment;
        }
    }
    ReadSymbol ReadNext(string txt)
    {
        if (txt == _assign) _int1 = 1;
        return _action;
    }

    ReadSymbol ReadNote(string txt)
    {
        if (logReadSymbol) Debug.Log("ReadNote");
        if (txt == Environment.NewLine) return ReadNote;
        switch (txt)
        {
            case " ": break;
            case "-": AddNote(Note.Empty); return ReadNote;
            case "E": _int1 = 0; _int2 = 0; return ReadPitch;
            case "A": _int1 = 1; _int2 = 0; return ReadPitch;
            case "D": _int1 = 2; _int2 = 0; return ReadPitch;
            case "G": _int1 = 3; _int2 = 0; return ReadPitch;
            case ",": AddBar(); break;
            case "(": _int1 = 0; return ReadDefPara;
            case "[": _int1 = 0; _int2 = 99; return ReadQuotePara;
            case "x": _int3 = 0; return ReadRepeat;
            case "\r":
            case "\n": break;
            default: _word = txt; return ReadWord;
        }
        return ReadNote;
    }

    private ReadSymbol ReadRepeat(string txt)
    {
        if (logReadSymbol) Debug.Log("ReadRepeat");
        switch (txt)
        {
            case "0":
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "9":
                _int3 = _int3 * 10 + int.Parse(txt);
                return ReadRepeat;
            case " ":
                // (i=1)减一是因为repeat前已经输入了一次音符
                for (int i = 1; i < _int3; i++)
                {
                    if (_int4 == 0) AddNote(_note);
                    else AddBar(_paraLast);
                }
                return ReadNote;
            case ",":
                for (int i = 1; i < _int3; i++)
                {
                    if (_int4 == 0) AddNote(_note);
                    else AddBar(_paraLast);
                }
                AddBar();
                return ReadNote;
            default: throw new Exception("Wrong RepeatText.");
        }
    }

    ReadSymbol ReadValues(string txt)
    {
        if (logReadSymbol) Debug.Log("ReadValues");
        int value;
        switch (txt)
        {
            case "0":
            case "1":
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "9":
                value = int.Parse(txt);
                break;
            default: throw new Exception("Wrong Value.");
        }
        switch (_int1)
        {
            case 0: bpb = value; return ReadNote;
            case 1: npb = value; return ReadNote;
            case 2: sbn = value; return ReadNote;
            default: throw new Exception("Wrong Param.");
        }
    }
    ReadSymbol DoNothing(string txt) { return DoNothing; }
    ReadSymbol ReadWord(string txt)
    {
        if (logReadSymbol) Debug.Log("ReadWord");
        _word += txt;
        switch (_word)
        {
            case "/*": _int1 = 0; return ReadComment;
            case "bpb=": _int1 = 0; return ReadValues;
            case "npb=": _int1 = 1; return ReadValues;
            case "sbn=": _int1 = 2; return ReadValues;
            case "end": return DoNothing;
            default:
                if (_word.Length < 4) return ReadWord;
                else throw new Exception("Wrong Word: " + _word + ".");
        }
    }
}
