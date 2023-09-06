﻿using System;
using System.Collections.Generic;
using System.Numerics;

namespace Physics_Engine_Prototype
{
    public static class PhysicsWorld
    {
        static RigidBody[] rigidBodies;
        static Vector2 gravity = new Vector2(0, -9.81f);


        static void AddRigidbody()
        {



        }

        static void RemoveRigidbody(RigidBody rigidBody)
        {


        }

        static void Step(float dt)
        {



            for (int i = 0; i < rigidBodies.Length; i++)
            {
                rigidBodies[i].force += rigidBodies[i].mass * gravity;
                rigidBodies[i].velocity += rigidBodies[i].force / rigidBodies[i].mass * dt;
                rigidBodies[i].position += rigidBodies[i].velocity * dt;
                rigidBodies[i].force = Vector2.Zero;
            }




        }






    }
}

