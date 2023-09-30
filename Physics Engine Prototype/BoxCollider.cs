using System;
using System.Numerics;

namespace Physics_Engine
{
    public class BoxCollider : Component
    {
        public int width;
        public int height;
        float rotation;
        public RigidBody rigidBody;

        public BoxCollider(int _width, int _height, float _rotation = 0)
        {
            CollisionSystem.Register(this);
            width = _width;
            height = _height;

        }

        public override void Initialize()
        {
            rigidBody = entity.GetComponent<RigidBody>();

        }
    }
}

