using System;
using UnityEngine;

public class InterfaceConstraintAttribute : PropertyAttribute
{
    public Type InterfaceType { get; }

    public InterfaceConstraintAttribute(Type interfaceType)
    {
        if (!interfaceType.IsInterface)
            throw new ArgumentException("Type must be an interface");

        InterfaceType = interfaceType;
    }
}