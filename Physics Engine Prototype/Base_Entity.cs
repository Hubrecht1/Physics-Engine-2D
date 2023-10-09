using System;
namespace Physics_Engine
{
    public class Entity
    {
        public uint ID { get; set; }

        List<Component> components = new List<Component>();

        public void AddComponent(Component component)
        {
            components.Add(component);
            component.entity = this;
        }

        public T GetComponent<T>() where T : Component
        {
            foreach (Component component in components)
            {
                if (component.GetType().Equals(typeof(T)))
                {
                    return (T)component;
                }
            }
            Console.Error.WriteLine($"Enity {ID} couldn't find {typeof(T)} component");
            return null;
        }
    }
}

