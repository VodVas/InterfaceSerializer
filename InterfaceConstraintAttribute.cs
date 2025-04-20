using System;
using UnityEngine;

namespace VodVas.InterfaceSerializer
{
    public class InterfaceConstraintAttribute : PropertyAttribute
    {
        public Type InterfaceType { get; }

        public InterfaceConstraintAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }
    }
}