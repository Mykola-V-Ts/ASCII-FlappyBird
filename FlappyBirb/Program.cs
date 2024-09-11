using System;
using System.Threading;
using System.Diagnostics;

namespace SoftwareEngineering
{
    //Active part of the console
    class ScreenSize
    {
        public static int x = 40;   //Screen width
        public static int y = 40;   //Screen Height
    }

    //Obstacle
    class Pipe
    {
        public static int width = 14;       //Thickness of a pipe
        public static int interval;         //Distance between pipes
        public static int countdown;            //Timer for pipe spawning. Ticks are called in Scroll()

        public static int hole;             //Position of a hole in an obstacle (0 is the bottom). Height at which lower part of a pipe ends
        public static int holeHeight = 9;   //Hieght of a hole. hole + holeHeight is where top part of a pipe begins
    }

    class Player
    {
        public static bool flap = false;    //When is true the player gets an impulse

        public static float altitude;       //Current vertical position of the player

        public static float speed = 0f;     //Vertical speed of the player

        public static float g = 0.003f;       //Gravity

        public static float impulse = 0.05f; //Bird's "jump" power
    }

    class Program
    {
        static char[,] screen;              //Game space

        static Random rand;                 //Seed for a "hole"

        static bool end = false;            //Flag for the game end

        static int score = 0;               //Score. Rises when a new pipe has been created

        static public float scrollTimer;

        static public int tickTime = 16;    //tick time in milliseconds for fixed update

        static public Stopwatch Time = new Stopwatch();
        static public Stopwatch pipeTime = new Stopwatch();
        static public Stopwatch totalTime = new Stopwatch();

        //static public string tmp = "\0";    //Output buffer

        static void Main(string[] args)
        {
            //Set up the window
            Console.SetWindowSize(ScreenSize.x + 1, ScreenSize.y + 1);

            Console.SetBufferSize(ScreenSize.x + 1, ScreenSize.y + 1);

            screen = new char[ScreenSize.x, ScreenSize.y];

            //Random generator for holes
            rand = new Random();
            Pipe.hole = rand.Next(3, ScreenSize.y - Pipe.holeHeight - 1);

            Console.CursorVisible = false;

            //var deltaTime = new Stopwatch();
            Time.Start();
            pipeTime.Start();
            totalTime.Start();

            Thread t = new Thread(FixedUpdate);
            t.Start();

            while (true)
            {
                //Calculating the distance between pipes
                Pipe.interval = Pipe.width * 2;

                //Setting timer for pipe spawner
                Pipe.countdown = Convert.ToInt32(Pipe.interval * 0.5);

                Player.altitude = ScreenSize.y * 0.4f;

                scrollTimer = 30;

                //Initial screen
                for (int x = 0; x < ScreenSize.x; x++)
                {
                    for (int y = 0; y < ScreenSize.y; y++)
                    {
                        if(y == ScreenSize.y - 1)
                            screen[x, y] = '_';
                        else
                            screen[x, y] = ' ';
                    }
                }

            
                //Game loop
                while (!end)
                {
                    //Done as fast as possible
                    Update();

                }

                //Freeze screen until Enter is pressed
                Console.ReadLine();
                end = false;
                score = 0;
                Player.speed = 0f;
            }
        }

        static void Update()
        {

            //Input capture
            if (Console.KeyAvailable)
            {
                //Setting flag and refreshing "KeyAvailable"
                Player.flap = true;
                Console.ReadKey(false);
            }

            //Scroll the screen
            if (pipeTime.ElapsedMilliseconds >= scrollTimer)
                Scroll();

            //Display a new frame
            Draw();
        }

        static void FixedUpdate()
        {
            while (true) {
                if (Time.ElapsedMilliseconds >= tickTime) {

                    //If flag is active apply the impulse
                    if (Player.flap)
                        Player.speed = Player.impulse;

                    //Else apply g
                    else
                        Player.speed -= Player.g;

                    //Calculate new player position
                    Player.altitude -= Player.speed * Time.ElapsedMilliseconds;

                    //Clearing the flag
                    Player.flap = false;

                    //Console.WriteLine("Fixed update call:" + totalTime.ElapsedMilliseconds);

                    //Inverting flag for scrolling
                    //scrollTimer *= (-1);

                    //Wait until next FixedUpdate
                    //Thread.Sleep(tickTime);
                    Time.Restart();
                }
            }
        }

        static void Draw()
        {
            string tmp = "";

            //If player is outside of edges of the screen -- game over
            if (Player.altitude >= ScreenSize.y || Player.altitude < 0)
                end = true;
            else
                //If player hits a pipe -- game over
                if (screen[7, Convert.ToInt32(Math.Truncate(Player.altitude))] == '0')
                    end = true;


            //Fill the console buffer
            for (int y = 0; y < ScreenSize.y; y++)
            {
                //If game is over draw the text
                if (end && y == 12)
                {
                    tmp += "                                        \n";
                    tmp += "                GAME OVER               \n";
                    tmp += "                Score: " + score + "               \n";
                    tmp += "                                        \n";
                    tmp += "           Press ENTER to RETRY         \n";
                    tmp += "                                        ";
                    y += 3;
                }
                else
                    for (int x = 0; x < ScreenSize.x; x++)
                    {
                        //Draw the player
                        if (x == 7 && y == Convert.ToInt32(Player.altitude) && 0.5f < (Player.altitude - Math.Truncate(Player.altitude)))
                            tmp += "P";
                        else
                        if (x == 7 && y == Convert.ToInt32(Player.altitude) && 0.5f >= (Player.altitude - Math.Truncate(Player.altitude)))
                            tmp += "b";

                        //Transfer char from the matrix
                        else
                            tmp += screen[x, y];
                    }

                //Go to the next line
                tmp += "\n";
            }

            //Console.Clear();
            Console.SetCursorPosition(0,0);

            //Output the console buffer
            Console.Write(tmp);

        }

        static void Scroll()
        {
            pipeTime.Restart();

            //Move the game world one character to the left
            for (int x = 0; x < ScreenSize.x - 1; x++)
            {
                for (int y = 0; y < ScreenSize.y; y++)
                {
                    screen[x, y] = screen[x + 1, y];
                }
            }

            //Add a pipe
            if (Pipe.countdown <= 0 && Math.Abs(Pipe.countdown) <= Pipe.width)
            {
                //Add lower part of the pipe
                for (int y = 0; y < Pipe.hole; y++)
                {
                    screen[ScreenSize.x - 1, y] = '0';
                }

                //Add top part of the pipe
                for (int y = Pipe.hole + Pipe.holeHeight; y < ScreenSize.y; y++)
                {
                    screen[ScreenSize.x - 1, y] = '0';
                }
            }

            //If pipe has been fully drawn, restart the spawner timer
            if (Pipe.countdown <= 0 && Math.Abs(Pipe.countdown) > Pipe.width)
            {
                Pipe.countdown = Pipe.interval;
                score++;

                scrollTimer *= 0.97f;

                //Position for the next hole
                Pipe.hole = rand.Next(4, ScreenSize.y - Pipe.holeHeight - 3);
            }

            //Fill the empty space
            if (Pipe.countdown > 0)
            {
                screen[ScreenSize.x - 1, ScreenSize.y - 1] = '_';
                for (int y = 0; y < ScreenSize.y - 1; y++)
                    screen[ScreenSize.x - 1, y] = ' ';
            }

            //Tick the countdown for a pipe spawn
            Pipe.countdown--;
        }
    }
}
