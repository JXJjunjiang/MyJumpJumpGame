using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Factory
{
    public class ItemInfoBase
    {
        public int id;
        public Transform parent;
        public Vector3 scale;
        public ItemInfoBase(int _id, Transform _trs, Vector3 _scale)
        {
            this.id = _id;
            this.parent = _trs;
            this.scale = _scale;
        }
    }

    public class PlayerInfo : ItemInfoBase
    {
        public Vector3 position;

        public PlayerInfo(int _id, Transform _trs, Vector3 _scale, Vector3 _position) : base(_id, _trs, _scale)
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
}