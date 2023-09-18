using System;
using System.Numerics;

namespace Physics_Engine_Prototype
{
    public static class PhysicsConstants
    {
        static public Vector2 gravityAccelaration { get; private set; } = new Vector2(0.0f, -9.81f);
        static public float pixelSizeInMeters { get; private set; } = 1 / 0.1f;


    }
}

