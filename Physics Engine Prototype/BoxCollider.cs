using System;
using System.Numerics;

namespace Physics_Engine
{
    public class BoxCollider : Component
    {
        public Vector2 min;
        public Vector2 max;
        float rotation;
        RigidBody rigidBody;

        public BoxCollider(Vector2 _min, Vector2 _max, float _rotation)
        {
            CollisionSystem.Register(this);

        }

        public override void Initialize()
        {
            rigidBody = entity.GetComponent<RigidBody>();

        }
    }
}

