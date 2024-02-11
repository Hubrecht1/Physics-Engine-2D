using System;
using static SDL2.SDL;

using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Physics_Engine
{
    public static class SDLRenderExtentions
    {
        private static int numThreads;

        public static void DrawCircle(IntPtr renderer, Vector2 position, int radius, SDL_Color color)
        {
            int new_x = 0;
            int new_y = 0;
            int old_x = (int)position.X + radius;
            int old_y = (int)position.Y;
            uint numberOfLines = 30;
            float step = (MathF.PI * 2) / numberOfLines;

            SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);

            List<(int, int, int, int)> coordinates = new List<(int, int, int, int)>();


            //new method:
            //for (float theta = 0; theta <= (MathF.PI * 2); theta += step)
            //{

            //    new_x = (int)(position.X + (radius * MathF.Cos(theta)));
            //    new_y = (int)(position.Y - (radius * MathF.Sin(theta)));
            //    coordinates.Add((old_x, old_y, new_x, new_y));

            //    old_x = new_x;
            //    old_y = new_y;
            //}
            //Parallel.ForEach(coordinates, item => { SDL_RenderDrawLine(renderer, item.Item1, item.Item2, item.Item3, item.Item4); });
            //coordinates.Clear();

            for (float theta = 0; theta <= (MathF.PI * 2); theta += step)
            {
                new_x = (int)(position.X + (radius * MathF.Cos(theta)));
                new_y = (int)(position.Y - (radius * MathF.Sin(theta)));

                SDL_RenderDrawLine(renderer, old_x, old_y, new_x, new_y);

                old_x = new_x;
                old_y = new_y;
            }



        }

    }
}

