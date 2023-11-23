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
        public SDL_Color color = new SDL_Color { r = 30, g = 200, b = 180, a = 255 };

        public RB_Circle(uint _ID, Vector2 position, int radius, float mass = -1.0f, float restitution = 0.1f, bool inRuntime = false)
        {
            ID = _ID;
            Transform transform = new Transform();
            transform.position = position;

            if (inRuntime)
            {
                int rndByte = new Random().Next(180, 200); //blue
                int rndByte2 = new Random().Next(30, 40); // red
                int rndByte3 = new Random().Next(180, 255); // green


                color = new SDL_Color { r = (byte)rndByte2, g = (byte)rndByte3, b = (byte)rndByte, a = 255 };
            }
            ScreenCircle screenCircle = new ScreenCircle(radius, color, Main_Window.renderer);

            if (mass == -1.0f)
            {
                mass = PhysicsConstants.inv_pixelSizeInMeters * MathF.PI * MathF.Pow(radius, 2);
            }



            RigidBody rigidBody = new RigidBody(mass, restitution);

            CircleCollider circleCollider = new CircleCollider(radius);

            AddComponent(transform);
            AddComponent(screenCircle);
            AddComponent(rigidBody);
            AddComponent(circleCollider);

            if (inRuntime)
            {
                transform.Initialize();
                screenCircle.Initialize();
                rigidBody.Initialize();
                circleCollider.Initialize();
            }

        }

    }

    public class RB_Box : Entity
    {
        public SDL_Color color = new SDL_Color { r = 185, g = 76, b = 76, a = 255 };

        public RB_Box(uint _ID, Vector2 position, int width, int height, float mass = -1.0f, float restitution = 0.1f, bool inRuntime = false)
        {
            ID = _ID;
            Transform transform = new Transform();
            transform.position = position;

            if (inRuntime)
            {
                Random rnd = new Random();
                Byte[] b = new Byte[1];
                rnd.NextBytes(b);
                color = new SDL_Color { r = b[0], g = 76, b = 76, a = 255 };
            }

            ScreenRectangle screenRectangle = new ScreenRectangle(width, height, color, Main_Window.renderer);

            if (mass == -1.0f)
            {
                mass = PhysicsConstants.inv_pixelSizeInMeters * width * height;
            }

            RigidBody rigidBody = new RigidBody(mass, restitution);

            BoxCollider boxCollider = new BoxCollider(width, height);


            AddComponent(transform);
            AddComponent(screenRectangle);
            AddComponent(rigidBody);
            AddComponent(boxCollider);

            if (inRuntime)
            {
                transform.Initialize();
                screenRectangle.Initialize();
                rigidBody.Initialize();
                boxCollider.Initialize();
            }

        }





    }






}

