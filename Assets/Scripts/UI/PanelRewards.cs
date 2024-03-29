using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelRewards : MonoBehaviour
{
    [SerializeField] private Button _buttonClose;
    [SerializeField] private Transform _parentPanels;
    [SerializeField] private PanelShowOneReward _panelRewardsPrefab;

    private void Awake()
    {
        _buttonClose.onClick.AddListener(OnClose);
    }

    public void Show(List<RewardModel> rewards)
    {
        gameObject.SetActive(true);
        _parentPanels.DestroyChildrens();
        foreach (var rm in rewards)
        {
            var pr = Instantiate(_panelRewardsPrefab, _parentPanels);
            pr.Init(rm);
        }
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
    }
}