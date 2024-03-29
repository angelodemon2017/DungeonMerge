using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class FiledForItemView : MonoBehaviour, IBeginDragHandler, IDropHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TextMeshProUGUI _textDamage;
    [SerializeField] private SimpleItemView _simpleItem;
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    private HeroField _heroField;
    public HeroField HeroField => _heroField;

    private HeroData _hero;
    private int _index;
    private Action<int, HeroField> _clickField;
    private Action<int, HeroField> _dropAction;
    private Action<HeroData, int, HeroField> _beginDrag;
    private Action _endDrag;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    public void Init(int number, HeroData hero, HeroField heroField,
        Action<int, HeroField> clickAction = null,
        Action<HeroData, int, HeroField> beginDrag = null,
        Action<int, HeroField> dropAct = null,
        Action endDrag = null,
        int damage = 0)
    {
        _heroField = heroField;
        _clickField = clickAction;
        _beginDrag = beginDrag;
        _endDrag = endDrag;
        _dropAction = dropAct;
        _index = number;
        _hero = hero;

        _textDamage.text = damage > 0 ? $"dam.:{damage}" : string.Empty;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_hero != null)
        {
            _image.color = _hero.HeroColor;
            _simpleItem.Init(new SimpleItemModel(_hero.Level), null);
        }
    }

    private void OnClick()
    {
        _clickField?.Invoke(_index, _heroField);
        UpdateUI();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _beginDrag?.Invoke(_hero, _index, HeroField);
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"Item OnDrop index{_index}");
        _dropAction?.Invoke(_index, _heroField);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"Item OnEndDrag index{_index}");
        _endDrag?.Invoke();
    }
}