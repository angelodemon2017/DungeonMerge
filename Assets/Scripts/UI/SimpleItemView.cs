using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SimpleItemView : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _textLabel;
    private SimpleItemModel _simpleItem;

    public void Init(SimpleItemModel simpleItem, Action<SimpleItemModel> updateUI)
    {
        _simpleItem = simpleItem;
        updateUI += UpdateItem;
        UpdateItem();
    }

    private void UpdateItem()
    {
        _icon.sprite = _simpleItem.Icon;
        _textLabel.text = $"L.{_simpleItem.Level}";
    }

    private void UpdateItem(SimpleItemModel simpleItem)
    {
        _icon.sprite = simpleItem.Icon;
        _textLabel.text = $"L.{simpleItem.Level}";
    }
}