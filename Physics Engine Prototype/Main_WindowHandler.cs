using static SDL2.SDL;
using System;
using System.Numerics;
using SDL2;


namespace Physics_Engine_Prototype
{
    public static class Main_Window
    {
        static IntPtr window;
        public static IntPtr renderer;
        public static bool running = true;
        public static string windowName = "Physics_Engine_Prototype";
        public static int windowHeight;
        public static int windowWidth;

        public static event Action? DrawShapes;

        //public static event EventHandler Draw;

        //main loop
        static void Main(string[] args)
        {
            Setup();

            while (running)
            {
#if DEBUG
                UInt64 start = SDL_GetPerformanceCounter();

                PollEvents();
                Render();

                UInt64 end = SDL_GetPerformanceCounter();
                float secondsElapsed = (end - start) / (float)SDL_GetPerformanceFrequency();
                Console.WriteLine(1 / secondsElapsed + " fps");

#else

                PollEvents();
                Render();

#endif
            }

            CleanUp();
        }

        /// <summary>
        /// Setup all of the SDL resources we'll need to display a window.
        /// </summary>
        static void Setup()
        {
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
                640,
                480,
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
                SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue creating the renderer. {SDL_GetError()}");
            }

            UpdateWindowSize();

            Main_Renderer.Initialize();

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
            SDL.SDL_GetRendererOutputSize(renderer, out windowWidth, out windowHeight);

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

            //drawshapes event if not null
            DrawShapes?.Invoke();

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
            Console.WriteLine("Succesfully closed " + windowName);
        }




    }
}

