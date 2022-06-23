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
    public class ItemInfoBase
    {
        public int id;
        public Transform parent;
        public Vector3 scale;
    }

    public class PlayerInfo : ItemInfoBase
    {
        
    }

    public class PlatformInfo : ItemInfoBase
    {

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

            for (int i = 0; i < 7; i++)
            {
                Create(FactoryType.Platform, new ItemInfoBase());
            }
        }
        public GameObject Create(FactoryType factory , ItemInfoBase itemInfo)
        {
            return factories[factory].Create(itemInfo);
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

