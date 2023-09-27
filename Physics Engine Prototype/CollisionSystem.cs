using System;
using System.Numerics;

namespace Physics_Engine
{
    public class CollisionSystem
    {
        protected static List<CircleCollider> circleColliders = new List<CircleCollider>();
        protected static List<BoxCollider> boxColliders = new List<BoxCollider>();

        public static void Register(CircleCollider circleCollider)
        {
            circleColliders.Add(circleCollider);
        }
        public static void Register(BoxCollider boxCollider)
        {
            boxColliders.Add(boxCollider);
        }

        public static void Initialize()
        {
            foreach (CircleCollider component in circleColliders)
            {
                component.Initialize();
            }
            foreach (BoxCollider component in boxColliders)
            {
                component.Initialize();
            }
        }

        public static void Update(float dt)
        {

        }

        bool CirclevsCircle(CircleCollider a, CircleCollider b)
        {
            Vector2 aPos = a.rigidBody.entityTransform.position;
            Vector2 bPos = b.rigidBody.entityTransform.position;

            float r = a.radius + b.radius;
            r *= r;

            return r < MathF.Pow(aPos.X + bPos.X, 2) + MathF.Pow(aPos.Y + bPos.Y, 2);
        }

        bool BoxvsBox(BoxCollider a, BoxCollider b)
        {
            if (a.max.X < b.min.X || a.min.X > b.max.X) { return false; }
            if (a.max.Y < b.min.Y || a.min.Y > b.max.Y) { return false; }

            return true;
        }

        void ResolveCollision(RigidBody A, RigidBody B)
        {
            // Calculate relative velocity 
            Vector2 rv = B.Velocity - A.Velocity;
            Vector2 normal = Vector2.Normalize(rv);
            // Calculate relative velocity in terms of the normal direction 
            float velAlongNormal = Vector2.Dot(rv, normal);
            // Do not resolve if velocities are separating 
            if (velAlongNormal > 0)
                return;
            // Calculate restitution 
            float e = Math.Min(A.restitution, B.restitution);
            // Calculate impulse scalar 
            float j = -(1 + e) * velAlongNormal;
            j /= 1 / A.mass + 1 / B.mass;
            // Apply impulse 
            Vector2 impulse = j * normal;
            A.Velocity -= 1 / A.mass * impulse;
            B.Velocity += 1 / B.mass * impulse;
        }


    }
}

