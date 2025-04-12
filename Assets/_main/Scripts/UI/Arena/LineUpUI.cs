using System;
using System.Collections.Generic;
using System.Linq;
using RExt.Extension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineUpUI : BaseUI {
    [SerializeField] LineUp_Destiny destinyPrefab;
    [SerializeField] Transform destinyContainer;
    [SerializeField] Button leftButton;
    [SerializeField] Button rightButton;
    [SerializeField] TMP_Text pageText;
    
    List<LineUp_Destiny> destinies;
    int currentPage;
    int maxPage;
    
    const int ITEM_PER_PAGE = 5;

    void Awake() {
        leftButton.onClick.AddListener(NavigateLeft);
        rightButton.onClick.AddListener(NavigateRight);
    }

    public void Initialize(Dictionary<Role, int> roleNumbers, Dictionary<Realm, int> realmNumbers) {
        destinies ??= destinyContainer.GetComponentsInChildren<LineUp_Destiny>().ToList();

        var destinyNumbers = new List<(object destiny, int number)>();
        foreach (var it in roleNumbers) {
            if (it.Value > 0) destinyNumbers.Add((it.Key, it.Value));
        }
        foreach (var it in realmNumbers) {
            if (it.Value > 0) destinyNumbers.Add((it.Key, it.Value));
        }
        destinyNumbers.Sort((nA, nB) => {
            var stagesA = nA.destiny switch {
                Role role => GameConfigs.ROLE_CONFIGS.Find(x => x.role == role).stages,
                Realm realm => GameConfigs.REALM_CONFIGS.Find(x => x.realm == realm).stages,
            };
            var unlockedA = stagesA.Count(x => x <= nA.number);

            var stagesB = nB.destiny switch {
                Role role => GameConfigs.ROLE_CONFIGS.Find(x => x.role == role).stages,
                Realm realm => GameConfigs.REALM_CONFIGS.Find(x => x.realm == realm).stages,
            };
            var unlockedB = stagesB.Count(x => x <= nB.number);

            return unlockedA != unlockedB ? unlockedB.CompareTo(unlockedA) : nB.number.CompareTo(nA.number);
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
                    var cfg = GameConfigs.ROLE_CONFIGS.Find(x => x.role == role);
                    destinies[index].Initialize(cfg, dn.number);
                    break;
                }
                case Realm realm: {
                    var cfg = GameConfigs.REALM_CONFIGS.Find(x => x.realm == realm);
                    destinies[index].Initialize(cfg, dn.number);
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