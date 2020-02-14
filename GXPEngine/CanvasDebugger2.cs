using System;
using System.Collections;
using System.Drawing;
using GXPEngine;
using GXPEngine.Core;

public class CanvasDebugger2 : EasyDraw
{
    private GameObject _drawer;

    public static CanvasDebugger2 Instance;

    private bool _lastDebugVal;

    public CanvasDebugger2(int width, int height, GameObject drawer = null, bool addCollider = true) : base(width,
        height, false)
    {
        width = Game.main.width;
        height = Game.main.height;

        SetOrigin(width * 0.5f, height * 0.5f);

        _drawer = drawer;
        if (_drawer == null)
        {
        }
        else
        {
            _drawer.AddChild(this);
        }

        Instance = this;

        this.name = "CanvasDebugger2_" + GetHashCode();

        Game.main.OnBeforeStep += ThisOnBeforeStep;
    }

    void ThisOnBeforeStep()
    {
        this.Clear(Color.FromArgb(0, 1, 1, 1));
    }
    
    public void DrawLine(Vector2 p0, Vector2 p1, Color pColor, Camera cam = null)
    {
        DrawLine(p0.x, p0.y, p1.x, p1.y, pColor, cam);
    }

    public void DrawLine(float x1, float y1, float x2, float y2, Color pColor, Camera cam = null)
    {
        if (MyGame.Debug == false)
        {
            return;
        }

        UpdateXYWithCamera(cam);

        float xx1 = x1 - x + MyGame.HALF_SCREEN_WIDTH;
        float yy1 = y1 - y + MyGame.HALF_SCREEN_HEIGHT;

        float xx2 = x2 - x + MyGame.HALF_SCREEN_WIDTH;
        float yy2 = y2 - y + MyGame.HALF_SCREEN_HEIGHT;

        this.Stroke(pColor);
        this.Line(xx1, yy1, xx2, yy2);
    }

    public void DrawEllipse(float x1, float y1, float w, float h, Color pColor, Camera cam = null)
    {
        if (MyGame.Debug == false)
        {
            return;
        }

        UpdateXYWithCamera(cam);

        this.Stroke(pColor);
        this.NoFill();
        this.ShapeAlign(CenterMode.Center, CenterMode.Center);
        this.Ellipse(x1 - x + MyGame.HALF_SCREEN_WIDTH,
            y1 - y + MyGame.HALF_SCREEN_HEIGHT, w, h);
    }

    public void DrawRect(float x1, float y1, float w, float h, Color pColor, bool noFilled = true, Camera cam = null)
    {
        if (MyGame.Debug == false)
        {
            return;
        }

        UpdateXYWithCamera(cam);

        this.ShapeAlign(CenterMode.Min, CenterMode.Min);
        Stroke(pColor);
        if (noFilled)
        {
            NoFill();
        }
        else
        {
            Fill(pColor);
        }
        this.Rect(x1 - x + MyGame.HALF_SCREEN_WIDTH - w * 0.5f,
            y1 - y + MyGame.HALF_SCREEN_HEIGHT - h * 0.5f, w, h);
    }

    public void DrawText(string pText, float pX, float pY, Color pColor, bool relativeToCamera = false,
        Camera cam = null)
    {
        if (MyGame.Debug == false)
        {
            return;
        }

        UpdateXYWithCamera(cam);

        float posX = (relativeToCamera) ? pX : pX - x + MyGame.HALF_SCREEN_WIDTH;
        float posY = (relativeToCamera) ? pY : pY - y + MyGame.HALF_SCREEN_HEIGHT;

        this.Fill(pColor);
        this.Text(pText, posX, posY);
    }

    void UpdateXYWithCamera(Camera cam)
    {
        if (cam != null || (cam = MyGame.ThisInstance.Camera) != null)
        {
            x = cam.x;
            y = cam.y;
        }
    }

    public GameObject Drawer
    {
        get => _drawer;
        set
        {
            _drawer = value;
            if (value != null)
            {
                _drawer.AddChild(this);
            }
        }
    }
}