using static SDL2.SDL;
using System;
using SDL2;


namespace Physics_Engine_Prototype
{
    public static class Main_Window
    {
        static IntPtr window;
        static IntPtr renderer;
        public static bool running = true;
        public static string windowName = "Physics_Engine_Prototype";
        public static int windowHeight;
        public static int windowWidth;

        //public static event EventHandler Draw;

        //main loop
        static void Main(string[] args)
        {
            Setup();

            while (running)
            {
                PollEvents();
                Render();
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


            // Set the color to red before drawing our shape
            SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);

            // Draw a line from top left to bottom right
            SDL.SDL_RenderDrawLine(renderer, 0, 0, windowWidth, windowHeight);

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
        }




    }
}

