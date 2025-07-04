using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoUI : BaseUI {
    [SerializeField] Image thumbnailImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text priceText;
    [SerializeField] HeroInfo_Destiny[] destinies;
    [SerializeField] Button skillButton;

    [SerializeField] TMP_Text hpText;
    [SerializeField] TMP_Text energyText;
    [SerializeField] TMP_Text physicalDmgText;
    [SerializeField] TMP_Text magicalDmgText;
    [SerializeField] TMP_Text armorText;
    [SerializeField] TMP_Text resistanceText;
    [SerializeField] TMP_Text attackSpeedText;
    [SerializeField] TMP_Text energyRegenEfficientText;
    [SerializeField] TMP_Text physicalPenetrationText;
    [SerializeField] TMP_Text magicalPenetrationText;
    [SerializeField] TMP_Text criticalChanceText;
    [SerializeField] TMP_Text criticalDamageText;
    [SerializeField] TMP_Text lifeStealText;
    [SerializeField] TMP_Text tenacityText;

    HeroAttributes attributes;
    HeroSkill skill;
    HeroInventory inventory;
    
    public void Initialize(Hero hero) {
        attributes = hero.GetAbility<HeroAttributes>();
        skill = hero.GetAbility<HeroSkill>();
        inventory = hero.GetAbility<HeroInventory>();
        UpdateAttributeValues();
    }

    void UpdateAttributeValues() {
        hpText.text = $"{(int)attributes.Hp}/{(int)attributes.MaxHp}";
        physicalDmgText.text = $"{attributes.PhysicalDamage}";
        magicalDmgText.text = $"{attributes.MagicalDamage}";
        armorText.text = $"{attributes.Armor}";
        resistanceText.text = $"{attributes.Resistance}";
        energyText.text = $"{attributes.Energy}";
        attackSpeedText.text = $"{(int)(attributes.AttackSpeed*100)}%";
        energyRegenEfficientText.text = $"{(int)(attributes.EnergyRegenEfficient*100)}%";
    }
}