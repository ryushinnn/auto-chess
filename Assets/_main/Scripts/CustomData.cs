using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomData<T> {
    List<Pair> pairs = new();
    
    public T this[string key] {
        get {
            var pair = pairs.Find(x => x.key == key);
            return pair != null ? pair.value : default;
        }
        set {
            var pair = pairs.Find(x => x.key == key);
            if (pair != null) {
                pair.value = value;
            }
            else {
                pairs.Add(new Pair(key, value));
            }
        }
    }

    [Serializable]
    public class Pair {
        public string key;
        public T value;
        
        public Pair(string key, T value) {
            this.key = key;
            this.value = value;
        }
    }
}