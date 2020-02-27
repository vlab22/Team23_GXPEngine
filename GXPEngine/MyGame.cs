using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
// System contains a lot of default C# libraries 
using GXPEngine;
using GXPEngine.GameLocalEvents;
using GXPEngine.OpenGL;
using TiledMapParserExtended;

// GXPEngine contains the engine

public class MyGame : Game
{
    public static bool Debug = false;

#if false
    public const int SCREEN_WIDTH = 1280; //1920
    public const int SCREEN_HEIGHT = 720; //1080
    public const bool FULLSCREEN = false;

#else
    public const int SCREEN_WIDTH = 1920;
    public const int SCREEN_HEIGHT = 1080;
    public const bool FULLSCREEN = false;
#endif

    //public const int SCREEN_WIDTH = 1080;
    //public const int SCREEN_HEIGHT = 1080;
    //public const bool FULLSCREEN = true;

    public static int HALF_SCREEN_WIDTH = SCREEN_WIDTH / 2;
    public static int HALF_SCREEN_HEIGHT = SCREEN_HEIGHT / 2;

    public static MyGame ThisInstance;

    private FollowCamera _camera;

    private string[] _levelFiles = new string[0];

    private Level _currentLevel;
    private int _levelIndex;

    private Map _mapData;

    private MapGameObject _map;
    private CanvasDebugger2 _canvasDebugger;
    private LevelManager _levelManager;
    private ParticleManager _particleManager;

    private bool _debugCreateLag;
    private int _lagLoopSteps = 10;

    private uint _totalScore;

    private List<PlayerScoreData> _scoreBoardList = new List<PlayerScoreData>();

    public MyGame(string[] tmxFileNames, int levelIndex) :
        base(SCREEN_WIDTH, SCREEN_HEIGHT, FULLSCREEN) // Create a window that's 800x600 and NOT fullscreen
    {
        GL.ClearColor(1f, 1f, 1f, 1f);

        ThisInstance = this;

        _levelFiles = tmxFileNames;

        if (_levelFiles.Length == 0)
        {
            throw new ApplicationException(
                $"_levelFiles.Length == 0, no tmx files found in {AppDomain.CurrentDomain.DynamicDirectory}");
        }

        LoadScoreBoardData();
        
        _mapData = TiledMapParserExtended.MapParser.ReadMap(_levelFiles[levelIndex]);

        _map = new MapGameObject(_mapData);

        _camera = new FollowCamera(0, 0, width, height);

        _canvasDebugger = new CanvasDebugger2(width, height);

        AddChild(SoundManager.Instance);

        var hudScreenFader = new HudScreenFader();

        StartScreen();
        
        //var startScreen = new StartScreen();
        //AddChild(startScreen);
    }

    public void ResetLevel(int levelId)
    {
        //CoroutineManager.ClearAllRoutines();

        UnLoadCurrentLevel();

        _currentLevel = new Level(_camera, _map);
        AddChild(_currentLevel);

        SoundManager.Instance.IsSoundEnabled = true;

        _levelManager = new LevelManager(_currentLevel, _totalScore);
        AddChild(_levelManager);

        if (_particleManager == null)
            _particleManager = new ParticleManager();

        AddChild(_particleManager);

        AddChild(_canvasDebugger);
    }

    public void UnLoadCurrentLevel()
    {
        //CoroutineManager.StopAllCoroutines(_camera);

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
            //_currentLevel.RemoveChild(_map);
            _currentLevel.Destroy();
        }
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

            Console.WriteLine(
                $"Lag will reach 20 fps: fps {fps:00.00} | _lagLoopSteps: {_lagLoopSteps} | Press \"L\" to disable lag");
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

    public uint TotalScore
    {
        get => _totalScore;
        set => _totalScore = value;
    }

    public void Close()
    {
        _glContext.Close();
    }

    public void NextLevel()
    {
        _levelIndex++;
        _levelIndex = _levelIndex % _levelFiles.Length;
        LoadLevel(_levelIndex);
    }

    public void LoadLevel(int levelIndex)
    {
        _currentLevel?.RemoveChild(_map);

        _mapData = TiledMapParserExtended.MapParser.ReadMap(_levelFiles[levelIndex]);
        _map = new MapGameObject(_mapData);

        ResetLevel(levelIndex);
    }

    public void StartScreen()
    {
        UnLoadCurrentLevel();

        //var startScreen = new StartScreen();
        //AddChildAt(startScreen, HudScreenFader.instance.Index - 1);
        
        var gameOverScreen = new GameOverScreen();
        AddChildAt(gameOverScreen, HudScreenFader.instance.Index - 1);
    }

    void LoadScoreBoardData()
    {
        //Load File
        string[] valuesLines = File.ReadAllLines("data/Players Score.txt");

        for (int i = 0; i < valuesLines.Length; i++)
        {
            var line = valuesLines[i];
            var lineSplit = line.Split('=');
            if (lineSplit.Length != 2)
            {
                throw new Exception("data/Players Score.txt incorrectly config, all lines must be key=value");
            }

            string key = lineSplit[0].Trim();
            string val = lineSplit[1].Trim();

            if (string.IsNullOrWhiteSpace(key))
            {
                Console.WriteLine($"Error: {key} not defined");
                continue;
            }

            if (!uint.TryParse(val, out uint score))
            {
                Console.WriteLine($"Error: {val} not a valid positive integer");
                continue;
            }

            var scoreData = new PlayerScoreData()
            {
                name = key,
                score = score
            };

            _scoreBoardList.Add(scoreData);
        }

        //Sort
        _scoreBoardList = _scoreBoardList.OrderByDescending(pd => pd.score).ToList();
    }

    public List<PlayerScoreData> ScoreBoardList => _scoreBoardList;
}

public struct PlayerScoreData
{
    public uint score;
    public string name;
}