using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BodyParts : MonoBehaviour {
    [SerializeField, TableList(ShowIndexLabels = true)] BodyPart[] _parts;

    Dictionary<string, GameObject> _dic = new();
    Dictionary<string, Coroutine> _coroutines = new();

    void Awake() {
        foreach (var part in _parts) {
            part.obj.SetActive(part.enableAtStart);
            _dic.Add(part.name, part.obj);
        }
    }

    public void SetBodyParts(float delay, params (string, bool)[] datas) {
        foreach (var data in datas) {
            if (delay > 0) {
                if (_coroutines.TryGetValue(data.Item1, out var c)) {
                    if (c != null) StopCoroutine(c);
                    _coroutines.Remove(data.Item1);
                }
                _coroutines.Add(data.Item1, StartCoroutine(DoSetBodyPart(delay, data)));
            } else {
                SetBodyPart(data);
            }
        }
    }

    IEnumerator DoSetBodyPart(float delay, (string,bool) data) {
        yield return new WaitForSeconds(delay);
        SetBodyPart(data);
    }

    void SetBodyPart((string, bool) data) {
        if (_dic.TryGetValue(data.Item1, out var part)) {
            part.SetActive(data.Item2);
        }
    }
}

[Serializable]
public class BodyPart {
    public bool enableAtStart;
    public string name;
    public GameObject obj;
}