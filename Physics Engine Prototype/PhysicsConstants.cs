using System;
using System.Numerics;

namespace Physics_Engine
{
    public static class PhysicsConstants
    {
        static public Vector2 GravityAccelaration { get; private set; } = new Vector2(0.0f, -9.81f);
        static public float inv_pixelSizeInMeters { get; private set; } = 1 / 0.02f;
        static public float airDensity = 1.293f;

    }
}

