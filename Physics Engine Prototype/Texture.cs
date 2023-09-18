using System;
using SDL2;
using System.Numerics;

namespace Physics_Engine_Prototype
{

    public class Texture : Component
    {

        public Texture()
        {
            TextureSystem.Register(this);
        }
    }
}

