using System;
using System.Drawing;
// System contains a lot of default C# libraries 
using GXPEngine;
using TiledMapParserExtended;

// GXPEngine contains the engine

public class MyGame : Game
{
    public static bool Debug = true;

    public const int SCREEN_WIDTH = 1280;
    public const int SCREEN_HEIGHT = 720;
    
    public static int HALF_SCREEN_WIDTH = SCREEN_WIDTH / 2;
    public static int HALF_SCREEN_HEIGHT = SCREEN_HEIGHT / 2;

    public static MyGame ThisInstance;

    private FollowCamera _camera;

    private string[] _levelFiles = new []{"Level00.tmx"};

    private Level _currentLevel;

    private Map _mapData;

    private MapGameObject _map;
    private CanvasDebugger2 _canvasDebugger;
    private LevelManager _levelManager;
    private ParticleManager _particleManager;

    public MyGame() : base(SCREEN_WIDTH, SCREEN_HEIGHT, false) // Create a window that's 800x600 and NOT fullscreen
    {
        ThisInstance = this;

        _mapData = TiledMapParserExtended.MapParser.ReadMap(_levelFiles[0]);
        
        _map = new MapGameObject(_mapData);
        
        _camera = new FollowCamera(0, 0, width, height);

        _canvasDebugger = new CanvasDebugger2(width, height);
        
        ResetLevel(0);
    }

    public void ResetLevel(int levelId)
    {
        if (_currentLevel != null)
        {
            RemoveChild(_canvasDebugger);
            RemoveChild(_currentLevel);
            RemoveChild(_levelManager);
            RemoveChild(_particleManager);
            _levelManager.Destroy();
            _currentLevel.RemoveChild(_camera);
            _currentLevel.RemoveChild(_map);
            _currentLevel.Destroy();
        }

        _currentLevel = new Level(_camera, _map);
        AddChild(_currentLevel);
        
        _levelManager = new LevelManager(_currentLevel);
        AddChild(_levelManager);
        
        _particleManager = new ParticleManager();
        AddChild(_particleManager);
        
        AddChild(_canvasDebugger);
    }

    void Update()
    {
        CoroutineManager.Tick(Time.deltaTime);

        if (Input.GetKeyDown(Key.R))
        {
            ResetLevel(0);
        }
        if (Input.GetKeyDown(Key.O))
        {
            MyGame.Debug = ! MyGame.Debug;
        }
    }

    static void Main() // Main() is the first method that's called when the program is run
    {
        new MyGame().Start(); // Create a "MyGame" and start it
    }

    public FollowCamera Camera => _camera;
}