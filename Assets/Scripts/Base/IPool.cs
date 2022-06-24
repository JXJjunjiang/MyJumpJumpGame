using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Factory
{
    public interface IPool
    {
        GameObject Request();
        void Recycle(GameObject obj);
    }
}
