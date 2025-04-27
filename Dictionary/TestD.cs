using UnityEngine;
using VodVas.InterfaceSerializer;

public class TestD : MonoBehaviour
{
    [SerializeField] private SerializableDictionary<float, Test> d;
}

public enum Test
{
    Boss,
    Enemy,
    Player
}