using static SDL2.SDL;
using System;
using System.Numerics;
using SDL2;

namespace Physics_Engine_Prototype
{
    public static class Main_Renderer
    {

        static ScreenObject simpleRectangle;
        static IntPtr renderer;
        static SDL_Color color = new SDL_Color { r = 30, g = 50, b = 90, a = 100 };



        public static void Initialize()
        {
            Main_Window.DrawShapes += Draw;
            renderer = Main_Window.renderer;
            //new screenobj
            simpleRectangle = new ScreenObject(new Vector2(30, 30), 30, 30, color);



        }


        static void Draw()
        {
            simpleRectangle.Draw(renderer);

            // Set the color to red before drawing our shape
            SDL.SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);

            // Draw a line from top left to bottom right
            SDL.SDL_RenderDrawLine(renderer, 0, 0, Main_Window.windowWidth, Main_Window.windowHeight);



        }



    }
}

