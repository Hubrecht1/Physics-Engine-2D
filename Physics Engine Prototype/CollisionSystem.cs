﻿using System;
using System.Numerics;
using static SDL2.SDL;

namespace Physics_Engine
{
    public class CollisionSystem
    {
        protected static List<CircleCollider> circleColliders = new List<CircleCollider>();
        protected static List<BoxCollider> boxColliders = new List<BoxCollider>();

        protected static List<CircleCollider> big_CircleColliders = new List<CircleCollider>();
        protected static List<BoxCollider> big_BoxColliders = new List<BoxCollider>();

        static List<Manifold> contactList;

        static CollisionGrid collisionGrid;

        static int cellSize = 200;

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
            FindAndSolveCollisions(circleColliders, boxColliders);

            //collisionGrid = MakeGrid();
            //AddCollidersToGrid(collisionGrid);
            // SolveGrid(collisionGrid);
            //collisionGrid.collisionCells.Clear();

            //double secondsElapsed = (solvingend - solving) / (double)SDL_GetPerformanceFrequency();
            //Console.WriteLine(secondsElapsed * 1000);
        }



        static void FindAndSolveCollisions(List<CircleCollider> _circleColliders, List<BoxCollider> _boxColliders)
        {
            //checks every posible circlevscircle collision
            for (int i = 0; i < _circleColliders.Count; i++)
            {
                for (int j = i + 1; j < _circleColliders.Count; j++)
                {
                    float penetration = 0f;
                    Vector2 collisionNormal = Vector2.Zero;
                    if (CirclevsCircle(_circleColliders[i], _circleColliders[j], ref collisionNormal, ref penetration))
                    {
                        ResolveCollision(_circleColliders[i].rigidBody, _circleColliders[j].rigidBody, collisionNormal);
                        PositionalCorrection(_circleColliders[i].rigidBody, _circleColliders[j].rigidBody, collisionNormal, penetration);
                    }
                }
            }
            //checks every posible boxvsbox collision
            for (int i = 0; i < _boxColliders.Count; i++)
            {
                for (int j = i + 1; j < _boxColliders.Count; j++)
                {
                    float penetration = 0f;
                    Vector2 collisionNormal = Vector2.Zero;
                    if (BoxvsBox(_boxColliders[i], _boxColliders[j], ref collisionNormal, ref penetration))
                    {
                        ResolveCollision(_boxColliders[i].rigidBody, _boxColliders[j].rigidBody, collisionNormal);
                        PositionalCorrection(_boxColliders[i].rigidBody, _boxColliders[j].rigidBody, collisionNormal, penetration);
                    }
                }
            }
            //checks every posible circlevsbox collision
            for (int i = 0; i < _circleColliders.Count; i++)
            {


                for (int j = 0; j < _boxColliders.Count; j++)
                {
                    float penetration = 0f;
                    Vector2 collisionNormal = Vector2.Zero;
                    if (BoxvsCircle(_circleColliders[i], _boxColliders[j], ref collisionNormal, ref penetration))
                    {
                        ResolveCollision(_circleColliders[i].rigidBody, _boxColliders[j].rigidBody, collisionNormal);
                        PositionalCorrection(_circleColliders[i].rigidBody, _boxColliders[j].rigidBody, collisionNormal, penetration);
                    }



                }
            }


        }

        static void FindAndSolveCollisionsFromCollider(BoxCollider collider, List<CircleCollider> _circleColliders, List<BoxCollider> _boxColliders)
        {
            //boxvsbox
            for (int i = 0; i < _boxColliders.Count; i++)
            {
                float penetration = 0f;
                Vector2 collisionNormal = Vector2.Zero;
                if (BoxvsBox(collider, _boxColliders[i], ref collisionNormal, ref penetration))
                {
                    ResolveCollision(collider.rigidBody, _boxColliders[i].rigidBody, collisionNormal);
                    PositionalCorrection(collider.rigidBody, _boxColliders[i].rigidBody, collisionNormal, penetration);
                }
            }

            //boxvscircle
            for (int i = 0; i < _circleColliders.Count; i++)
            {
                float penetration = 0f;
                Vector2 collisionNormal = Vector2.Zero;
                if (BoxvsCircle(_circleColliders[i], collider, ref collisionNormal, ref penetration))
                {
                    ResolveCollision(_circleColliders[i].rigidBody, collider.rigidBody, collisionNormal);
                    PositionalCorrection(_circleColliders[i].rigidBody, collider.rigidBody, collisionNormal, penetration);
                }
            }

        }

