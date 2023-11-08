using System;
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

        static int cellSize = 30;

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
            //double secondsElapsed;
            //FindAndSolveCollisions(circleColliders, boxColliders);

            //double solving = SDL_GetPerformanceCounter();

            collisionGrid = MakeGrid();
            AddCollidersToGrid(collisionGrid);
            SolveGrid(collisionGrid);
            collisionGrid.collisionCells.Clear();

            //double solvingend = SDL_GetPerformanceCounter();
            //secondsElapsed = (solvingend - solving) / (double)SDL_GetPerformanceFrequency();

            //Console.WriteLine("time       " + secondsElapsed * 1000);
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
                if (collider == _boxColliders[i])
                {
                    continue;
                }
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
                if (collider == _circleColliders[i])
                {
                    continue;
                }
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
            Vector2 n = bPos - aPos; //distance vector a to b
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
                    boxColliders.Remove(boxColliders[i]);
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
                    circleColliders.Remove(circleColliders[i]);
                }
            }

        }

        static CollisionGrid MakeGrid()
        {
            float xMin;
            float yMin;
            float xMax;
            float yMax;
            if (circleColliders.Count() == 0 && boxColliders.Count() == 0)
            {
                return new CollisionGrid(Vector2.One, Vector2.One, cellSize);
            }
            else if (circleColliders.Count() == 0)
            {
                xMin = boxColliders.Min(x => x.rigidBody.entityTransform.position.X);
                yMin = boxColliders.Min(x => x.rigidBody.entityTransform.position.Y);
                xMax = boxColliders.Max(x => x.rigidBody.entityTransform.position.X);
                yMax = boxColliders.Max(x => x.rigidBody.entityTransform.position.Y);


            }
            else if (boxColliders.Count() == 0)
            {
                xMin = circleColliders.Min(x => x.rigidBody.entityTransform.position.X);
                yMin = circleColliders.Min(x => x.rigidBody.entityTransform.position.Y);
                xMax = circleColliders.Max(x => x.rigidBody.entityTransform.position.X);
                yMax = circleColliders.Max(x => x.rigidBody.entityTransform.position.Y);

            }
            else
            {
                xMin = MathF.Min(boxColliders.Min(x => x.rigidBody.entityTransform.position.X), circleColliders.Min(x => x.rigidBody.entityTransform.position.X));
                yMin = MathF.Min(boxColliders.Min(x => x.rigidBody.entityTransform.position.Y), circleColliders.Min(x => x.rigidBody.entityTransform.position.Y));
                xMax = MathF.Max(boxColliders.Max(x => x.rigidBody.entityTransform.position.X), circleColliders.Max(x => x.rigidBody.entityTransform.position.X));
                yMax = MathF.Max(boxColliders.Max(x => x.rigidBody.entityTransform.position.Y), circleColliders.Max(x => x.rigidBody.entityTransform.position.Y));

            }

            Vector2 topLeft = new Vector2(xMin, yMin);
            Vector2 bottomRight = new Vector2(xMax, yMax);

            return new CollisionGrid(topLeft, bottomRight, cellSize);

        }

        static void SolveGrid(CollisionGrid _collisionGrid)
        {

            for (int i = 0; i < _collisionGrid.collisionCells.Count; i++)
            {
                for (int j = 0; j < _collisionGrid.collisionCells[0].Count; j++)
                {
                    if (_collisionGrid.collisionCells[i][j].circleColliders.Count == 0 && _collisionGrid.collisionCells[i][j].boxColliders.Count == 0) { continue; }


                    List<CircleCollider> potentialCircles = getPotentialCircleCollidersFromCell(_collisionGrid, i, j);
                    List<BoxCollider> potentialBoxes = getPotentialBoxCollidersFromCell(_collisionGrid, i, j);

                    foreach (CircleCollider circle in _collisionGrid.collisionCells[i][j].circleColliders)
                    {
                        FindAndSolveCollisionsFromCollider(circle, potentialCircles, potentialBoxes);
                    }
                    foreach (BoxCollider box in _collisionGrid.collisionCells[i][j].boxColliders)
                    {
                        FindAndSolveCollisionsFromCollider(box, potentialCircles, potentialBoxes);
                    }

                    _collisionGrid.collisionCells[i][j].boxColliders.Clear();
                    _collisionGrid.collisionCells[i][j].circleColliders.Clear();



                }
            }

            foreach (BoxCollider bigBox in big_BoxColliders)
            {
                FindAndSolveCollisionsFromCollider(bigBox, circleColliders, boxColliders);


            }
            foreach (CircleCollider bigCircle in big_CircleColliders)
            {
                FindAndSolveCollisionsFromCollider(bigCircle, circleColliders, boxColliders);



            }

            FindAndSolveCollisions(big_CircleColliders, big_BoxColliders);




        }

        static List<CircleCollider> getPotentialCircleCollidersFromCell(CollisionGrid _collisiongrid, int x, int y)
        {
            int dx = -1;
            int dy = -1;
            int dxEnd = 1;
            int dyEnd = 1;

            List<CircleCollider> potentialColliders = new List<CircleCollider>();

            if (x == 0)
            {
                dx = 0;
            }
            if (y == 0) { dy = 0; }

            if (x == _collisiongrid.collisionCells.Count - 1) { dxEnd -= 1; }
            if (y == _collisiongrid.collisionCells[0].Count - 1) { dyEnd -= 1; }

            for (int i = dx; i <= dxEnd; i++)
            {
                for (int j = dy; j <= dyEnd; j++)
                {
                    potentialColliders.AddRange(_collisiongrid.collisionCells[x + i][y + j].circleColliders);

                }

            }

            return potentialColliders;



        }
        static List<BoxCollider> getPotentialBoxCollidersFromCell(CollisionGrid _collisiongrid, int x, int y)
        {
            int dx = -1;
            int dy = -1;
            int dxEnd = 1;
            int dyEnd = 1;

            List<BoxCollider> potentialColliders = new List<BoxCollider>();

            if (x == 0)
            {
                dx = 0;

            }
            if (y == 0) { dy = 0; }
            if (x == 0)
            {
                dx = 0;
            }
            if (y == 0) { dy = 0; }

            if (x == _collisiongrid.collisionCells.Count - 1) { dxEnd -= 1; }
            if (y == _collisiongrid.collisionCells[0].Count - 1) { dyEnd -= 1; }
            for (int i = dx; i <= dxEnd; i++)
            {
                for (int j = dy; j <= dyEnd; j++)
                {
                    potentialColliders.AddRange(_collisiongrid.collisionCells[x + i][y + j].boxColliders);

                }

            }

            return potentialColliders;

        }









    }






}


