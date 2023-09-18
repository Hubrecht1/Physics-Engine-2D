using System;
using System.Numerics;
using SDL2;
using static SDL2.SDL;

namespace Physics_Engine_Prototype
{
    public class MyCharacter : Entity
    {
        public MyCharacter()
        {
            //example
            ID = 1;

            Transform transform = new Transform();
            transform.position = new Vector2(30, 30);
            AddComponent(transform);

            ScreenRectangle screenObject = new ScreenRectangle(30, 30, new SDL_Color { r = 90, g = 0, b = 0, a = 255 }, Main_Window.renderer);
            AddComponent(screenObject);

            RigidBody rigidBody = new RigidBody(1);
            AddComponent(rigidBody);






        }
    }
}

