using System;
using System.Numerics;

namespace Physics_Engine
{
    public class CircleCollider : Component
    {
        public float radius;
        public RigidBody rigidBody;

        public CircleCollider(float _radius)
        {
            CollisionSystem.Register(this);

        }

        public override void Initialize()
        {
            rigidBody = entity.GetComponent<RigidBody>();

        }






    }
}

