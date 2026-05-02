using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : BetterButton
{
    [SerializeField] private Image _upgradeIcon;
    private Upgrade _upgrade;

    public void Setup(Upgrade upg)
    {
        _upgrade = upg;
        _upgradeIcon.sprite = _upgrade.UpgradeIcon;

        TriggerEvent.AddListener(SelectThis);
    }

    public void SelectThis()
    {
        //set active selection
        UpgradesManager.Instance.CurrentSelection = _upgrade;

        //update UI panel text
        UpgradesManager.Instance.NameText.text = _upgrade.UpgradeName;
        UpgradesManager.Instance.DescriptionText.text = _upgrade.UpgradeDescription;
    }

    public override void Select(bool triggerSelectEvent = false)
    {
        base.Select(triggerSelectEvent);
        SelectThis();
    }
}
