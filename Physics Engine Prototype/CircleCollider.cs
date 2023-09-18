using System;
using System.Numerics;

namespace Physics_Engine_Prototype
{
    public class CircleCollider : Component
    {
        Vector2 Center;
        float radius;
        RigidBody RigidBody;

        public CircleCollider(float _radius)
        {


        }

        public override void Initialize()
        {
            RigidBody = entity.GetComponent<RigidBody>();

        }




    }
}

