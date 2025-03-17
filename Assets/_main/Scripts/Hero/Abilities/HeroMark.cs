using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeroMark : HeroAbility {
    [SerializeField, ReadOnly] List<MarkGroup> markGroups = new();

    public override void ResetAll() {
        markGroups.Clear();
    }

    public override void Process() {
        ProcessMarks();
    }

    void ProcessMarks() {
        for (int i = markGroups.Count - 1; i >= 0; i--) {
            var marks = markGroups[i].marks;
            for (int j = marks.Count - 1; j >= 0; j--) {
                if (marks[j].permanent) continue;
                marks[j].duration -= Time.deltaTime;
                if (marks[j].duration <= 0) {
                    marks.RemoveAt(j);
                }
            }

            if (marks.Count == 0) {
                markGroups.RemoveAt(i);
            }
        }
    }

    public void AddMark(Mark mark) {
        var group = markGroups.Find(g => g.key == mark.key);
        if (group == null) {
            group = new MarkGroup(mark.key);
            markGroups.Add(group);
        }
        
        group.marks.Add(mark);
    }

    public void RemoveMark(string id) {
        for (int i= markGroups.Count-1; i>=0; i--) {
            var mark = markGroups[i].marks.Find(m => m.id == id);
            if (mark != null) {
                markGroups[i].marks.Remove(mark);
                if (markGroups[i].marks.Count == 0) {
                    markGroups.RemoveAt(i);
                }
                return;
            }
        }
    }
    
    public int CountMarks(string key) {
        var group = markGroups.Find(g => g.key == key);
        return group?.marks.Count ?? 0;
    }
}

[Serializable]
public class MarkGroup {
    public string key;
    public List<Mark> marks;

    public MarkGroup(string key) {
        this.key = key;
        marks = new List<Mark>();
    }
}

[Serializable]
public class Mark {
    public string key;
    public string id;
    public float duration;
    public bool permanent;
    
    public static Mark Create(string key, float duration) {
        return new Mark {
            id = Guid.NewGuid().ToString(),
            key = key,
            duration = duration,
            permanent = false
        };
    }

    public static Mark Create(string key) {
        return new Mark {
            id = Guid.NewGuid().ToString(),
            key = key,
            duration = Mathf.Infinity,
            permanent = true
        };
    }
}