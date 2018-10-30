using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI控件
public partial class UITimeLine : MonoSingleton<UITimeLine>
{
    public static float Fps
    {
        get { return I.fps; }
        set { I.fps = value; }
    }
    public static float timePerFrame
    {
        get { return 1 / Fps; }
    }
    public float fps = 60;
    public Color clrTimeLine = Color.green;
    public Color clrGrid = Palette.L1;
    //    public float realtime;
    //    public float normalizedTime;
    public int xSpaceTextInRuler; // 标尺每隔多少帧有一个帧数数字
    public int xSpaceLineInRuler; // 多少帧画一条线
    public float rulerScalerSensitivity = 20;
    //    public Canvas canvas;
    //    private RectTransform area;
    //    private RectTransform topBar;
    //    private RectTransform ruler;

    private float leftTimer;
    private float rightTimer;
    private float upTimer;
    private float downTimer;
    public float continuousKeyTime = 0.5f; // 上下左右键连发延迟
    public float continuousKeyInterval = 0.01f; // 间隔（其实0.01通常约等于每帧触发）
    public Text txtFrameIdx;
    public Text txtFrameIdxN;
    //    public Vector3 mousePos;
    //    public Rect uiRect;
    //    public Vector2 pos2D;
    //    public int fontSize;

    //    public float lineWidth = 5;
    //    public Vector2 range
    //    {
    //        get { return endPos - startPos; }
    //    }
    //    public Vector2 endPos
    //    {
    //        get { return startPos + new Vector2(rulerLength, 0); }
    //    }
    //    public Vector2Int startPosInt
    //    {
    //        get { return new Vector2Int(Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.y)); }
    //    }
    //    private Vector2 startPos;

    //    public static float FrameValue;
    public int frameIdx
    {
        get { return Mathf.RoundToInt(frameIdx_F); }
        set { frameIdx_F = value; }
    }
    [SerializeField] float _frameIdx_F;
    public System.Action<int> onFrameIdxChanged;
    //[MAD.ShowProperty(MAD.ShowPropertyAttribute.EValueType.Float)]
    public float frameIdx_F
    {
        get { return _frameIdx_F; }
        set
        {
            _frameIdx_F = value;
            txtFrameIdx.text = "帧：" + frameIdx.ToString();
            txtFrameIdxN.text = "n：" + frameIdxN.ToString("0.00");
            if (onFrameIdxChanged != null) onFrameIdxChanged(frameIdx);
        }
    }
    public float frameIdxN
    {
        get
        {
            var end = UIClip.I.clip.frameRange.y;
            return end == 0 ? 0 : MathTool.Round(frameIdx_F / end, indexNAccuracy);
        }
    }
    public int indexNAccuracy = 3;

