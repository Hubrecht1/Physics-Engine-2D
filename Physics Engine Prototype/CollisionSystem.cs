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

            //checks every posible circlevscircle collision
            for (int i = 0; i < circleColliders.Count; i++)
            {
                for (int j = i + 1; j < circleColliders.Count; j++)
                {
                    float penetration = 0f;
                    Vector2 collisionNormal = Vector2.Zero;
                    if (CirclevsCircle(circleColliders[i], circleColliders[j], ref collisionNormal, ref penetration))
                    {

                        ResolveCollision(circleColliders[i].rigidBody, circleColliders[j].rigidBody, collisionNormal);
                        //PositionalCorrection(circleColliders[i].rigidBody, circleColliders[j].rigidBody, collisionNormal, penetration);
                    }
                }
            }
            //checks every posible boxvsbox collision
            for (int i = 0; i < boxColliders.Count; i++)
            {
                for (int j = i + 1; j < boxColliders.Count; j++)
                {
                    float penetration = 0f;
                    Vector2 collisionNormal = Vector2.Zero;
                    if (BoxvsBox(boxColliders[i], boxColliders[j], ref collisionNormal, ref penetration))
                    {
                        ResolveCollision(boxColliders[i].rigidBody, boxColliders[j].rigidBody, collisionNormal);
                        //PositionalCorrection(boxColliders[i].rigidBody, boxColliders[j].rigidBody, collisionNormal, penetration);
                    }
                }
            }
            //checks every posible circlevsbox collision
            for (int i = 0; i < circleColliders.Count; i++)
            {
                for (int j = 0; j < boxColliders.Count; j++)
                {
                    float penetration = 0f;
                    Vector2 collisionNormal = Vector2.Zero;
                    if (BoxvsCircle(circleColliders[i], boxColliders[j], ref collisionNormal, ref penetration))
                    {
                        ResolveCollision(circleColliders[i].rigidBody, boxColliders[j].rigidBody, collisionNormal);
                        //PositionalCorrection(circleColliders[i].rigidBody, boxColliders[j].rigidBody, collisionNormal, penetration);
                    }
                }
            }
        }


        static void ResolveCollision(RigidBody A, RigidBody B, Vector2 normal)
        {

            // Calculate relative velocity 
            Vector2 rv = (B.Velocity - A.Velocity);

            // Calculate relative velocity in terms of the normal direction 
            float velAlongNormal = Vector2.Dot(rv, normal);
            // Do not resolve if velocities are separating 
            if (velAlongNormal > 0)
                return;
            // Calculate restitution 
            float e = Math.Min(A.restitution, B.restitution);
            // Calculate impulse scalar 

            float impulseMagnitude = -1 * (1 + e) * velAlongNormal;


            float total_inv_Mass = A.inv_mass + B.inv_mass;

            Vector2 impulse = (impulseMagnitude / total_inv_Mass) * normal;
            // Apply impulse 

            A.Velocity -= A.inv_mass * impulse;

            B.Velocity += B.inv_mass * impulse;


        }


        static bool CirclevsCircle(CircleCollider a, CircleCollider b, ref Vector2 normal, ref float penetration)
        {
            Vector2 aPos = a.rigidBody.entityTransform.position;
            Vector2 bPos = b.rigidBody.entityTransform.position;

            Vector2 n = bPos - aPos;
            float r = a.radius + b.radius;
            r *= r;

            if (n.LengthSquared() > r)
            {

                return false;
            }

            //collision

            float d = n.Length();

            if (d != 0)
            {
                penetration = r - d;
                //calculates normal by dividing vector by its own length
                normal = n / d;
                return true;
            }
            else
            {
                penetration = a.radius;
                normal = new Vector2(1, 0);
                return true;
            }

        }


        static bool BoxvsBox(BoxCollider A, BoxCollider B, ref Vector2 normal, ref float penetration)
        {
            Vector2 aPos = A.rigidBody.entityTransform.position;
            Vector2 bPos = B.rigidBody.entityTransform.position;

            Vector2 a_Min = aPos;
            Vector2 b_Min = bPos;

            Vector2 a_Max = aPos + new Vector2(A.width, A.height);
            Vector2 b_Max = bPos + new Vector2(B.width, B.height);

            if (a_Max.X < b_Min.X || a_Min.X > b_Max.X) { return false; }
            if (a_Max.Y < b_Min.Y || a_Min.Y > b_Max.Y) { return false; }

            // Vector from A to B 
            Vector2 n = bPos - aPos;
            // Calculate half extents along x axis for each object 
            float a_extent = (a_Max.X - a_Min.X) / 2;
            float b_extent = (b_Max.X - b_Min.X) / 2;

            // Calculate overlap on x axis 
            float x_overlap = a_extent + b_extent - Math.Abs(n.X);

            // SAT test on x axis 
            if (x_overlap > 0)
            {
                // Calculate half extents along x axis for each object 
                a_extent = (a_Max.Y - a_Min.Y) / 2;
                b_extent = (b_Max.Y - b_Min.Y) / 2;

                // Calculate overlap on y axis 
                float y_overlap = a_extent + b_extent - Math.Abs(n.Y);

                // SAT test on y axis 
                if (y_overlap > 0)
                {
                    // Find out which axis is axis of least penetration 
                    if (x_overlap > y_overlap)
                    {
                        // Point towards B knowing that n points from A to B 
                        if (n.X < 0)
                            normal = new Vector2(-1, 0);
                        else
                            normal = new Vector2(0, 0);
                        penetration = x_overlap;
                        return true;
                    }
                    else
                    {
                        // Point toward B knowing that n points from A to B 
                        if (n.Y < 0)
                            normal = new Vector2(0, -1);
                        else
                            normal = new Vector2(0, 1);
                        penetration = y_overlap;
                        return true;
                    }
                }
                else { return false; }
            }
            else { return false; }
        }


        static bool BoxvsCircle(CircleCollider A, BoxCollider B, ref Vector2 normal, ref float penetration)
        {
            Vector2 circlePosition = A.rigidBody.entityTransform.position;
            Vector2 boxPosition = B.rigidBody.entityTransform.position;

            Vector2 boxCenter = boxPosition + new Vector2(B.width / 2, B.height / 2);

            Vector2 n = boxCenter - circlePosition;

            Vector2 closest = n;

            float xExtent = B.width / 2;
            float yExtent = B.height / 2;

            closest.X = Math.Clamp(closest.X, -xExtent, xExtent);
            closest.Y = Math.Clamp(closest.Y, -yExtent, yExtent);

            bool inside = false;

            if (n == closest)
            {
                inside = true;

                // Determine closest axis and clamp to closest extent
                if (Math.Abs(n.X) > Math.Abs(n.Y))
                {
                    // Clamp to closest extent 
                    if (closest.X > 0)
                        closest.X = xExtent;
                    else
                        closest.X = -xExtent;
                }
                else
                {
                    if (closest.Y > 0)
                        closest.Y = yExtent;
                    else
                        closest.Y = -yExtent;
                }
            }

            Vector2 collisionNormal = n - closest;
            float d = collisionNormal.LengthSquared();

            float r = A.radius;

            if (d > r * r && !inside)
                return false;

            d = MathF.Sqrt(d);

            if (inside)
            {
                normal = -collisionNormal;

                penetration = r - d;
            }
            else
            {
                normal = collisionNormal;

                penetration = r - d;
            }

            normal = Vector2.Normalize(normal);

            return true;
        }

        static void PositionalCorrection(RigidBody A, RigidBody B, Vector2 normal, float penetration)
        {
            const float percent = 0.1f; // usually 20% to 80%
            const float slop = 0.02f;   // usually 0.01 to 0.1

            Vector2 correction = (Math.Max(penetration - slop, 0.0f) / (A.inv_mass + B.inv_mass)) * percent * normal;

            A.entityTransform.position -= A.inv_mass * correction;
            B.entityTransform.position += B.inv_mass * correction;
        }


    }






}

