using System;
using System.Numerics;
using SDL2;
using static SDL2.SDL;

namespace Physics_Engine
{
    public static class EntityCreator
    {
        static uint ID = 0;



    }

    public class RB_Circle : Entity
    {
        public SDL_Color color = new SDL_Color { r = 0, g = 0, b = 0, a = 255 };

        public RB_Circle(uint _ID, Vector2 position, int radius, float mass = -1.0f, float restitution = 0.1f)
        {
            ID = _ID;
            Transform transform = new Transform();
            transform.position = position;
            AddComponent(transform);

            ScreenCircle screenCircle = new ScreenCircle(radius, color, Main_Window.renderer);
            AddComponent(screenCircle);

            if (mass == -1.0f)
            {
                mass = PhysicsConstants.inv_pixelSizeInMeters * radius;
            }

            RigidBody rigidBody = new RigidBody(mass, restitution);
            AddComponent(rigidBody);

            CircleCollider circleCollider = new CircleCollider(radius);
            AddComponent(circleCollider);

        }

    }

    public class RB_Box : Entity
    {
        public SDL_Color color = new SDL_Color { r = 0, g = 0, b = 0, a = 255 };

        public RB_Box(uint _ID, Vector2 position, int width, int height, float mass = -1.0f, float restitution = 0.1f)
        {
            ID = _ID;
            Transform transform = new Transform();
            transform.position = position;
            AddComponent(transform);

            ScreenRectangle screenRectangle = new ScreenRectangle(width, height, color, Main_Window.renderer);
            AddComponent(screenRectangle);

            if (mass == -1.0f)
            {
                mass = PhysicsConstants.inv_pixelSizeInMeters * MathF.Sqrt((int)(Math.Pow(width, 2) + Math.Pow(height, 2)));
            }

            RigidBody rigidBody = new RigidBody(mass, restitution);
            AddComponent(rigidBody);

            BoxCollider boxCollider = new BoxCollider(width, height);
            AddComponent(boxCollider);

        }

    }




}

