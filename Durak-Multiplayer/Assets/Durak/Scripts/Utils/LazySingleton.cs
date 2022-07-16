using System;
using UnityEngine;

    public class LazySingleton<T> where T : class, new()
    {
        private LazySingleton() { }

        public static bool InstanceExists { get { return instance != null; } }

        private static readonly Lazy<T> instance = new Lazy<T>(() => new T());

        public static T Instance => instance.Value;

    }

    public abstract class LazyMonoSingleton<T> : MonoBehaviour where T : Component, new()
    {
        private static readonly Lazy<T> LazyInstance = new Lazy<T>(CreateSingleton);

        public static T Instance => LazyInstance.Value;

        private static T CreateSingleton()
        {
            var _instance = FindObjectOfType<T>();

            if (_instance == null)
            {
                GameObject obj = new GameObject
                {
                    name = typeof(T).Name
                };
                _instance = obj.AddComponent<T>();
            }

            return _instance;
        }
    }
