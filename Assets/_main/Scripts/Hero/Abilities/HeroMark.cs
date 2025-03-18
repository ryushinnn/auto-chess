using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeroMark : HeroAbility {
    [SerializeField] MarkHolder markHolderPrefab;
    [SerializeField] Transform markHolderParent;
    [SerializeField, ReadOnly] List<MarkGroup> markGroups = new();

    List<MarkHolder> markHolders = new();

    public override void Initialize(Hero hero) {
        base.Initialize(hero);
        foreach (Transform child in markHolderParent) {
            if (child.TryGetComponent(out MarkHolder mh)) {
                markHolders.Add(mh);
                mh.gameObject.SetActive(false);
            }
        }
    }

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
                if (marks[j].duration <= 0 && marks[j].autoRemove) {
                    marks.RemoveAt(j);
                    markHolders.ForEach(x => {
                        if (x.Id == marks[j].id) {
                            x.gameObject.SetActive(false);
                        }
                    });
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

        group.marks.RemoveAll(x => x.SameAs(mark));
        group.marks.Add(mark);

        var markHolder = markHolders.Find(x => !x.gameObject.activeSelf);
        if (markHolder == null) {
            markHolder = Instantiate(markHolderPrefab, markHolderParent);
            markHolders.Add(markHolder);
        }
        
        markHolder.gameObject.SetActive(true);
        markHolder.Initialize(mark.id, StaticDataManager.Instance.GetMarkIcon(mark.key).value, mark.duration, mark.stacks);
    }

    public void RemoveMark(string id) {
        for (int i= markGroups.Count-1; i>=0; i--) {
            var mark = markGroups[i].marks.Find(m => m.id == id);
            if (mark != null) {
                markGroups[i].marks.Remove(mark);
                markHolders.ForEach(x => {
                    if (x.Id == mark.id) {
                        x.gameObject.SetActive(false);
                    }
                });
                
                if (markGroups[i].marks.Count == 0) {
                    markGroups.RemoveAt(i);
                }
                return;
            }
        }
    }

    public Mark GetMark(string key, Hero owner) {
        var group = markGroups.Find(g => g.key == key);
        return group?.marks.Find(m => m.owner == owner);
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
    public Hero owner;
    public int stacks;
    public float duration;
    public bool permanent;
    public bool autoRemove;
    
    public static Mark Create(string key, Hero owner, int stacks, float duration, bool autoRemove = true) {
        return new Mark {
            id = Guid.NewGuid().ToString(),
            key = key,
            owner = owner,
            stacks = stacks,
            duration = duration,
            permanent = false,
            autoRemove = autoRemove
        };
    }

    public static Mark Create(string key, Hero owner, int stacks) {
        return new Mark {
            id = Guid.NewGuid().ToString(),
            key = key,
            owner = owner,
            stacks = stacks,
            duration = Mathf.Infinity,
            permanent = true,
            autoRemove = false
        };
    }
    
    public bool SameAs(Mark other) {
        return key == other.key && owner == other.owner;
    }
}