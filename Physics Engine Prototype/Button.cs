using System;
using System.Drawing;
using static SDL2.SDL;

namespace Physics_Engine_Prototype
{
    public class Button
    {


        int width;
        int height;
        Color color;
        SDL_Rect srect, drect;
        bool isSelected = false;

        void Update()
        {






        }
        void Draw()
        {


        }
        Button(int _width, int _height, Color _color)
        {
            width = _width;
            height = _height;
            color = _color;


        }



    }
}

