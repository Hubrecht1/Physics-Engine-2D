using System;
using System.Numerics;
using SDL2;
using static SDL2.SDL;

namespace Physics_Engine
{
    public class MyCharacter : Entity
    {
        public MyCharacter(int _ID, Vector2 position, int radius = 20)
        {
            //example
            ID = _ID;

            Transform transform = new Transform();
            transform.position = position;
            AddComponent(transform);

            ScreenCircle screenCircle = new ScreenCircle(radius, new SDL_Color { r = 90, g = 0, b = 0, a = 255 }, Main_Window.renderer);
            AddComponent(screenCircle);

            RigidBody rigidBody = new RigidBody(4f, 0.7f);
            AddComponent(rigidBody);

            CircleCollider circleCollider = new CircleCollider(radius);
            AddComponent(circleCollider);




        }
    }

    public class MycharacterBox : Entity
    {
        public MycharacterBox(int _ID, Vector2 position, int width, int height)
        {
            ID = _ID;

            Transform transform = new Transform();
            transform.position = position;
            AddComponent(transform);

            ScreenRectangle screenObject = new ScreenRectangle(width, height, new SDL_Color { r = 90, g = 0, b = 0, a = 255 }, Main_Window.renderer);
            AddComponent(screenObject);

            RigidBody rigidBody = new RigidBody(0, 0.5f);
            AddComponent(rigidBody);

            BoxCollider boxCollider = new BoxCollider(width, height);
            AddComponent(boxCollider);

        }


    }


}

