using System;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class DestinyConfig : ScriptableObject {
    public int[] checkpoints;
    [TableList(ShowIndexLabels = true)] public DestinyParam[] destinyParams;
    [TextArea(1,100)] public string description;

    public abstract string GetName();

    public abstract Sprite GetIcon();

    public int GetCheckpointIndex(int current) {
        for (int i = checkpoints.Length - 1; i >= 0; i--) {
            if (current >= checkpoints[i]) return i;
        }

        return -1;
    }

    public string Description(int stage) {
        MatchEvaluator eval = match => {
            var index = int.Parse(match.Groups[1].Value);
            if (index >= 0 && index < destinyParams.Length) {
                var param = destinyParams[index];
                var val = Mathf.Abs(param.value);
                return param.isPercentage ? $"{val * 100}%" : $"{val}";
            }

            return match.Value;
        };

        var result = ReformDescription(description, eval);
        for (int i = 0; i < destinyParams.Length; i++) {
            if (i == stage) {
                result = result.Replace($"[[{i}", "<color=yellow>");
                result = result.Replace($"{i}]]", "</color>");
            }
            else {
                result = result.Replace($"[[{i}", "");
                result = result.Replace($"{i}]]", "");
            }
        }

        return result;
    }
    
    string ReformDescription(string des, MatchEvaluator eval) {
        const string pattern = @"#\[(\d+)\]";
        return Regex.Replace(des, pattern, eval);
    }
}

[Serializable]
public class DestinyParam {
    [TableColumnWidth(170, resizable:false)] 
    public string key;
    public float value;
    [TableColumnWidth(30, resizable:false), VerticalGroup("%"), LabelText("")] 
    public bool isPercentage;
}