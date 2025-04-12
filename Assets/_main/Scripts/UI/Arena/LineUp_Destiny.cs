using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineUp_Destiny : MonoBehaviour {
    [SerializeField] Image iconImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text currentNumberText;
    [SerializeField] TMP_Text[] stageTexts;
    [SerializeField] TMP_Text[] separators;
    
    public bool Empty => empty;
    
    bool empty;

    public void Initialize(RoleConfig config, int current) {
        empty = false;
        iconImage.sprite = AssetDB.Instance.GetRoleIcon(config.role).value;
        Initialize(config.stages, current);
        nameText.text = config.role.ToName();
    }

    public void Initialize(RealmConfig config, int current) {
        empty = false;
        iconImage.sprite = AssetDB.Instance.GetRealmIcon(config.realm).value;
        Initialize(config.stages, current);
        nameText.text = config.realm.ToName();
    }
    
    public void MarkAsEmpty() {
        empty = true;
    }

    void Initialize(int[] stages, int current) {
        var unlockAtLeastOne = false;
        for (int i = 0; i < stageTexts.Length; i++) {
            if (i >= stages.Length) {
                stageTexts[i].gameObject.SetActive(false);
                if (i > 0) { 
                    separators[i-1].gameObject.SetActive(false);
                }
                continue;
            }

            stageTexts[i].gameObject.SetActive(true);
            stageTexts[i].text = stages[i].ToString();
            if (current >= stages[i]) {
                var isCurrentStage = i == stages.Length - 1 || current < stages[i + 1];
                stageTexts[i].color = isCurrentStage ? Color.red : Color.white;
                stageTexts[i].fontSize = isCurrentStage ? 30 : 20;

                if (i > 0) {
                    separators[i-1].gameObject.SetActive(true);
                    separators[i-1].color = Color.white;
                }

                unlockAtLeastOne = true;
            }
            else {
                stageTexts[i].color = Color.gray;
                stageTexts[i].fontSize = 20;
                if (i > 0) {
                    separators[i-1].gameObject.SetActive(true);
                    separators[i-1].color = Color.gray;
                }
            }
        }
        currentNumberText.text = current.ToString();
        iconImage.color = unlockAtLeastOne ? Color.white : Color.gray;
        nameText.color = unlockAtLeastOne ? Color.white : Color.gray;
    }

    [Button]
    void dev_testRole(int current) {
        Initialize(new RoleConfig {
            role = Role.Cultist,
            stages = new[] { 2, 4, 6 }
        },current);
    }
    
    [Button]
    void dev_testRealm(int current) {
        Initialize(new RealmConfig {
            realm = Realm.Mecha,
            stages = new[] { 2, 4, 6,8 }
        },current);
    }
}