using System.Linq;
using RExt.Extension;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop_Hero : MonoBehaviour {
    [SerializeField] Image thumbnailImage;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text priceText;
    [SerializeField] Shop_Hero_Destiny[] destinies;

    public void Initialize(HeroTrait trait) {
        thumbnailImage.sprite = trait.thumbnail;
        nameText.text = trait.DisplayName();
        priceText.text = trait.price.ToString();
        destinies[0].Initialize(trait.realm);
        var roles = trait.role.GetAllFlags().ToArray();
        for (int i = 1; i <= 2; i++) {
            if (i-1>=roles.Length) {
                destinies[i].gameObject.SetActive(false);
                continue;
            }
            
            destinies[i].gameObject.SetActive(true);
            destinies[i].Initialize(roles[i-1]);
        }
    }
}