    public InsertKeyType insertType;
    public enum InsertKeyType
    {
        EulPos,
        Eul,
        Pos,
    }
    List<Command> cmds;
    private void OnGUI()
    {
        if (cmds.NotEmpty())
        {
            CommandHandler hdl = new IMUIHandler(transform as RectTransform);
            hdl.commands = cmds;
            hdl.Execute();
        }
    }
    public Vector2 c;
    public Vector2 SIZE = new Vector2(20, 15);
    Vector2Int SizeInt { get { return SIZE.ToInt(); } }
    private void OnRenderObject()
    {
        var pos = rtAreaPos / UI.scaler.referenceResolution;
        var scl = rtAreaSize / UI.scaler.referenceResolution / SizeInt;
        var matrixAreaToRect = Matrix4x4.TRS(pos, Quaternion.identity, scl);

        var matrixRectToRef = Matrix4x4.TRS(rtPos, Quaternion.identity, rtSize / SizeInt);
        var matrixRulerToRef = Matrix4x4.TRS(rtPos, Quaternion.identity, rtSize / SizeInt);

        var a = Vector2.zero;
        var b = Vector2.up * SIZE.y;
        var f = 1f / SIZE.x;
        cmds = new List<Command>();
        // grid
        for (int i = 0; i < SIZE.x; i++)
        {
            if ((i % xSpaceLineInRuler) == 0)
            {
                a.x = b.x = i;
                DrawLine(a, b, I.clrGrid, matrixAreaToRect);
            }
            if ((i % xSpaceTextInRuler) == 0)
            {
                a.x = i + f * 0.5f;
                c = matrixRectToRef.MultiplyPoint(a);
                c = rulerPos + c.ToLT();
                var cmd = IMUI.Cmd(IMUICmdType.DrawText, i.ToString(), c, Vectors.half2d);// 画字 帧号标签

                cmds.Add(cmd);
            }
        }
        // timeline
        if (frameIdx.Between(SizeInt.x))
        {
            b.x = a.x = frameIdx;
            DrawLine(a, b, I.clrTimeLine, matrixAreaToRect);
        }
    }
    void DrawLine(Vector2 a, Vector2 b, Color color) // 接口 
    {
        DrawLines.DoDrawLines(color, new Vector2[] { a, b }, new int[] { 0, 1 });
    }
    void DrawLine(Vector2 a, Vector2 b, Color color, Matrix4x4 m) // 接口 
    {
        DrawLines.DoDrawLines(color, new Vector2[] { a, b }, new int[] { 0, 1 }, m);
    }
    void Start()
    {
        this.AddInputCB(GetInput, -5);
        frameIdx = 0;
    }
    //    public Vector2 areaP;
    //    public Vector2 topBarP;
    //    public Vector2 rulerP;
    //    void UpdateASUI()
    //    {
    //        float rulerX;
    //        int num;
    //        areaP = ASUI.AbsRefPos(area);
    //        topBarP = ASUI.AbsRefPos(topBar);
    //        rulerP = ASUI.AbsRefPos(ruler);
    //        Vector2 p;
    //        for (int i = 0; i < rulerLength; i++)
    //        {
    //            num = startPosInt.x + i;
    //            rulerX = i / rulerLength;
    //            var dX = rulerX * ruler.rect.width;
    //            if ((num % xSpaceTextInRuler) == 0)
    //            {
    //                p = new Vector2(rulerP.x + dX, rulerP.y + ruler.rect.height * 0.5f);
    //                IMUI.DrawText(num.ToString(), p, Vector2.one * 0.5f); // 画字 帧号标签

    //                p = new Vector2(areaP.x + dX, areaP.y);
    //                GLUI.DrawLine(p, p + Vector2.up * area.rect.height, Color.grey - ASColor.V * 0.3f); // 画线
    //            }
    //            else if ((num % xSpaceLineInRuler) == 0)
    //            {
    //                p = new Vector2(areaP.x + dX, areaP.y);
    //                GLUI.DrawLine(p, p + Vector2.up * area.rect.height, Color.grey - ASColor.V * 0.2f);//画线
    //            }
    //            if (UIClip.I.clip.HasKey(objCurve, num))
    //            {
    //                p = new Vector2(areaP.x + dX, areaP.y);
    //                GLUI.commandOrder = 2;
    //                GLUI.DrawLine(p, p + Vector2.up * area.rect.height, lineWidth * 0.45f, Color.yellow - ASColor.V * 0.2f);//画线
    //            }
    //        }
    //        if (MathTool.Between(frameIdx, startPos.x, endPos.x))
    //        {
    //            rulerX = (frameIdx - startPos.x) / rulerLength;
    //            p = new Vector2(areaP.x + rulerX * area.rect.width, areaP.y);
    //            GLUI.commandOrder = 1;
    //            GLUI.DrawLine(p, p + Vector2.up * area.rect.height, lineWidth, Color.green);
    //        }
    //    }
    //    private void MouseDown(MB button)
    //    {
    //        use = true;
    //        MouseDrag(button);
    //    }
    //    public float lx;
    //    private void MouseDrag(MB button)
    //    {
    //        use = true;
    //        if (!ASUI.MouseOver(area)) return;
    //        var deltaV = ASUI.mousePositionRef - oldPos;
    //        deltaV = MathTool.Divide(deltaV, area.rect.size);
    //        deltaV = Vector2.Scale(deltaV, new Vector2(rulerLength, 0));
    //        switch (button)
    //        {
    //            case MB.Left:

    //                lx = ASUI.mousePositionRef.x - area.anchoredPosition.x;
    //                lx = lx / area.rect.width;
    //                lx = Mathf.Clamp01(lx);
    //                frameIdx = (int)startPos.x + Mathf.RoundToInt(lx * rulerLength);
    //                break;

    //            case MB.Right:

    //                break;

    //            case MB.Middle:

