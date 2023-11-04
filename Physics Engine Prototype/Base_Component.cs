using System;
using System.Numerics;

namespace Physics_Engine
{
    public class Component
    {
        public Entity entity;

        public virtual void Update(float dt) { }
        public virtual void Initialize() { }


    }

}

