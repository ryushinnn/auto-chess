using System.Collections.Generic;
using System.Linq;
using RExt.Extensions;
using UnityEngine;

public class Destinies : MonoBehaviour {
    Dictionary<Role,int> roleNumbers = new();
    Dictionary<Realm,int> realmNumbers = new();
    HashSet<HeroTrait> uniqueTraits = new();

    public void Initialize() {
        UIManager_Arena.Instance.Destinies.SetData(roleNumbers, realmNumbers);
    }

    public void SetData(HeroTrait[] traits) {
        roleNumbers.Clear();
        realmNumbers.Clear();
        uniqueTraits.Clear();
        
        foreach (var trait in traits) {
            if (uniqueTraits.Add(trait)) {
                var roles = trait.role.GetAllFlags().Where(x => x != 0).ToArray();
                foreach (var role in roles) {
                    if (!roleNumbers.TryAdd(role, 1)) {
                        roleNumbers[role]++;
                    }
                }

                if (!realmNumbers.TryAdd(trait.realm, 1)) {
                    realmNumbers[trait.realm]++;
                }
            }
        }
        UIManager_Arena.Instance.Destinies.SetData(roleNumbers, realmNumbers);
    }

    public void Activate() {
        foreach (var (role, num) in roleNumbers) {
            var stages = GameConfigs.ROLE_CONFIGS[role];
            if (role == Role.Marksman) {
                for (int i = stages.Length - 1; i >= 0; i--) {
                    if (num >= stages[i]) {
                        var processor = DestinyProcessorFactory.Create(role);
                        var heroes = GameManager.Instance.BattleField.GetAliveHeroes(TeamSide.Ally);
                        processor.Activate(heroes, i);
                        break;
                    }
                }
            }
        }
    }
}