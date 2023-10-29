using System;
using System.Numerics;
namespace Physics_Engine
{
    public readonly struct Manifold
    {
        public readonly RigidBody BodyA;
        public readonly RigidBody BodyB;
        public readonly Vector2 Normal;
        public readonly float depth;
        public readonly Vector2 Contact1;
        public readonly Vector2 Contact2;
        public readonly int ContactCount;

        public Manifold(RigidBody _BodyA, RigidBody _BodyB, Vector2 _Normal, float _depth, Vector2 _Contact1, Vector2 _Contact2, int _ContactCount)
        {
            this.BodyA = _BodyA;
            this.BodyB = _BodyB;
            this.Normal = _Normal;
            this.depth = _depth;
            this.Contact1 = _Contact1;
            this.Contact2 = _Contact2;
            this.ContactCount = _ContactCount;


        }
    }

}

