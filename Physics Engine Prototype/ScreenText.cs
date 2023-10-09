using System;
using System.Numerics;
using static SDL2.SDL;
using static SDL2.SDL_ttf;
using static SDL2.SDL_mixer;
namespace Physics_Engine
{
    public class ScreenText : Component
    {
        SDL_Color color;
        Transform enityTransform;
        int width;
        int height;
        string text;
        nint surfaceText;
        nint texture;
        nint helvetica;
        IntPtr renderer;
        SDL_Rect Message_rect;

        public ScreenText(string _text, int _width, int _height, SDL_Color _color, IntPtr _renderer)
        {
            ScreenTextSystem.Register(this);
            renderer = _renderer;
            width = _width;
            height = _height;
            color = _color;
            helvetica = TTF_OpenFont("Fonts/Helvetica.ttf", 24);
            surfaceText = TTF_RenderText_Solid(helvetica, _text, color);
            texture = SDL_CreateTextureFromSurface(renderer, surfaceText);

            Message_rect.x = 0;
            Message_rect.y = 0;
            Message_rect.w = width;
            Message_rect.h = height;
        }

        public override void Update(float dt)
        {
            Message_rect.x = (int)enityTransform.position.X;
            Message_rect.y = (int)enityTransform.position.Y;


            // SDL_RenderCopy(renderer, texture, ref t, ref Message_rect);
        }

        public override void Initialize()
        {
            enityTransform = entity.GetComponent<Transform>();

        }


    }
}

