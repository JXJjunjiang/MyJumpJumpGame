using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Factory;

namespace Factory
{
    public interface IFactory
    {
        GameObject Create(ItemInfoBase itemInfo);
    }
}
