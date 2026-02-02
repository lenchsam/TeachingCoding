using System;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    private Dictionary<string, object> _values = new Dictionary<string, object>();

    //define variable
    public void Assign(string name, object value)
    {
        _values[name] = value;
    }

    //get variable
    public object Get(string name)
    {
        if (_values.ContainsKey(name))
        {
            return _values[name];
        }
        Debug.LogError($"Undefined variable '{name}'.");
        return null;
    }
}