        static void FindAndSolveCollisionsFromCollider(CircleCollider collider, List<CircleCollider> _circleColliders, List<BoxCollider> _boxColliders)
        {
            //circlevscircle
            for (int i = 0; i < _circleColliders.Count; i++)
            {
                float penetration = 0f;
                Vector2 collisionNormal = Vector2.Zero;
                if (CirclevsCircle(collider, _circleColliders[i], ref collisionNormal, ref penetration))
                {
                    ResolveCollision(collider.rigidBody, _circleColliders[i].rigidBody, collisionNormal);
                    PositionalCorrection(collider.rigidBody, _circleColliders[i].rigidBody, collisionNormal, penetration);
                }
            }

            //boxvscircle
            for (int i = 0; i < _boxColliders.Count; i++)
            {
                float penetration = 0f;
                Vector2 collisionNormal = Vector2.Zero;
                if (BoxvsCircle(collider, _boxColliders[i], ref collisionNormal, ref penetration))
                {
                    ResolveCollision(collider.rigidBody, _boxColliders[i].rigidBody, collisionNormal);
                    PositionalCorrection(collider.rigidBody, _boxColliders[i].rigidBody, collisionNormal, penetration);
                }
            }

        }

        static void ResolveCollision(RigidBody A, RigidBody B, Vector2 normal)
        {
            if (A.mass == 0 && B.mass == 0)
            {
                return;
            }

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

            if (n.LengthSquared() > r * r)
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

            Vector2 aCenter = aPos + (0.5f * new Vector2(A.width, A.height));
            Vector2 bCenter = bPos + (0.5f * new Vector2(B.width, B.height));

            // Calculate the vector from A's center to B's center
            Vector2 n = bCenter - aCenter;

            // Calculate half extents along x axis for each object
            float a_extent = (a_Max.X - a_Min.X) / 2;
            float b_extent = (b_Max.X - b_Min.X) / 2;

            // Calculate overlap on x axis
            float x_overlap = a_extent + b_extent - Math.Abs(n.X);

            // SAT test on x axis
            if (x_overlap > 0)
            {
                // Calculate half extents along y axis for each object
                a_extent = (a_Max.Y - a_Min.Y) / 2;
                b_extent = (b_Max.Y - b_Min.Y) / 2;

                // Calculate overlap on y axis
                float y_overlap = a_extent + b_extent - Math.Abs(n.Y);

                // SAT test on y axis
                if (y_overlap > 0)
                {
                    // Find out which axis is axis of least penetration
                    if (x_overlap < y_overlap)
                    {
                        // Point towards B knowing that n points from A to B
                        if (n.X < 0)
                            normal = new Vector2(-1, 0);
                        else
                            normal = new Vector2(1, 0);
                        penetration = x_overlap;
                    }
                    else
                    {
                        // Point toward B knowing that n points from A to B
                        if (n.Y < 0)
                            normal = new Vector2(0, -1);
                        else
                            normal = new Vector2(0, 1);
                        penetration = y_overlap;
                    }

                    return true;
                }
            }

            return false;
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
            if (A.mass == 0 && B.mass == 0)
            {
                return;
            }

            const float percent = 0.1f; // usually 20% to 80%
            const float slop = 0.02f;   // usually 0.01 to 0.1

            Vector2 correction = (Math.Max(penetration - slop, 0.0f) / (A.inv_mass + B.inv_mass)) * percent * normal;

            A.entityTransform.position -= A.inv_mass * correction;
            B.entityTransform.position += B.inv_mass * correction;
        }

        static CollisionGrid MakeGrid()
        {
            float xMin = boxColliders.Min(x => x.rigidBody.entityTransform.position.X);
            float yMin = boxColliders.Min(x => x.rigidBody.entityTransform.position.Y);
            float xMax = boxColliders.Max(x => x.rigidBody.entityTransform.position.X);
            float yMax = boxColliders.Max(x => x.rigidBody.entityTransform.position.Y);

            Vector2 topLeft = new Vector2(xMin, yMin);
            Vector2 bottomRight = new Vector2(xMax, yMax);

            return new CollisionGrid(topLeft, bottomRight, cellSize);

        }


        static void AddCollidersToGrid(CollisionGrid _collisionGrid)
        {

            for (int i = 0; i < boxColliders.Count(); i++)
            {
                if (ColliderFitsIncell(boxColliders[i]))
                {
                    _collisionGrid.AddColliderToCell(boxColliders[i]);
                }
                else
                {
                    big_BoxColliders.Add(boxColliders[i]);

                }

            }

            for (int i = 0; i < circleColliders.Count(); i++)
            {
                if (ColliderFitsIncell(circleColliders[i]))
                {
                    _collisionGrid.AddColliderToCell(circleColliders[i]);

                }
                else
                {
                    big_CircleColliders.Add(circleColliders[i]);

                }
            }

        }

