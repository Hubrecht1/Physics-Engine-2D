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
            Transform transform = new Transform();
            transform.position = Vector2.Zero;
            AddComponent(transform);

            ScreenObject screenObject = new ScreenObject(new Vector2(30, 30), 30, 30, new SDL_Color { r = 90, g = 0, b = 0, a = 255 }, Main_Window.renderer);
            AddComponent(screenObject);




        }
    }
}

