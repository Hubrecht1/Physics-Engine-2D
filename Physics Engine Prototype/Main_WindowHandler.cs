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
        public static int windowHeight = 1000;
        public static int windowWidth = 1200;

        static ulong NOW, LAST = 0;

        static double deltaTime = 0;
        static uint frameCount = 0;

        static double totalSecondsElapsed = 0;
        static double totalPhysicsSecondsElapsed = 0;

        static SDL_Color TextColor = new SDL_Color { r = 0, g = 0, b = 0, a = 255 };

        static public double inv_PhysicsTPS = 1.0d / 2048d; //  TPS
        static double accumulator = 0.0;

        static uint numberOfCircles = 0;

        //main loop
        static void Main(string[] args)
        {
            Setup();


            while (running)
            {
                // Update update

                LAST = NOW;
                NOW = SDL_GetPerformanceCounter();
                deltaTime = (double)(NOW - LAST) / SDL_GetPerformanceFrequency();

                if (LAST == 0)
                {
                    deltaTime = 0;
                }

                frameCount++;
                ulong start = SDL_GetPerformanceCounter();
                PollEvents();
                ulong physicsStart = SDL_GetPerformanceCounter();
                UpdatePhysics((float)deltaTime);
                ulong physicsEnd = SDL_GetPerformanceCounter();

                Render();

                ulong end = SDL_GetPerformanceCounter();
                double secondsElapsed = (end - start) / (double)SDL_GetPerformanceFrequency();
                double physicsTimeElapsed = (physicsEnd - physicsStart) / (double)SDL_GetPerformanceFrequency();

                totalSecondsElapsed += secondsElapsed;
                totalPhysicsSecondsElapsed += 1000 * physicsTimeElapsed;

                if (frameCount % 30 == 0)
                {
                    double averageFPS = Math.Round(30 / totalSecondsElapsed, 2);
                    double averagePhysicsTime = Math.Round(((totalPhysicsSecondsElapsed) / 30), 2);

                    if (averagePhysicsTime >= totalSecondsElapsed * 1000 / 60) //2*30=60
                    {
                        inv_PhysicsTPS *= 2;

                    }

                    string line = $"(average)fps {averageFPS}; dt: {Math.Round(deltaTime * 1000, 2)} ms; physics: {averagePhysicsTime} ms; TPS: {1 / (float)inv_PhysicsTPS} circles: {numberOfCircles}";

                    Console.Clear();
                    Console.Write(line);

                    totalSecondsElapsed = 0;
                    totalPhysicsSecondsElapsed = 0;
                }



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


            new RB_Box(0, new Vector2(0, windowHeight - 30), windowWidth, 8, 0, 0.5f); //bottom

            new RB_Box(0, new Vector2(0, -8), windowWidth, 8, 0, 0.5f); //top


            new RB_Box(0, new Vector2(0, windowHeight), windowWidth, 8, 0, 0.5f); //left

            new RB_Box(1, new Vector2(0, 0), 8, windowHeight, 0, 1); //right

            new RB_Box(3, new Vector2(windowWidth - 4, 0), 8, windowHeight, 0f);

            //random box
            new RB_Box(0, new Vector2(400, windowHeight - 200), 300, 8, 0, 0.5f);
            new RB_Box(0, new Vector2(450, windowHeight - 300), 8, 100, 0, 0.5f);
            new RB_Box(0, new Vector2(250, windowHeight - 300), 8, 100, 0, 0.5f);



            new RB_Box(4, new Vector2(200, 50), 60, 60, -1f, 1.0f);
            //boxes

            new RB_Box(0, new Vector2(200, 250), 200, 50, 0, 0.5f);
            new RB_Box(0, new Vector2(250, 490), 400, 20, 0, 0.5f);

            new RB_Box(0, new Vector2(400, 400), 50, 100, 0, 0.5f);
            new RB_Box(0, new Vector2(200, 400), 50, 100, 0, 0.5f);




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

        /// <summary>
        /// Checks to see if there are any events to be processed.
        /// </summary>
        static void PollEvents()
        {
            uint MouseState = SDL_GetMouseState(out int x, out int y);

            if (MouseState == SDL_BUTTON_X1)
            {
                new RB_Circle(5, new Vector2(x, y), new Random().Next(3, 14), -1, 0.7f, true).GetComponent<RigidBody>().Velocity = new Vector2(new Random().Next(-5, 5), new Random().Next(-5, 5));
                //new RB_Box(0, new Vector2(x, y), new Random().Next(6, 28), new Random().Next(6, 28), -1, 0.5f, true);
                numberOfCircles++;

            }

            // Check to see if there are any events and continue to do so until the queue is empty.
            while (SDL_PollEvent(out SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        running = false;
                        break;
                    case SDL_EventType.SDL_KEYDOWN:
                        if (e.key.keysym.sym == SDL_Keycode.SDLK_ESCAPE)
                        {
                            running = false;
                            break;
                        }
                        continue;
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

        static void UpdatePhysics(float dt) //dt = deltatime
        {
            accumulator += dt;

            //updates every frame
            while (accumulator >= inv_PhysicsTPS)
            {
                //tps = ticks per second
                float fixedTimeStepFloat = (float)inv_PhysicsTPS; // inv_PhysicsTPS = 1 / tps 

                CollisionSystem.Update(fixedTimeStepFloat);
                RigidBodySystem.Update(fixedTimeStepFloat);

                accumulator -= inv_PhysicsTPS;

            }

        }

        /// <summary>
        /// Renders to the window.
        /// </summary>
        static void Render()
        {
            // Sets the color that the screen will be cleared with.
            SDL_SetRenderDrawColor(renderer, 50, 60, 70, 255);

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

