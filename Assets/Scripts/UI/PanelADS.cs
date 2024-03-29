using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PanelADS : MonoBehaviour
{
    [SerializeField] private Button _cheatClose;
    [SerializeField] private Button _closeADS;
    [SerializeField] private TextMeshProUGUI _textTime;

    private RewardController _rewardController;

    private void Awake()
    {
        _cheatClose.onClick.AddListener(OnCheatButtonClose);
        _closeADS.onClick.AddListener(OnButtonClose);
    }

    public void Init(RewardController rewardController)
    {
        _rewardController = rewardController;
        _rewardController._endADSTimer += EndADSTimer;
        _rewardController._closeADSPanel += ClosePanel;
        _rewardController._updateADSTimer += UpdateTimer;
        _rewardController._pickUpRandomReward += Show;
    }

    public void Show()
    {
        if (_rewardController.IsPremium)
            return;

        gameObject.SetActive(true);
        _closeADS.gameObject.SetActive(false);
    }

    private void EndADSTimer()
    {
        _closeADS.gameObject.SetActive(true);
    }

    private void OnCheatButtonClose()
    {
        _rewardController.CloseADSPanel();
    }

    private void OnButtonClose()
    {
        _rewardController.CloseADSPanel();
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    private void UpdateTimer(float timeAds)
    {
        _textTime.text = $"{timeAds:f1}s";
    }
}