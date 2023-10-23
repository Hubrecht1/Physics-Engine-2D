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
        static SDL_Color TextColor = new SDL_Color { r = 0, g = 0, b = 0, a = 255 };

        static double FixedTimeStep = 1.0d / 1000d; //  TPS
        static double accumulator = 0.0;

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
                UpdatePhysics((float)deltaTime);
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

            Console.ForegroundColor = ConsoleColor.DarkGreen;

            Console.WriteLine("Succesfully started " + windowName);
            Console.ResetColor();

            UpdateWindowSize();

            CreateEntities();
            InitializeSystems();


        }

        static void CreateEntities()
        {
            List<RB_Box> rB_StaticBoxes = new List<RB_Box>();
            List<RB_Box> rB_DynamicBoxes = new List<RB_Box>();
            List<RB_Circle> rB_DynamicCircles = new List<RB_Circle>();

            rB_StaticBoxes.Add(new RB_Box(0, new Vector2(0, windowHeight - 30), windowWidth, 4, 0, 0.5f));
            rB_StaticBoxes.Add(new RB_Box(1, new Vector2(0, 0), 4, windowHeight, 0, 1));
            rB_StaticBoxes.Add(new RB_Box(2, new Vector2(windowWidth, 0), 4, windowHeight, 0f));

            rB_DynamicBoxes.Add(new RB_Box(3, new Vector2(200, 50), 40, 40));

            //left ball
            rB_DynamicCircles.Add(new RB_Circle(4, new Vector2(40, 40), 20, -1, 0.7f));

            //right ball
            RB_Circle test = new RB_Circle(5, new Vector2(200, 40), 15, -1, 0.7f);

            rB_DynamicCircles.Add(test);
            test.GetComponent<RigidBody>().Velocity = new Vector2(-4, 0);

        }

        static void InitializeSystems()
        {
            //All Components are initialized
            TransformSystem.Initialize();
            ScreenRectangleSystem.Initialize();
            ScreenCircleSystem.Initialize();
            RigidBodySystem.Initialize();
            CollisionSystem.Initialize();

        }

        static void OnKeyDown(SDL_Keysym keySym)
        {
            if (keySym.sym == SDL_Keycode.SDLK_RETURN)
            {
                //UpdatePhysics(0.1f);
            }

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
                    case SDL_EventType.SDL_KEYDOWN:
                        OnKeyDown(e.key.keysym);
                        continue;

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

        static void UpdatePhysics(float dt)
        {
            accumulator += dt;

            while (accumulator >= FixedTimeStep)
            {
                float fixedTimeStepFloat = (float)FixedTimeStep;

                // Update your physics here with a fixed time step.
                CollisionSystem.Update(fixedTimeStepFloat);
                RigidBodySystem.Update(fixedTimeStepFloat);

                accumulator -= FixedTimeStep;


            }


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

