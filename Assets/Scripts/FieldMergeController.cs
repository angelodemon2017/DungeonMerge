using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldMergeController : MonoBehaviour
{
    [SerializeField] private Transform _parentCells;
    [SerializeField] private AutoGridLayoutGroup _autoGridLayout;
    [SerializeField] private HeroField _heroField;
    [SerializeField] private FiledForItemView _fieldForItemPrefab;

    private Action<HeroData, int, HeroField> _beginDrag;
    private Action<HeroField, int, HeroField, int> _replaceAct;
    private Action _endDrag;

    private void Awake()
    {

    }

    public void Init(Action<HeroData, int, HeroField> beginDrag = null, Action endDrag = null, Action<HeroField, int, HeroField, int> replAct = null)
    {
        _beginDrag = beginDrag;
        _endDrag = endDrag;
        _replaceAct = replAct;
    }

    public void UpdateCells(List<HeroData> heroes, PlayerData _playerData, Config _config)
    {
        _parentCells.DestroyChildrens();
        _autoGridLayout.UpdateCells(heroes.Count);
        for (int i = 0; i < heroes.Count; i++)
        {
            var newCell = Instantiate(_fieldForItemPrefab, _parentCells);

            int damage = 0;
            if (_playerData != null)
            {
                var levelPerk = _playerData.GetPerkLevelByClass(heroes[i]._heroClass);
                damage = (int)(_config.GetDamageHero(heroes[i].Level) * _config.GetMultipleDamage(levelPerk));
            }
            newCell.Init(i, heroes[i], _heroField, Click, BeginDrag, DropCell, EndDrag, damage);
        }
    }

    private void Click(int index, HeroField heroField)
    {
        Debug.Log($"Click {index},{heroField}");
    }

    private void BeginDrag(HeroData hero, int index, HeroField field)
    {
        _beginDrag?.Invoke(hero, index, field);
    }

    private void DropCell(int index, HeroField heroField)
    {
        _replaceAct?.Invoke(Cursor.fromField, Cursor.fromIndex, heroField, index);
        _endDrag?.Invoke();
    }

    private void EndDrag()
    {
        _endDrag?.Invoke();
    }
}