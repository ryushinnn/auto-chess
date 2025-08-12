using System;
using System.Collections.Generic;
using System.Linq;
using RExt.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DestiniesUI : BaseUI {
    [SerializeField] Destinies_Destiny destinyPrefab;
    [SerializeField] Transform destinyContainer;
    [SerializeField] Button leftButton;
    [SerializeField] Button rightButton;
    [SerializeField] TMP_Text pageText;

    List<Destinies_Destiny> destinies = new();
    int currentPage;
    int maxPage;
    
    const int ITEM_PER_PAGE = 5;

    void Awake() {
        leftButton.onClick.AddListener(NavigateLeft);
        rightButton.onClick.AddListener(NavigateRight);
        destinyContainer.ProcessChildren<Destinies_Destiny>(d => destinies.Add(d));
    }

    public void SetData(Dictionary<Role, int> roleNumbers, Dictionary<Realm, int> realmNumbers) {
        var destinyNumbers = new List<(object destiny, int number)>();
        foreach (var (role,num) in roleNumbers) {
            destinyNumbers.Add((role, num));
        }
        foreach (var (realm,num) in realmNumbers) {
            destinyNumbers.Add((realm, num));
        }
        destinyNumbers.Sort((dnA, dnB) => {
            var stagesA = dnA.destiny switch {
                Role role => GameConfigs.ROLE_CONFIGS[role],
                Realm realm => GameConfigs.REALM_CONFIGS[realm],
            };
            var unlockedA = stagesA.Count(x => x <= dnA.number);

            var stagesB = dnB.destiny switch {
                Role role => GameConfigs.ROLE_CONFIGS[role],
                Realm realm => GameConfigs.REALM_CONFIGS[realm],
            };
            var unlockedB = stagesB.Count(x => x <= dnB.number);

            return unlockedA != unlockedB ? unlockedB.CompareTo(unlockedA) : dnB.number.CompareTo(dnA.number);
        });
        
        var index = 0;
        foreach (var dn in destinyNumbers) {
            if (index >= destinies.Count) {
                var destiny = Instantiate(destinyPrefab, destinyContainer);
                destinies.Add(destiny);
            }

            destinies[index].gameObject.SetActive(true);
            switch (dn.destiny) {
                case Role role: {
                    destinies[index].SetData(role, dn.number);
                    break;
                }
                case Realm realm: {
                    destinies[index].SetData(realm, dn.number);
                    break;
                }
            }
            index++;
        }

        for (int i = index; i < destinies.Count; i++) {
            destinies[i].gameObject.SetActive(false);
            destinies[i].MarkAsEmpty();
        }
        
        maxPage = (index + ITEM_PER_PAGE - 1) / ITEM_PER_PAGE - 1;
        currentPage = Mathf.Clamp(currentPage, 0, maxPage);
        RefreshCurrentPage();
    }

    void NavigateLeft() {
        currentPage--;
        if (currentPage < 0) currentPage = maxPage;
        RefreshCurrentPage();
    }

    void NavigateRight() {
        currentPage++;
        if (currentPage > maxPage) currentPage = 0;
        RefreshCurrentPage();
    }

    void RefreshCurrentPage() {
        var index = 0;
        foreach (var d in destinies) {
            if (d.Empty) continue;
            d.gameObject.SetActive(index / ITEM_PER_PAGE == currentPage);
            index++;
        }
        pageText.text = $"{currentPage+1}/{maxPage+1}";
    }
}