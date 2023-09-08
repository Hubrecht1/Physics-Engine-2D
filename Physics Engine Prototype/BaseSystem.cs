using System;
namespace Physics_Engine_Prototype
{
    public class BaseSystem<T> where T : Component
    {
        protected static List<T> components = new List<T>();

        public static void Register(T component)
        {
            components.Add(component);
        }

        public static void Update(float dt)
        {
            foreach (T component in components)
            {
                component.Update(dt);
            }
        }
    }
}

