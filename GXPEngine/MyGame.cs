using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
// System contains a lot of default C# libraries 
using GXPEngine;
using GXPEngine.OpenGL;
using TiledMapParserExtended;

// GXPEngine contains the engine

public class MyGame : Game
{
    public static bool Debug = false;

    public const int SCREEN_WIDTH = 1280; //1920
    public const int SCREEN_HEIGHT = 720; //1080
    public const bool FULLSCREEN = false;

    // public const int SCREEN_WIDTH = 1920;
    // public const int SCREEN_HEIGHT = 1080;
    // public const bool FULLSCREEN = true;

    //public const int SCREEN_WIDTH = 1080;
    //public const int SCREEN_HEIGHT = 1080;
    //public const bool FULLSCREEN = true;

    public static int HALF_SCREEN_WIDTH = SCREEN_WIDTH / 2;
    public static int HALF_SCREEN_HEIGHT = SCREEN_HEIGHT / 2;

    public static MyGame ThisInstance;

    private FollowCamera _camera;

    private string[] _levelFiles = new string[0];

    private Level _currentLevel;

    private Map _mapData;

    private MapGameObject _map;
    private CanvasDebugger2 _canvasDebugger;
    private LevelManager _levelManager;
    private ParticleManager _particleManager;
    
    private bool _debugCreateLag;
    private int _lagLoopSteps = 10;
    
    public MyGame(string[] tmxFileNames, int levelIndex) :
        base(SCREEN_WIDTH, SCREEN_HEIGHT, FULLSCREEN) // Create a window that's 800x600 and NOT fullscreen
    {
        GL.ClearColor(1f,1f,1f , 1f);
        
        ThisInstance = this;

        _levelFiles = tmxFileNames;

        if (_levelFiles.Length == 0)
        {
            throw new ApplicationException(
                $"_levelFiles.Length == 0, no tmx files found in {AppDomain.CurrentDomain.DynamicDirectory}");
        }

        _mapData = TiledMapParserExtended.MapParser.ReadMap(_levelFiles[levelIndex]);

        _map = new MapGameObject(_mapData);

        _camera = new FollowCamera(0, 0, width, height);

        _canvasDebugger = new CanvasDebugger2(width, height);

        AddChild(SoundManager.Instance);
        
        ResetLevel(0);
    }

    public void ResetLevel(int levelId)
    {
        CoroutineManager.ClearAllRoutines();

        SoundManager.Instance.StopAllSounds();
        
        if (_currentLevel != null)
        {
            HUD.Instance.Reset();
            
            RemoveChild(_canvasDebugger);
            RemoveChild(_currentLevel);
            RemoveChild(_levelManager);
            RemoveChild(_particleManager);

            _particleManager.Reset();
            
            _levelManager.Destroy();
            
            _currentLevel.Hud.Destroy();
            
            _currentLevel.RemoveChild(_camera);
            _currentLevel.RemoveChild(_map);
            _currentLevel.Destroy();
        }

        _currentLevel = new Level(_camera, _map);
        AddChild(_currentLevel);

        _levelManager = new LevelManager(_currentLevel);
        AddChild(_levelManager);

        if (_particleManager == null)
            _particleManager = new ParticleManager();
        else
        {
            
        }
        
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
            MyGame.Debug = !MyGame.Debug;
        }

        if (Input.GetKeyDown(Key.I))
        {
            string s = "Stats:\r\n";
        }
        
        if (Input.GetKeyDown(Key.L))
        {
            _debugCreateLag = !_debugCreateLag;

            _lagLoopSteps = 0;
        }

        if (_debugCreateLag)
        {
            float fps = 1000f / Time.deltaTime;
            if (fps > 20)
            {
                _lagLoopSteps += 100000;
            }

            for (int i = 0; i < _lagLoopSteps; i++)
            {
                //Loop through nothing, but auses lag
            }
            
            Console.WriteLine($"Lag will reach 20 fps: fps {fps:00.00} | _lagLoopSteps: {_lagLoopSteps} | Press \"L\" to disable lag");
        }
        
    }

    private List<GameObject> TransverseChildren(GameObject root)
    {
        var gos = new List<GameObject>();

        return new List<GameObject>();
    }

    static void Main(string[] args) // Main() is the first method that's called when the program is run
    {
        Console.WriteLine($"main args: {string.Join(Environment.NewLine, args.Select(arg => $"'{arg}'"))}");

        var tmxFileNames = TmxFilesLoader.GetTmxFileNames().Where(tmx => !tmx.EndsWith("HUD.tmx")).ToArray();

        int levelIndex = 0;
        if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
        {
            levelIndex = Array.FindIndex(tmxFileNames,
                t => t.Equals(args[0], StringComparison.InvariantCultureIgnoreCase));
        }

        levelIndex = (levelIndex == -1) ? 0 : levelIndex;

        var myGame = new MyGame(tmxFileNames, levelIndex);

        myGame.Start();
    }

    public FollowCamera Camera => _camera;

    public Level CurrentLevel => _currentLevel;

    public void Close()
    {
        _glContext.Close();
    }
}