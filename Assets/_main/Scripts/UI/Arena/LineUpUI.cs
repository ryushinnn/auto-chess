using System.Collections.Generic;
using System.Linq;
using RExt.Extension;
using UnityEngine;

public class LineUpUI : BaseUI {
    [SerializeField] LineUp_Destiny destinyPrefab;
    [SerializeField] Transform destinyContainer;
    
    List<LineUp_Destiny> destinies;

    public void Initialize(Dictionary<Role, int> roleStages, Dictionary<Realm, int> realmStages) {
        destinies ??= destinyContainer.GetComponentsInChildren<LineUp_Destiny>().ToList();
        
        var index = 0;
        foreach (var it in roleStages) {
            var cfg = GameConfigs.ROLE_CONFIGS.Find(x => x.role == it.Key);
            if (it.Value == 0) continue;
            if (index >= destinies.Count) {
                var destiny = Instantiate(destinyPrefab, destinyContainer);
                destinies.Add(destiny);
            }

            destinies[index].gameObject.SetActive(true);
            destinies[index].Initialize(cfg, it.Value);
            index++;
        }
        
        foreach (var it in realmStages) {
            var cfg = GameConfigs.REALM_CONFIGS.Find(x => x.realm == it.Key);
            if (it.Value == 0) continue;
            if (index >= destinies.Count) {
                var destiny = Instantiate(destinyPrefab, destinyContainer);
                destinies.Add(destiny);
            }

            destinies[index].gameObject.SetActive(true);
            destinies[index].Initialize(cfg, it.Value);
            index++;
        }

        for (int i = index; i < destinies.Count; i++) {
            destinies[i].gameObject.SetActive(false);
        }
    }
}