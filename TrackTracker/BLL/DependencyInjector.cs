using System;
using System.Collections.Generic;
using System.Linq;



namespace TrackTracker.BLL
{
    /*
     * Handles static service collection, implementing Dependency Injection (DI) functionality.
    */
    public static class DependencyInjector
    {
        private static Dictionary<Type, Type> dependencies = new Dictionary<Type, Type>();

        public static void AddService<Interface, Service>() // Registers a service implementation to an interface
        {
            Type TInterface = typeof(Interface);
            Type TService = typeof(Service);

            if (TService.GetInterfaces().Contains(TInterface) == false)
                throw new ArgumentException($"Cannot add dependency, because {nameof(TService)} does not implement {nameof(TInterface)}.");
            if (dependencies.ContainsKey(TInterface) == true)
                throw new ArgumentException($"Cannot add dependency, since another one is already registered to {nameof(TInterface)}.", nameof(TService));

            dependencies.Add(TInterface, TService);
        }
        public static Interface GetService<Interface>() // Instantiates and returns a service that implements the given interface
        {
            Type TInterface = typeof(Interface);

            if (dependencies.ContainsKey(TInterface) == false)
                throw new InvalidOperationException($"Cannot instantiate dependency, since {nameof(TInterface)} does not have a registered value.");

            Type TService = null;
            dependencies.TryGetValue(TInterface, out TService);

            return (Interface)Activator.CreateInstance(TService);
        }
        public static void RemoveService<Interface>() // Removes a previously registered service
        {
            Type TInterface = typeof(Interface);
            
            if (dependencies.ContainsKey(TInterface) == false)
                throw new ArgumentException($"Cannot delete {nameof(TInterface)} dependency, since no service is registered to it.", nameof(TInterface));

            dependencies.Remove(TInterface);
        }
        public static void UpdateService<Interface, Service>() // Modifies a previously registered service
        {
            // Don't need own logic, RemoveService() and AddService() already covers update functionality
            RemoveService<Interface>();
            AddService<Interface, Service>();
        }
    }
}
