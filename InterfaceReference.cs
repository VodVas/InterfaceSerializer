using UnityEngine;
using System;

[Serializable]
public struct InterfaceReference<T> where T : class
{
    [SerializeField] private MonoBehaviour _implementation;

    public T Value => _implementation as T;
    public bool IsValid => _implementation != null && Value != null;
}