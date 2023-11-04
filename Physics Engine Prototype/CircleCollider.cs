using System;
using System.Numerics;

namespace Physics_Engine
{
    public class CircleCollider : Component
    {
        public int radius;
        public RigidBody rigidBody;

        public CircleCollider(int _radius)
        {
            radius = _radius;
            CollisionSystem.Register(this);

        }

        public override void Initialize()
        {
            rigidBody = entity.GetComponent<RigidBody>();
            rigidBody.crossSectionArea = (2 * (radius / PhysicsConstants.inv_pixelSizeInMeters));
            rigidBody.dragCoefficient = 0.47f;
        }






    }
}

