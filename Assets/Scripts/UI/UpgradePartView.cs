using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradePartView : MonoBehaviour
{
    [SerializeField] private Button _buttonUpgrade;
    [SerializeField] private TextMeshProUGUI _textName;
    [SerializeField] private TextMeshProUGUI _textLevel;
    [SerializeField] private TextMeshProUGUI _textCostCrystal;
    [SerializeField] private TextMeshProUGUI _textCards;
    [SerializeField] private TypePerk _typePerk;
    private Action<TypePerk> _updAction;
    private PerkModel _perkModel;

    public void Init(PerkModel perkModel, Action<TypePerk> updAction, PlayerData playerData)
    {
        _perkModel = perkModel;
        _updAction = updAction;
        _typePerk = perkModel._typePerk;
        _buttonUpgrade.onClick.AddListener(TryUpgrade);
        _textName.text = perkModel._typePerk.GetName();
        _perkModel._updatePerk += UpdatePerkView;
        UpdatePerkView(_perkModel);

        _buttonUpgrade.GetComponent<Image>().color = perkModel._typePerk.GetClass().GetColor();
    }

    private void TryUpgrade()
    {
        _updAction?.Invoke(_typePerk);
    }

    private void UpdatePerkView(PerkModel perkModel)
    {
        if (perkModel._typePerk != _typePerk)
            return;

        _textLevel.text = $"L:{perkModel._level}";
        _textCostCrystal.text = $"Cost:{perkModel.CostDiamond}";
        _textCards.text = $"Cards:{perkModel._CountCards}/{perkModel.NeedCard}";
    }

    private void OnDestroy()
    {
        _buttonUpgrade.onClick.RemoveAllListeners();
        _perkModel._updatePerk -= UpdatePerkView;
    }
}