        static void SolveGrid(CollisionGrid _collisionGrid)
        {
            //ulong start = SDL_GetPerformanceCounter();
            for (int i = 0; i < big_BoxColliders.Count(); i++)
            {
                FindAndSolveCollisionsFromCollider(big_BoxColliders[i], circleColliders, boxColliders);
            }

            for (int i = 0; i < big_CircleColliders.Count(); i++)
            {
                FindAndSolveCollisionsFromCollider(big_CircleColliders[i], circleColliders, boxColliders);

            }
            //ulong end = SDL_GetPerformanceCounter();

            //double secondsElapsed = (end - start) / (double)SDL_GetPerformanceFrequency();
            //Console.WriteLine("bigcolldir :" + secondsElapsed * 1000);

            for (int x = 0; x < _collisionGrid.collisionCells.Count() - 1; x++)
            {
                for (int y = 0; y < _collisionGrid.collisionCells[0].Count() - 1; y++)
                {
                    if (_collisionGrid.collisionCells[x][y].boxColliders.Count == 0 && _collisionGrid.collisionCells[x][y].circleColliders.Count == 0)
                    {
                        continue;
                    }

                    CollisionCell currentCell = _collisionGrid.collisionCells[x][y];

                    int _dx = -1;
                    int _dy = -1;
                    if (x == 0)
                    { _dx = 0; }
                    if (y == 0)
                    { _dy = 0; }


                    for (int dx = _dx; dx <= 1; dx++)
                    {
                        for (int dy = _dy; dy <= 1; dy++)
                        {

                            CollisionCell otherCell = _collisionGrid.collisionCells[x + dx][y + dy];
                            SolveCellCollisions(currentCell, otherCell);

                        }

                    }
                    _collisionGrid.collisionCells[x][y].boxColliders.Clear();
                    _collisionGrid.collisionCells[x][y].circleColliders.Clear();
                }




            }
        }

        static void SolveCellCollisions(CollisionCell cell1, CollisionCell cell2)
        {
            //checks every  circlevscircle collision
            for (int i = 0; i < cell1.circleColliders.Count; i++)
            {
                for (int j = 0; j < cell2.circleColliders.Count; j++)
                {
                    if (cell1.circleColliders[i].GetHashCode() == cell2.circleColliders[j].GetHashCode())
                    {
                        continue;
                    }
                    float penetration = 0f;
                    Vector2 collisionNormal = Vector2.Zero;
                    if (CirclevsCircle(cell1.circleColliders[i], cell2.circleColliders[j], ref collisionNormal, ref penetration))
                    {
                        ResolveCollision(cell1.circleColliders[i].rigidBody, cell2.circleColliders[j].rigidBody, collisionNormal);
                        PositionalCorrection(cell1.circleColliders[i].rigidBody, cell2.circleColliders[j].rigidBody, collisionNormal, penetration);
                    }


                }
            }
            //checks every  boxvsbox collision
            for (int i = 0; i < cell1.boxColliders.Count; i++)
            {
                for (int j = 0; j < cell2.boxColliders.Count; j++)
                {
                    if (!cell1.boxColliders[i].Equals(cell2.boxColliders[j]))
                    {
                        {
                            float penetration = 0f;
                            Vector2 collisionNormal = Vector2.Zero;
                            if (BoxvsBox(cell1.boxColliders[i], cell2.boxColliders[j], ref collisionNormal, ref penetration))
                            {
                                ResolveCollision(cell1.boxColliders[i].rigidBody, cell2.boxColliders[j].rigidBody, collisionNormal);
                                PositionalCorrection(cell1.boxColliders[i].rigidBody, cell2.boxColliders[j].rigidBody, collisionNormal, penetration);
                            }
                        }
                    }
                }

            }

            //checks every  circlevsbox collision
            for (int i = 0; i < cell1.circleColliders.Count; i++)
            {
                for (int j = 0; j < cell2.boxColliders.Count; j++)
                {
                    float penetration = 0f;
                    Vector2 collisionNormal = Vector2.Zero;
                    if (BoxvsCircle(cell1.circleColliders[i], cell2.boxColliders[j], ref collisionNormal, ref penetration))
                    {
                        ResolveCollision(cell1.circleColliders[i].rigidBody, cell2.boxColliders[j].rigidBody, collisionNormal);
                        PositionalCorrection(cell1.circleColliders[i].rigidBody, cell2.boxColliders[j].rigidBody, collisionNormal, penetration);
                    }



                }
            }

            if (cell1.Equals(cell2))
            {
                return;
            }


            //checks every  boxvscircle collision
            for (int i = 0; i < cell2.circleColliders.Count; i++)
            {
                for (int j = 0; j < cell1.boxColliders.Count; j++)
                {

                    float penetration = 0f;
                    Vector2 collisionNormal = Vector2.Zero;
                    if (BoxvsCircle(cell2.circleColliders[i], cell1.boxColliders[j], ref collisionNormal, ref penetration))
                    {
                        ResolveCollision(cell2.circleColliders[i].rigidBody, cell1.boxColliders[j].rigidBody, collisionNormal);
                        PositionalCorrection(cell2.circleColliders[i].rigidBody, cell1.boxColliders[j].rigidBody, collisionNormal, penetration);
                    }



                }
            }

        }

        static bool ColliderFitsIncell(BoxCollider _boxCollider)
        {
            if (_boxCollider.width >= collisionGrid.cellSize || _boxCollider.height >= collisionGrid.cellSize)
            {
                return false;
            }
            return true;

        }

        static bool ColliderFitsIncell(CircleCollider _CircleCollider)
        {
            if (2 * _CircleCollider.radius >= collisionGrid.cellSize || 2 * _CircleCollider.radius >= collisionGrid.cellSize)
            {
                return false;
            }
            return true;

        }














    }
}

