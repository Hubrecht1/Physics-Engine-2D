using System;
using System.Numerics;

namespace Physics_Engine
{
    public class RigidBody : Component
    {
        public Transform entityTransform;
        public Vector2 Velocity = Vector2.Zero;
        public float mass;
        public float restitution;
        bool isStatic = false;

        Vector2 Force = Vector2.Zero;
        Vector2 NewPosition = Vector2.Zero;

        public RigidBody(float _mass, float _restitution = 0.1f)
        {
            RigidBodySystem.Register(this);
            mass = _mass;
            restitution = _restitution;

        }

        public override void Initialize()
        {
            entityTransform = entity.GetComponent<Transform>();

        }

        public override void Update(float dt)
        {
            Force += -PhysicsConstants.gravityAccelaration * mass;
            Velocity += Force / mass * dt;
            NewPosition += Velocity * dt * PhysicsConstants.pixelSizeInMeters;
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

