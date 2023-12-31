using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardButton : MonoBehaviour
{
    public LevelUpgrades Upgrade;
    [SerializeField] Image Sprite;
    [SerializeField] TMPro.TMP_Text Name;
    [SerializeField] RewardSystem rewardManager;

    void OnEnable()
    {
        Sprite.sprite = Upgrade.UpgradeIcon;
        Name.text = Upgrade.UpgradeName;
    }

    public void Click()
    {
        rewardManager.Upgrade(Upgrade);
    }
}
