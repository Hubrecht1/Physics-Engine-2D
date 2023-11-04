using System;
using System.Numerics;

namespace Physics_Engine
{
    public class RigidBody : Component
    {
        public Transform entityTransform;
        public Vector2 Velocity = Vector2.Zero;
        public float inv_mass;
        public float mass;
        public float restitution;
        public float dragCoefficient = 0;
        public float crossSectionArea = 0;

        Vector2 Force = Vector2.Zero;
        Vector2 NewPosition = Vector2.Zero;

        public RigidBody(float _mass, float _restitution = 0.1f)
        {
            RigidBodySystem.Register(this);
            mass = _mass;

            if (_mass == 0)
            {
                inv_mass = 0;
            }
            else
            {
                inv_mass = 1 / _mass;

            }
            restitution = _restitution;

        }

        public override void Initialize()
        {
            entityTransform = entity.GetComponent<Transform>();

        }

        public override void Update(float dt)
        {
            Force += -PhysicsConstants.GravityAccelaration * mass;
            Force += -0.5f * PhysicsConstants.airDensity * Velocity * Vector2.Abs(Velocity) * crossSectionArea * dragCoefficient;


            Velocity += Force * inv_mass * dt;
            NewPosition += Velocity * dt * PhysicsConstants.inv_pixelSizeInMeters;

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

