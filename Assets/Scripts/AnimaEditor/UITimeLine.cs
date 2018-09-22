using UnityEngine;
using UnityEngine.UI;

// UI控件
public class UITimeLine : MonoSingleton<UITimeLine>
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
    //    public float realtime;
    //    public float normalizedTime;
    //    public int xSpaceTextInRuler; // 标尺每隔多少帧有一个帧数数字
    //    public int xSpaceLineInRuler; // 多少帧画一条线
    //    public float rulerScalerSensitivity = 20;
    //    public float rulerLength = 200;
    //    public Canvas canvas;
    //    private RectTransform area;
    //    private RectTransform topBar;
    //    private RectTransform ruler;

    //    private float leftTimer;
    //    private float rightTimer;
    //    private float upTimer;
    //    private float downTimer;
    //    public float continuousKeyTime = 0.5f; // 上下左右键连发延迟
    //    public float continuousKeyInterval = 0.01f; // 间隔（其实0.01通常约等于每帧触发）
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
    //    public string path;
    //    public string folder = "Clips/";
    //    public string fileName = "default.xml";
    //    string rootPath { get { return Application.dataPath + "/../"; } }
    public InsertKeyType insertType;
    public enum InsertKeyType
    {
        EulPos,
        Eul,
        Pos,
    }
    //    //public static CurveObj ObjCurve
    //    //{
    //    //    get { return UIClip.I.clip[I.trans]; }
    //    //}
    //    public static CurveObj ObjCurve
    //    {
    //        get { return UIClip.I.clip[I.ast]; }
    //    }
    //    CurveObj objCurve
    //    {
    //        //get { return UIClip.I.clip[trans]; }
    //        get { return UIClip.I.clip[ast]; }
    //    }
    //    ASTransDOF ast
    //    {
    //        get { return UIDOFEditor.I.ast; }
    //    }
    //    //Transform trans
    //    //{
    //    //    get { return UIDOFEditor.I.ast.transform; }
    //    //}
    //    Vector3 euler
    //    {
    //        get { return UIDOFEditor.I.ast.euler; }
    //    }
    //    Vector3 pos
    //    {
    //        //get { return trans.localPosition; }
    //        get { return ast.transform.localPosition; }
    //    }
    //    #endregion




    void Start()
    {
        this.AddInputCB(GetInput, 1);
        frameIdx = 0;
        //        area = transform.Search("Area") as RectTransform;
        //        topBar = transform.Search("TopBar") as RectTransform;
        //        ruler = transform.Search("Ruler X") as RectTransform;
        //        InitASUI();
    }
    //    private void InitASUI()
    //    {
    //        IMUI.fontSize = fontSize;
    //        ASUI.parent = area;
    //        ASUI.BeginHorizon();
    //        ASUI.EndHorizon();
    //    }
    //    public Vector2 areaP;
    //    public Vector2 topBarP;
    //    public Vector2 rulerP;
    //    void UpdateASUI()
    //    {
    //        ASUI.owner = area;
    //        GLUI.BeginOrtho();

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
    //        if (MathTool.Between(frameIndex, startPos.x, endPos.x))
    //        {
    //            rulerX = (frameIndex - startPos.x) / rulerLength;
    //            p = new Vector2(areaP.x + rulerX * area.rect.width, areaP.y);
    //            GLUI.commandOrder = 1;
    //            GLUI.DrawLine(p, p + Vector2.up * area.rect.height, lineWidth, Color.green);
    //        }
    //    }
    //    private void Update()
    //    {
    //        UpdateASUI();
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
    //                frameIndex = (int)startPos.x + Mathf.RoundToInt(lx * rulerLength);
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
        //        use = false;
        //        over = ASUI.MouseOver(area, ruler);
        //        var simMidDown = Events.MouseDown(MB.Left) && alt;
        //        if ((Events.MouseDown(MB.Middle) || simMidDown) && over) { oldPos = ASUI.mousePositionRef; MouseDown(MB.Middle); middle = true; }
        //        if (Events.MouseDown(MB.Left) && over && !simMidDown) { oldPos = ASUI.mousePositionRef; MouseDown(MB.Left); left = true; }
        //        var simMid = Events.Mouse(MB.Left) && alt;
        //        if (!Events.Mouse(MB.Middle) && !simMid) middle = false;
        //        if (!Events.Mouse(MB.Left) || simMid) left = false;
        //        if (middle) MouseDrag(MB.Middle);
        //        if (left) MouseDrag(MB.Left);
        //        float delta = Events.Axis("Mouse ScrollWheel");
        //        if (delta != 0 && ASUI.MouseOver(area, ruler))
        //        {
        //            use = true;
        //            rulerLength -= delta * rulerScalerSensitivity;
        //            rulerLength = Mathf.Clamp(rulerLength, 1, Mathf.Infinity);
        //        }
        //        if (Events.Key(KeyCode.LeftArrow))
        //        {
        //            leftTimer += Time.deltaTime;
        //        }
        //        else { leftTimer = 0; }
        //        if (Events.Key(KeyCode.RightArrow))
        //        {
        //            rightTimer += Time.deltaTime;
        //        }
        //        else { rightTimer = 0; }
        //        if (Events.Key(KeyCode.UpArrow))
        //        {
        //            upTimer += Time.deltaTime;
        //        }
        //        else { upTimer = 0; }
        //        if (Events.Key(KeyCode.DownArrow))
        //        {
        //            downTimer += Time.deltaTime;
        //        }
        //        else { downTimer = 0; }
        //        if (leftTimer > continuousKeyTime || Events.KeyDown(KeyCode.LeftArrow))
        //        {
        //            leftTimer -= continuousKeyInterval;
        //            frameIndex--;
        //        }
        //        else if (rightTimer > continuousKeyTime || Events.KeyDown(KeyCode.RightArrow))
        //        {
        //            rightTimer -= continuousKeyInterval;
        //            frameIndex++;
        //        }
        //        else if (upTimer > continuousKeyTime * 1.5f || Events.KeyDown(KeyCode.UpArrow))
        //        {
        //            upTimer -= continuousKeyInterval * 1.5f;
        //            if (UIClip.I.clip.curves.Count > 0)
        //            {
        //                var keys = UIClip.I.clip.curves[0].timeCurve.keys;
        //                for (int i = keys.Count - 1; i >= 0; i--)
        //                {
        //                    if (keys[i].frameIndex < frameIndex)
        //                    {
        //                        frameIndex = keys[i].frameIndex;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        else if (downTimer > continuousKeyTime * 1.5f || Events.KeyDown(KeyCode.DownArrow))
        //        {
        //            downTimer -= continuousKeyInterval * 1.5f;
        //            if (UIClip.I.clip.curves.Count > 0)
        //            {
        //                var keys = UIClip.I.clip.curves[0].timeCurve.keys;
        //                for (int i = 0; i < keys.Count; i++)
        //                {
        //                    if (keys[i].frameIndex > frameIndex)
        //                    {
        //                        frameIndex = keys[i].frameIndex;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //        if (Events.KeyDown(KeyCode.I))
        //        {
        //            if (Events.Key(KeyCode.LeftAlt) || Events.Key(KeyCode.RightAlt))
        //            {
        //                RemoveKey();
        //            }
        //            else
        //            {
        //                InsertKey();
        //            }
        //        }
        //        if (use) Events.Use();
        //    }
        //    public void Load()
        //    {
        //        path = rootPath + folder + fileName;
        //        UIClip.I.clip = Serializer.XMLDeSerialize<Clip>(path);
        //    }
        //    [ContextMenu("Save")]
        //    public void Save()
        //    {
        //        path = rootPath + folder + fileName;
        //        Serializer.XMLSerialize(UIClip.I.clip, path);
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
    //    private void RemoveKey()
    //    {
    //        UIClip.I.clip.RemoveKey(objCurve, frameIndex);
    //    }
}