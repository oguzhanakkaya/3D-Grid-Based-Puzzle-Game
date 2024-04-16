using System;
using System.Collections.Generic;
using UnityEngine;

    public class ServiceLocator
    {
        private static ServiceLocator _instance;
        private readonly Dictionary<Type, IGameService> services = new Dictionary<Type, IGameService>();

        private ServiceLocator()
        {
        }

        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null) _instance = new ServiceLocator();

                return _instance;
            }
        }


        public T Resolve<T>() where T : IGameService
        {
            var type = typeof(T);
            if (!services.ContainsKey(type)) throw new Exception(type.Name + "not registered");

            return (T)services[type];
        }

        public void Register<T>(T service) where T : IGameService
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                Debug.LogError("Service " + type + " is already registered.");
                return;
            }

            services.Add(type, service);
            service.Initialize();
        }

        public void Unregister<T>() where T : IGameService
        {
            var type = typeof(T);
            if (!services.ContainsKey(type))
            {
                Debug.LogError("Service " + type + " is not registered.");
                return;
            }

            services.Remove(type);
        }
    }