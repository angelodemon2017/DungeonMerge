using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelUpgraded : MonoBehaviour
{
    [SerializeField] private Transform _parentPanels;
    [SerializeField] private UpgradePartView upgradePartViewPrefab;
    [SerializeField] private Button _buttonClose;
    [SerializeField] private TextMeshProUGUI _textDiamonds;

    private void Awake()
    {
        _buttonClose.onClick.AddListener(OnClosePanel);
    }

    public void Init(PlayerData playerData)
    {
        foreach (var p in playerData.Perks)
        {
            var upgView = Instantiate(upgradePartViewPrefab, _parentPanels);
            upgView.Init(p, playerData.UpgradePerk, playerData);
        }
        playerData._diamondUpdate += UpdateDiamonds;
        UpdateDiamonds(playerData.Diamond);
    }

    private void UpdateDiamonds(int diamond)
    {
        _textDiamonds.text = $"Diamonds: {diamond}";
    }

    private void OnClosePanel()
    {
        gameObject.SetActive(false);
    }
}