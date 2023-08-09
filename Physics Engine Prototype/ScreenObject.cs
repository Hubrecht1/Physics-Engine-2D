﻿using System;
using System.Drawing;
using static SDL2.SDL;
using System.Numerics;

namespace Physics_Engine_Prototype
{
    public struct ScreenObject
    {
        int width;
        int height;
        Vector2 screenPosition;
        SDL_Color color;
        SDL_Rect rect;

        public ScreenObject(Vector2 _screenPosition, int _width, int _height, SDL_Color _color)
        {
            width = _width;
            height = _height;
            color = _color;
            screenPosition = _screenPosition;
            rect = new SDL_Rect { x = (int)screenPosition.X, y = (int)screenPosition.Y, w = width, h = height };

        }

        public void Draw(IntPtr renderer)
        {
            SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
            SDL_RenderFillRect(renderer, ref rect);


        }






    }
}

