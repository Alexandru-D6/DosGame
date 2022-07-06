using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class utils
{
    
}

[System.Serializable]
public class Pair<T,K> : IEquatable<Pair<T, K>>
{
    public T first;
    public K second;

    public Pair(T a, K b) {
        first = a;
        second = b;
    }

    public Pair() {}

    public override bool Equals(object obj) {
        return Equals(obj as Pair<T, K>);
    }

    public bool Equals(Pair<T, K> other) {
        return other is not null &&
               EqualityComparer<T>.Default.Equals(first, other.first) &&
               EqualityComparer<K>.Default.Equals(second, other.second);
    }

    public override int GetHashCode() {
        return HashCode.Combine(first, second);
    }

    public static bool operator ==(Pair<T, K> left, Pair<T, K> right) {
        return EqualityComparer<Pair<T, K>>.Default.Equals(left, right);
    }

    public static bool operator !=(Pair<T, K> left, Pair<T, K> right) {
        return !(left == right);
    }
}
