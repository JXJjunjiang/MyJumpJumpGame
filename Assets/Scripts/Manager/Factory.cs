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
        public ItemInfoBase(int _id,Transform _trs,Vector3 _scale)
        {
            this.id = _id;
            this.parent = _trs;
            this.scale = _scale;
        }
    }

    public class PlayerInfo : ItemInfoBase
    {
        public Vector3 position;

        public PlayerInfo(int _id,Transform _trs,Vector3 _scale,Vector3 _position):base (_id,_trs,_scale)
        {
            this.position = _position;
        }
    }

    public class PlatformInfo : ItemInfoBase
    {
        public Vector3 position;
        public PlatformInfo(int _id, Transform _trs, Vector3 _scale, Vector3 _position) : base(_id, _trs, _scale)
        {
            this.position = _position;
        }
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

