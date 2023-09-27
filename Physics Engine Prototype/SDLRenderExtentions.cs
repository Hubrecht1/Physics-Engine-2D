using System;
using static SDL2.SDL;

using System.Numerics;
namespace Physics_Engine
{
    public static class SDLRenderExtentions
    {
        public static void DrawCircle(IntPtr renderer, Vector2 position, int radius, SDL_Color color)
        {
            int new_x = 0;
            int new_y = 0;
            int old_x = (int)position.X + radius;
            int old_y = (int)position.Y;
            float step = (MathF.PI * 2) / 16;

            SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);

            for (float theta = 0; theta <= (MathF.PI * 2); theta += step)
            {
                new_x = (int)(position.X + (radius * MathF.Cos(theta)));
                new_y = (int)(position.Y - (radius * MathF.Sin(theta)));

                SDL_RenderDrawLine(renderer, old_x, old_y, new_x, new_y);

                old_x = new_x;
                old_y = new_y;
            }

            new_x = (int)(position.X + (radius * MathF.Cos(0)));
            new_y = (int)(position.Y - (radius * MathF.Sin(0)));
            SDL_RenderDrawLine(renderer, old_x, old_y, new_x, new_y);

        }

    }
}

