using System;
using System.Numerics;

namespace Physics_Engine_Prototype
{
    public class RigidBody : Component
    {
        Transform entityTransform;
        Vector2 Velocity = Vector2.Zero;
        Vector2 Force = Vector2.Zero;
        Vector2 NewPosition = Vector2.Zero;
        float mass;

        public RigidBody(float _mass)
        {
            RigidBodySystem.Register(this);
            mass = _mass;

        }

        public override void Initialize()
        {
            entityTransform = entity.GetComponent<Transform>();
            ;
        }

        public override void Update(float dt)
        {
            Force += -PhysicsConstants.gravityAccelaration * mass;
            Velocity += Force / mass * dt;
            NewPosition += Velocity * dt * (1 / PhysicsConstants.pixelSizeInMeters);
            Force = Vector2.Zero;
            UpdatePosition();

        }

        void UpdatePosition()
        {
            NewPosition += entity.GetComponent<Transform>().position;
            entityTransform.position = NewPosition;
            NewPosition = Vector2.Zero;

        }
    }
}

