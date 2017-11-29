using UnityEngine;
using UnityEngine.UI;

// UI控件
public class UITimeLine : MonoBehaviour
{
    public float realtime;
    public float normalizedTime;
    public int xSpaceInRuler; // 标尺每隔多少帧有一个帧数数字
    public int xSpaceLineInRuler; // 多少帧画一条线
    public float XSpaceInArea; // 时间线指针UI的移动步长
    public Canvas canvas;
    private RectTransform area;
    private RectTransform ruler;

    private float xFactor;
    private float leftTimer;
    private float rightTimer;
    public float continuousKeyTime = 0.5f; // 上下左右键连发延迟
    public float continuousKeyInterval = 0.01f; // 间隔（其实0.01通常约等于每帧触发）
    public Text uiFrameIndex;
    public bool hover;
    public Vector3 mousePos;
    public Rect uiRect;
    public Vector2 pos2D;
    public int fontSize;
    public float rulerScalerSensitivity = 1f;
    public float rulerLength = 200;

    private int _frameIndex;
    public int frameIndex
    {
        get
        {
            return _frameIndex;
        }
        set
        {
            _frameIndex = value;
            uiFrameIndex.text = "帧：" + _frameIndex.ToString();
        }
    }
    UIDOFEditor dofe;
    public string path;
    public string folder = "Clips/";
    public string fileName = "default.xml";
    string rootPath { get { return Application.dataPath + "/../"; } }
    public ASClip clip;
    public InsertKeyType insertType;
    public enum InsertKeyType
    {
        EulPos,
        Eul,
        Pos,
    }
    ASObjectCurve curve
    {
        get { return clip[trans]; }
    }
    Transform trans
    {
        get { return dofe.ast.transform; }
    }
    Vector3 euler
    {
        get { return dofe.ast.euler; }
    }
    Vector3 pos
    {
        get { return trans.localPosition; }
    }
    void Start()
    {
        frameIndex = 0;
        xFactor = 1 / XSpaceInArea;
        area = transform.Search("Area") as RectTransform;
        ruler = transform.Search("Ruler") as RectTransform;
        var mouse = area.gameObject.AddComponent<UIMouseEventWrapper>();
        mouse.CreateBox2D();
        mouse.onMouseDown = MouseDown;
        mouse.onMouseDrag = MouseDrag;

        dofe = FindObjectOfType<UIDOFEditor>();
        InitClip();
        InitASUI();
    }
    private void InitClip()
    {
        clip = new ASClip();
        foreach (var ast in dofe.avatar.setting.asts)
        {
            clip.AddCurve(ast.transform);
        }
    }
    private void InitASUI()
    {
        IMUI.fontSize = fontSize;
        ASUI.parent = area;
        ASUI.BeginHorizon();
        ASUI.EndHorizon();
    }
    public float lineWidth = 2;
    void UpdateASUI()
    {
        ASUI.owner = this;
        GLUI.BeginOrtho();

        float x, y, rulerX;
        Vector2 p;
        for (int i = 0; i < 1000; i++)
        {
            rulerX = i / rulerLength;
            if ((i % xSpaceInRuler) == 0)
            {
                x = rulerX * ruler.sizeDelta.x;
                if (x + IMUI.CalSize(i.ToString()).x > ruler.sizeDelta.x) break;
                x += ruler.anchoredPosition.x;
                y = -ruler.anchoredPosition.y;
                p = new Vector2(x, y);
                IMUI.DrawText(i.ToString(), p, Vector2.one * 0.5f);
                y = -area.anchoredPosition.y;
                p = new Vector2(x, y);
                GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, Color.grey - ASColor.V * 0.3f);
            }
            else if ((i % xSpaceLineInRuler) == 0)
            {
                x = rulerX * area.sizeDelta.x;
                if (x > area.sizeDelta.x) break;
                x += area.anchoredPosition.x;
                y = -area.anchoredPosition.y;
                p = new Vector2(x, y);
                GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, Color.grey - ASColor.V * 0.2f);
            }
            if (clip.HasKey(curve, i))
            {
                x = rulerX * area.sizeDelta.x;
                if (x > area.sizeDelta.x) break;
                x += area.anchoredPosition.x;
                y = -area.anchoredPosition.y;
                p = new Vector2(x, y);
                GLUI.commandOrder = 2;
                GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, lineWidth * 0.45f, Color.yellow - ASColor.V * 0.2f);
            }
        }
        rulerX = frameIndex / rulerLength;
        x = rulerX * area.sizeDelta.x;
        if (x > area.sizeDelta.x) return;
        x += area.anchoredPosition.x;
        y = -area.anchoredPosition.y;
        p = new Vector2(x, y);
        GLUI.commandOrder = 1;
        GLUI.DrawLine(p, p + Vector2.up * area.sizeDelta.y, lineWidth, Color.green + ASColor.H * 0.2f);
    }
    void Update()
    {
        UpdateASUI();
        float delta = Input.GetAxis("Mouse ScrollWheel");
        if (delta != 0)
        {
            rulerLength += delta * rulerScalerSensitivity;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            leftTimer += Time.deltaTime;
        }
        else { leftTimer = 0; }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rightTimer += Time.deltaTime;
        }
        else { rightTimer = 0; }
        if (leftTimer > continuousKeyTime || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftTimer -= continuousKeyInterval;
            frameIndex--;
        }
        else if (rightTimer > continuousKeyTime || Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightTimer -= continuousKeyInterval;
            frameIndex++;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                RemoveKey();
            }
            else
            {
                InsertKey();
            }
        }
    }
    public void Load()
    {
        path = rootPath + folder + fileName;
        clip = Serializer.XMLDeSerialize<ASClip>(path);
    }
    [ContextMenu("Save")]
    public void Save()
    {
        path = rootPath + folder + fileName;
        Serializer.XMLSerialize(clip, path);
    }
    private void InsertKey()
    {
        switch (insertType)
        {
            case InsertKeyType.EulPos: clip.AddEulerPos(curve, frameIndex, euler, pos); break;
            case InsertKeyType.Eul: break;
            case InsertKeyType.Pos: break;
            default: throw null;
        }
    }
    private void RemoveKey()
    {
        clip.RemoveKey(curve, frameIndex);
    }
    private void MouseDown()
    {
        MouseDrag();
    }
    private void MouseDrag()
    {
        var lx = Input.mousePosition.x * IMUI.facterToReference - area.anchoredPosition.x;
        lx = lx / area.sizeDelta.x;
        lx = Mathf.Clamp01(lx);
        frameIndex = Mathf.RoundToInt(lx * rulerLength);
    }
}