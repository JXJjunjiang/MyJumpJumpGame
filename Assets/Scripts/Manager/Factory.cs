using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Factory
{
    public enum FactoryType
    {
        Character,
        Platform
    }
    public class CreateFactory : MonoSingleton<CreateFactory>,IMgrInit
    {
        private Dictionary<FactoryType, IFactory> factories;
        public void Init()
        {
            factories = new Dictionary<FactoryType, IFactory>()
            {
                {FactoryType.Character,new CharacterFactory() },
                {FactoryType.Platform,new PlatformFactory() }
            };
        }
        public GameObject Create(FactoryType factory , ItemInfoBase itemInfo)
        {
            return factories[factory].Create(itemInfo);
        }

        public void Clear()
        {
            using (var e = factories.GetEnumerator())
            {
                IDisposable dispose = null;
                while (e.MoveNext())
                {
                    dispose = e.Current.Value as IDisposable;
                    dispose.Dispose();
                }
            }
        }

        public void UnInit()
        {
            using (var e =factories.GetEnumerator())
            {
                IDisposable dispose = null;
                while (e.MoveNext())
                {
                    dispose = e.Current.Value as IDisposable;
                    dispose.Dispose();
                }
            }
            factories.Clear();
            factories = null;
        }
    }
}

