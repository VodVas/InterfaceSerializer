using System;
using System.Collections.Generic;
using UnityEngine;

namespace VodVas.InterfaceSerializer
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> _keys = new();
        [SerializeField] private List<TValue> _values = new();

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();
            foreach (var pair in this)
            {
                _keys.Add(pair.Key);
                _values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < _keys.Count; i++)
            {
                if (i < _values.Count && !ContainsKey(_keys[i]))
                    Add(_keys[i], _values[i]);
            }
        }

        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                if (ContainsKey(key)) base[key] = value;
                else Add(key, value);
            }
        }
    }
}