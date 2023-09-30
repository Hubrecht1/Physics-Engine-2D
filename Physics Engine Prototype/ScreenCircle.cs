using System;
using System.Numerics;
using static SDL2.SDL;

namespace Physics_Engine
{
    public class ScreenCircle : Component
    {
        int radius;

        SDL_Color color;
        IntPtr renderer;
        Transform entityTransform;

        public ScreenCircle(int _radius, SDL_Color _color, IntPtr _renderer)
        {
            ScreenCircleSystem.Register(this);
            renderer = _renderer;
            radius = _radius;
            color = _color;

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
            SDLRenderExtentions.DrawCircle(renderer, entityTransform.position, radius, color);

        }

    }

}