using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class utils
{
    
}

[System.Serializable]
public class Pair<T,K>
{
    public T first;
    public K second;

    public Pair(T a, K b) {
        first = a;
        second = b;
    }

    public Pair() {}
}
