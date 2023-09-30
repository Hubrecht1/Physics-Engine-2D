using System;
using System.Drawing;
using static SDL2.SDL;
using System.Numerics;

namespace Physics_Engine
{
    public class ScreenRectangle : Component
    {
        int width;
        int height;
        SDL_Color color;
        SDL_Rect rect;
        IntPtr renderer;
        Transform entityTransform;

        public ScreenRectangle(int _width, int _height, SDL_Color _color, IntPtr _renderer)
        {
            ScreenRectangleSystem.Register(this);
            renderer = _renderer;
            width = _width;
            height = _height;
            color = _color;

            rect = new SDL_Rect { x = 0, y = 0, w = width, h = height };

        }

        public override void Initialize()
        {
            if (entity.GetComponent<Transform>() == null)
            {
                Console.Error.WriteLine($"Entity {entity.ID} must contain TransformComponent for screenobjectComponent to work");

            }
            entityTransform = entity.GetComponent<Transform>();

        }

        public override void Update(float dt)
        {
            SDL_SetRenderDrawColor(renderer, color.r, color.g, color.b, color.a);
            SDL_RenderFillRect(renderer, ref rect);

            //updates position
            rect.x = (int)entityTransform.position.X;
            rect.y = (int)entityTransform.position.Y;
        }






    }
}

