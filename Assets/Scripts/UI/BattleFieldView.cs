using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class BattleFieldView : MonoBehaviour
{
    [SerializeField] private Transform _panelCells;
    [SerializeField] private AutoGridLayoutGroup _autoGridLayout;
    [SerializeField] private FiledForItemView _fieldForItemPrefab;
    [SerializeField] private HeroField _heroField;

    [SerializeField] private TextMeshProUGUI _currentLevelDungeon;
    [SerializeField] private TextMeshProUGUI textTimePrepare;
    [SerializeField] private TextMeshProUGUI textTimeLeft;
    [SerializeField] private TextMeshProUGUI textHP;
    [SerializeField] private TextMeshProUGUI textMobs;
    [SerializeField] private TextMeshProUGUI textDamage;

    [SerializeField] private Slider _sliderHP;
    [SerializeField] private Slider _sliderHPLock;
    [SerializeField] private Slider _sliderLeftTime;

    private Dungeon _dungeon;
    private PlayerData _playerData;
    private Config _config;

    private Action<HeroData, int, HeroField> _beginDrag;
    private Action<HeroField, int, HeroField, int> _replaceAct;
    private Action _endDrag;

    public void Init(Dungeon dungeon, PlayerData playerData, Config config, Action<HeroData, int, HeroField> beginDrag = null, Action endDrag = null, Action<HeroField, int, HeroField, int> replAct = null)
    {
        _dungeon = dungeon;
        _playerData = playerData;
        _config = config;
        _dungeon._timeLeft += UpdateTimeLeft;
        _dungeon._timePrepare += UpdateTimePrepare;
        _dungeon._mobsHP += UpdateHP;
        _dungeon._Mobs += UpdateMobs;

        _beginDrag = beginDrag;
        _endDrag = endDrag;
        _replaceAct = replAct;

        _sliderLeftTime.maxValue = _dungeon.MaxTimeLevel;
    }

    public void UpdateHeroPanels(List<HeroData> heroes)
    {
        _panelCells.DestroyChildrens();

        int totalDamage = 0;
        _autoGridLayout.UpdateCells(6, 2);
/*        if (heroes.Count < 7)
        {
            _autoGridLayout.UpdateCells(6, 1);
        }
        else
        {
            _autoGridLayout.UpdateCells(8, 2);
        }/**/
        for (int i = 0; i < heroes.Count; i++)
        {
            int damage = 0;
            if (_playerData != null)
            {
                var levelPerk = _playerData.GetPerkLevelByClass(heroes[i]._heroClass);
                damage = (int)(_config.GetDamageHero(heroes[i].Level) * _config.GetMultipleDamage(levelPerk));
                totalDamage += damage;
            }

            var newCell = Instantiate(_fieldForItemPrefab, _panelCells);
            newCell.Init(i, heroes[i], _heroField, Click, BeginDrag, DropCell, EndDrag, damage);
        }
        textDamage.text = $"damage: {totalDamage} per second";
    }

    private void UpdateTimePrepare(float timePrepare)
    {
        textTimePrepare.text = $"{timePrepare:F1}";
    }

    private void UpdateTimeLeft(float timeLeft)
    {
        textTimeLeft.text = $"{timeLeft:F1}";
        _sliderLeftTime.value = timeLeft;
    }

    private void UpdateHP(int current, int max)
    {
        textHP.text = $"{current}/{max} HP";
        _sliderHP.maxValue = max;
        _sliderHP.value = current;
        _sliderHPLock.maxValue = _dungeon.CurrentLevel.CountMobsByLevel();
        _currentLevelDungeon.text = $"L:{_dungeon.CurrentLevel}";
    }

    private void UpdateMobs(int left, int current)
    {
        textMobs.text = $"mobs: {current}/{left}";
        _sliderHPLock.value = left;
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

    private void OnDestroy()
    {
        _dungeon._timeLeft -= UpdateTimeLeft;
        _dungeon._timePrepare -= UpdateTimePrepare;
        _dungeon._mobsHP -= UpdateHP;
        _dungeon._Mobs -= UpdateMobs;
    }
}