    //                startPos -= deltaV;
    //                break;
    //            default: throw null;
    //        }
    //        oldPos = ASUI.mousePositionRef;
    //    }
    //    Vector2 oldPos;
    //    bool use, left, right, middle, shift, ctrl;
    //    public bool over;
    void GetInput()
    {
        //        var shift = Events.Shift;
        //        var ctrl = Events.Ctrl;
        //        var alt = Events.Alt;
        var use = false;
        //        over = ASUI.MouseOver(area, ruler);
        //        var simMidDown = Events.MouseDown(MB.Left) && alt;
        //        if ((Events.MouseDown(MB.Middle) || simMidDown) && over) { oldPos = ASUI.mousePositionRef; MouseDown(MB.Middle); middle = true; }
        //        if (Events.MouseDown(MB.Left) && over && !simMidDown) { oldPos = ASUI.mousePositionRef; MouseDown(MB.Left); left = true; }
        //        var simMid = Events.Mouse(MB.Left) && alt;
        //        if (!Events.Mouse(MB.Middle) && !simMid) middle = false;
        //        if (!Events.Mouse(MB.Left) || simMid) left = false;
        //        if (middle) MouseDrag(MB.Middle);
        //        if (left) MouseDrag(MB.Left);
        float delta = Events.AxisMouseWheel;
        if (delta != 0 && ASUI.MouseOver(rt, ruler))
        {
            use = true;
            SIZE.x -= delta * rulerScalerSensitivity;
            SIZE.x = Mathf.Clamp(SIZE.x, 10, Mathf.Infinity);
        }
        if (Events.Key(KeyCode.LeftArrow))
        {
            leftTimer += Time.deltaTime;
        }
        else { leftTimer = 0; }
        if (Events.Key(KeyCode.RightArrow))
        {
            rightTimer += Time.deltaTime;
        }
        else { rightTimer = 0; }
        if (Events.Key(KeyCode.UpArrow))
        {
            upTimer += Time.deltaTime;
        }
        else { upTimer = 0; }
        if (Events.Key(KeyCode.DownArrow))
        {
            downTimer += Time.deltaTime;
        }
        else { downTimer = 0; }
        if (leftTimer > continuousKeyTime || Events.KeyDown(KeyCode.LeftArrow))
        {
            leftTimer -= continuousKeyInterval;
            frameIdx--;
        }
        else if (rightTimer > continuousKeyTime || Events.KeyDown(KeyCode.RightArrow))
        {
            rightTimer -= continuousKeyInterval;
            frameIdx++;
        }
        else if (upTimer > continuousKeyTime * 1.5f || Events.KeyDown(KeyCode.UpArrow))
        {
            upTimer -= continuousKeyInterval * 1.5f;
            if (UIClip.I.clip.curves.Count > 0)
            {
                var keys = UIClip.I.clip.curves[0].pos.x.keys;
                for (int i = keys.Count - 1; i >= 0; i--)
                {
                    if (keys[i].time < frameIdx)
                    {
                        frameIdx = Mathf.RoundToInt(keys[i].time);
                        break;
                    }
                }
            }
        }
        else if (downTimer > continuousKeyTime * 1.5f || Events.KeyDown(KeyCode.DownArrow))
        {
            downTimer -= continuousKeyInterval * 1.5f;
            if (UIClip.I.clip.curves.Count > 0)
            {
                var keys = UIClip.I.clip.curves[0].pos.x.keys;
                for (int i = 0; i < keys.Count; i++)
                {
                    if (keys[i].time > frameIdx)
                    {
                        frameIdx = Mathf.RoundToInt(keys[i].time);
                        break;
                    }
                }
            }
        }
        if (Events.KeyDown(KeyCode.I))
        {
            if (Events.Alt)
                RemoveKey();
            else InsertKey();
        }
        if (Events.Mouse1to3) use = true;
        if (use) Events.Use();
    }
    public void RemoveKey()
    {
        RemoveKeyAt(frameIdx);
    }
    public void RemoveKeyAt(float time)
    {
        switch (insertType)
        {
            case InsertKeyType.EulPos: UIClip.I.clip.RemoveEulerPosAllCurve(time); break;
            case InsertKeyType.Eul: break;
            case InsertKeyType.Pos: break;
            default: throw null;
        }
        ClipTool.GetFrameRange(UIClip.I.clip);
    }
    public void InsertKey()
    {
        switch (insertType)
        {
            case InsertKeyType.EulPos: UIClip.I.clip.AddEulerPosAllCurve(frameIdx); break;
            case InsertKeyType.Eul: break;
            case InsertKeyType.Pos: break;
            default: throw null;
        }
        ClipTool.GetFrameRange(UIClip.I.clip);
    }
}