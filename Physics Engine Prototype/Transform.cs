using System;
using System.Numerics;

namespace Physics_Engine
{
    public class Transform : Component
    {
        public Vector2 position = Vector2.Zero;
        public Vector2 scale = Vector2.Zero;
        public float rotation = 0;

        public Transform()
        {
            TransformSystem.Register(this);
        }
    }


}

