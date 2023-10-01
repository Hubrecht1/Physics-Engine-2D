using static SDL2.SDL;
using System;
using System.Numerics;

namespace Physics_Engine
{
    public static class Main_Window
    {
        static IntPtr window;
        public static IntPtr renderer;
        public static bool running = true;
        public static string windowName = "Physics_Engine";
        public static int windowHeight = 600;
        public static int windowWidth = 600;

        static ulong NOW, LAST = 0;
        static bool firstFrame = true;

        static double deltaTime = 0;
        static uint physicsTPS = 60;
        static uint frameCount = 0;
        static float totalsecondsElapsed = 0;

        //public static event EventHandler Draw;

        //main loop
        static void Main(string[] args)
        {
            Setup();


            while (running)
            {

                LAST = NOW;
                NOW = SDL_GetPerformanceCounter();
                deltaTime = (double)(NOW - LAST) / SDL_GetPerformanceFrequency();

                if (LAST == 0)
                {
                    deltaTime = 0;
                }

#if DEBUG
                frameCount++;
                UInt64 start = SDL_GetPerformanceCounter();

                PollEvents();
                float physicsStart = SDL_GetPerformanceCounter();
                UpdatePhysics();
                float physicsEnd = SDL_GetPerformanceCounter();
                Render();

                UInt64 end = SDL_GetPerformanceCounter();
                float secondsElapsed = (end - start) / (float)SDL_GetPerformanceFrequency();
                float physicsTimeElapsed = (physicsEnd - physicsStart) / SDL_GetPerformanceFrequency();

                totalsecondsElapsed += secondsElapsed;

                if (frameCount % 100 == 0)
                {
                    double averageFPS = Math.Round(100 / totalsecondsElapsed, 2);
                    string line = $"(average)fps {averageFPS}; dt: {Math.Round(deltaTime * 1000, 2)} ms; physics: {Math.Round(physicsTimeElapsed * 1000, 2)} ms";
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(line);
                    totalsecondsElapsed = 0;
                }

#else

                PollEvents();

                Render();


#endif

            }

            CleanUp();
        }


        static void UpdateSystems() //this funtion is called every frame
        {
            float dt = (float)deltaTime;

            TransformSystem.Update(dt);
            ScreenRectangleSystem.Update(dt);
            ScreenCircleSystem.Update(dt);


        }


        /// <summary>
        /// Setup all of the SDL resources we'll need to display a window.
        /// </summary>
        static void Setup()
        {
#if DEBUG
            SDL_SetHint(SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");
#endif
            // Initilizes SDL
            if (SDL_Init(SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine($"There was an issue initializing  {SDL_GetError()}");
            }

            // Create a new window given a title, size, and passes it a flag indicating it should be shown.
            window = SDL_CreateWindow(
                windowName,
                SDL_WINDOWPOS_UNDEFINED,
                SDL_WINDOWPOS_UNDEFINED,
                windowWidth,
                windowHeight,
                SDL_WindowFlags.SDL_WINDOW_SHOWN);

            //makes window resizable
            SDL_SetWindowResizable(window, SDL_bool.SDL_TRUE);

            if (window == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue creating the window. {SDL_GetError()}");
            }

            // Creates a new SDL hardware renderer using the default graphics device with VSYNC enabled.
            renderer = SDL_CreateRenderer(
                window,
                -1,
                SDL_RendererFlags.SDL_RENDERER_ACCELERATED & SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE |
                SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue creating the renderer. {SDL_GetError()}");
            }

            UpdateWindowSize();

            //enityIsCreated
            MyCharacter testobject0 = new MyCharacter(0, new Vector2(300, 30), 10);
            MyCharacter testobject1 = new MyCharacter(1, new Vector2(100, 100), 40);
            MyCharacter testobject2 = new MyCharacter(2, new Vector2(200, 50), 5);
            MyCharacter testobject3 = new MyCharacter(3, new Vector2(250, 50));
            MyCharacter testobject4 = new MyCharacter(7, new Vector2(250, 150), 10);

            testobject1.GetComponent<RigidBody>().Velocity += new Vector2(-10f, 0);
            testobject0.GetComponent<RigidBody>().Velocity += new Vector2(10f, 0);
            testobject4.GetComponent<RigidBody>().Velocity += new Vector2(0, 0);

            MycharacterBox box = new MycharacterBox(4, new Vector2(0, windowHeight), windowWidth, 2);
            MycharacterBox box1 = new MycharacterBox(5, new Vector2(0, 0), 2, windowHeight);
            MycharacterBox box2 = new MycharacterBox(6, new Vector2(windowWidth, 0), 2, windowHeight);
            //All Components are initialized
            TransformSystem.Initialize();
            ScreenRectangleSystem.Initialize();
            ScreenCircleSystem.Initialize();

            RigidBodySystem.Initialize();
            CollisionSystem.Initialize();

            Console.ForegroundColor = ConsoleColor.DarkGreen;

            Console.WriteLine("Succesfully started " + windowName);
            Console.ResetColor();
        }


        /// <summary>
        /// Checks to see if there are any events to be processed.
        /// </summary>
        static void PollEvents()
        {
            // Check to see if there are any events and continue to do so until the queue is empty.
            while (SDL_PollEvent(out SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        running = false;
                        break;
                    case SDL_EventType.SDL_WINDOWEVENT:
                        HandleWindowEvents(e.window);
                        break;

                }

            }
        }

        static void HandleWindowEvents(SDL_WindowEvent wEvent)
        {
            switch (wEvent.windowEvent)
            {
                case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                    UpdateWindowSize();
                    break;

                    // more window events here ...
            }
        }

        static void UpdateWindowSize()
        {
            SDL_GetRendererOutputSize(renderer, out windowWidth, out windowHeight);

        }

        static void UpdatePhysics()
        {
            float dt = (float)deltaTime;
            CollisionSystem.Update(dt);
            RigidBodySystem.Update(dt);

        }
        /// <summary>
        /// Renders to the window.
        /// </summary>
        static void Render()
        {
            // Sets the color that the screen will be cleared with.
            SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);

            // Clears the current render surface.
            SDL_RenderClear(renderer);

            //updates components for new frame
            UpdateSystems();

            // Switches out the currently presented render surface with the one we just did work on.
            SDL_RenderPresent(renderer);
        }

        /// <summary>
        /// Clean up the resources that were created.
        /// </summary>
        static void CleanUp()
        {
            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);
            SDL_Quit();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nSuccesfully closed " + windowName);
        }




    }
}

