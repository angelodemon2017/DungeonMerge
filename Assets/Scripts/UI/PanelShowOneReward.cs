using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PanelShowOneReward : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textNameTypeReward;
    [SerializeField] private TextMeshProUGUI _textNameTypePerk;
    [SerializeField] private TextMeshProUGUI _textCount;
    [SerializeField] private Image _iconForPerk;

    public void Init(RewardModel reward)
    {
        _textNameTypeReward.text = $"{reward.typeReward.GetName()}";

        _textNameTypePerk.text = reward.typeReward == TypeReward.Cards 
            ? $"{reward.GetTypePerk.GetName()}" : "";
        _textCount.text = $"{reward.Amount}";
//        _iconForPerk.color = reward.typeReward.GetColor();
    }
}