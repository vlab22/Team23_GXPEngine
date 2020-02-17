using System.Collections;

namespace GXPEngine
{
    public class MyGameFoo : Game
    {
        public const int SCREEN_WIDTH = 1280; //1920
        public const int SCREEN_HEIGHT = 720; //1080
        public const bool FULLSCREEN = false;

        // public const int SCREEN_WIDTH = 1920;
        // public const int SCREEN_HEIGHT = 1080;
        // public const bool FULLSCREEN = true;
    
        public static int HALF_SCREEN_WIDTH = SCREEN_WIDTH / 2;
        public static int HALF_SCREEN_HEIGHT = SCREEN_HEIGHT / 2;

        public MyGameFoo() : base(SCREEN_WIDTH, SCREEN_HEIGHT, FULLSCREEN)
        {
            CoroutineManager.StartCoroutine(FooRoutine(), this);
        }

        private IEnumerator FooRoutine()
        {
            int time = 0;
            while (time < 5000)
            {
                time += Time.deltaTime;
                yield return null;
            }
        }

        void Update()
        {
            CoroutineManager.Tick(Time.deltaTime);

            if (Input.GetKeyDown(Key.R))
            {
              
            }
            if (Input.GetKeyDown(Key.O))
            {
                MyGame.Debug = ! MyGame.Debug;
            }
        
            if (Input.GetKeyDown(Key.I))
            {
                string s = "Stats:\r\n";
            }
        }
        //
        // static void Main(string[] args)
        // {
        //     var fooGame = new MyGameFoo();
        //     
        //     fooGame.Start();
        // }
    }
}