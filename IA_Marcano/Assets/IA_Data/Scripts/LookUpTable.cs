using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookUpTable<T, U>
{
    Func<T, U> _method;
    Dictionary<T, U> _cache;

    public LookUpTable(Func<T, U> method)
    {
        _method = method;
        _cache = new Dictionary<T, U>();
    }
    public U Run(T input)
    {
        if (!_cache.ContainsKey(input))
        {
            _cache[input] = _method(input);
        }
        return _cache[input];
    }
}
