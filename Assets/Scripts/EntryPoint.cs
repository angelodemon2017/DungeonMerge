using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EntryPoint : MonoBehaviour
{
    public static EntryPoint Instance;

    private PlayerData _playerData;
    private Dungeon _dungeon;
    private GameProcess _gameProcess;
    private RewardController _rewardController;
    [SerializeField] private Config _config;

    [SerializeField] private PanelUpgraded _panelUpgraded;
    [SerializeField] private FieldMergeController _fieldMergeController;
    [SerializeField] private BattleFieldView _battleFieldView;
    [SerializeField] private Button _buttonSpawn;
    [SerializeField] private Button _buttonGetRandomReward;
    [SerializeField] private Button _cellUpgrade;
    [SerializeField] private TextMeshProUGUI _labelSpawnCost;
    [SerializeField] private TextMeshProUGUI _balance;
    [SerializeField] private Cursor _cursor;
    [SerializeField] private Image _randomRewardIcon;
    [SerializeField] private TextMeshProUGUI _randomRewardName;
    [SerializeField] private PanelADS _panelADS;
    [SerializeField] private PanelRewards _panelRewards;
    [SerializeField] private Button _buttonReset;

    [Header("TEST FIELDS")]
    [Range(0,100)]
    [SerializeField] private float _timeSpeed = 1f;
    private float _startTimeSpeed;

    [Header("ВКЛ/ВЫКЛ Премиум(поставить до запуска)")]
    [SerializeField] private bool _PremiumEnable = false;
    [Header("Автобот(ГЛАВНЫЙ ПЕРЕКЛЮЧАТЕЛЬ): спавн, заполнение отряда")]
    [SerializeField] private bool _autoBot = false;
    [Header("Автобот: апгрейд способностей")]
    [SerializeField] private bool _autoUpgrade = false;
    [Header("Автобот: автоподбор улучшений")]
    [SerializeField] private bool _autoPickUpAdsReward = false;
    [Header("Автобот: автозакрытие наград за боссов")]
    [SerializeField] private bool _autoCloseReward = false;

    [SerializeField] private TimeSpan _time = new();
    [Header("Время игры")]
    [SerializeField] private string TotalTime;
    [Header("Максимальный уровень")]
    [SerializeField] private int _currentMaxLevel;
    [Header("Лимит уровней (-1 - без ограничений)")]
    [SerializeField] private int _stopLevel = -1;
    [Header("Лимит времени(ч) (-1 - без ограничений)")]
    [SerializeField] private int _limitPlayHours = -1;

    private float _seconds;


    private void Awake()
    {
        _startTimeSpeed = _timeSpeed;
           _seconds = 0;
        _buttonSpawn.onClick.AddListener(SpawnHero);
        _buttonGetRandomReward.onClick.AddListener(PickUpRandomReward);
        _cellUpgrade.onClick.AddListener(OpenUpgrades);
        _buttonReset.onClick.AddListener(OnReset);
        Instance = this;

        _playerData = new(_config, UpdateBalance, UpdateSpawnField,
            UpdateDungeonField, UpdateLabelSpawn);
        _playerData.IsPremium = _PremiumEnable;
        _dungeon = new (_config);
        _rewardController = new(_config, _playerData, 
            ShowRandomReward, ClosePanelRandomReward, EndADSTimer);

        _rewardController._getRewards += _panelRewards.Show;
        _battleFieldView.Init(_dungeon, _playerData, _config, _cursor.UpdateCursor, _cursor.HideCursor, _playerData.MoveHero);
        _fieldMergeController.Init(_cursor.UpdateCursor, _cursor.HideCursor, _playerData.MoveHero);
        _panelUpgraded.Init(_playerData);
        _panelADS.Init(_rewardController);

        _gameProcess = new(_playerData, _dungeon, _rewardController, _config);

        UpdateLabelSpawn(_playerData.CurrentMinimalHeroLevel);
    }

    private void Update()
    {
        var currentDeltaTime = Time.deltaTime * _timeSpeed;

        _cursor.transform.position = Input.mousePosition;
        if (_gameProcess != null)
        {
            _gameProcess.SpendTime(currentDeltaTime);
        }
        if (_autoBot)
        {
            AutoBot();
        }
        TestController();
        Timer(currentDeltaTime);
    }

    private void TestController()
    {
        if (_currentMaxLevel < _playerData.CurrentDungeonLevel)
            _currentMaxLevel = _playerData.CurrentDungeonLevel;

        if (_stopLevel > -1 && _stopLevel < _playerData.CurrentDungeonLevel)
        {
            _timeSpeed = 0;
        }
        if (_limitPlayHours > -1 && _time.TotalHours >= _limitPlayHours)
        {
            _timeSpeed = 0;
        }
    }

    private void OnReset()
    {
        _gameProcess.Reset();
        _playerData.Reset();
        _dungeon.Reset();
        _playerData.IsPremium = _PremiumEnable;
        _time = new();
        _currentMaxLevel = 0;
        _timeSpeed = _startTimeSpeed;
    }

    private void Timer(float deltaTime)
    {
        _seconds += deltaTime;
        while (_seconds > 1)
        {
            _seconds -= 1;
            _time += TimeSpan.FromSeconds(1);
            TotalTime = _time.ToString();
        }
    }

    private void UpdateSpawnField(List<HeroData> heroes)
    {
        _fieldMergeController.UpdateCells(heroes, _playerData, _config);
    }

    private void UpdateDungeonField(List<HeroData> heroes)
    {
        _battleFieldView.UpdateHeroPanels(heroes);
    }

    private void SpawnHero()
    {
        _playerData.TrySpawnHero();
    }

    private void OpenUpgrades()
    {
        _panelUpgraded.gameObject.SetActive(true);
    }

    private void ShowRandomReward(RewardModel rewardModel)
    {
        _buttonGetRandomReward.gameObject.SetActive(true);
        _randomRewardIcon.color = rewardModel.typeReward.GetColor();
        _randomRewardName.text = rewardModel.typeReward.ToString();
    }

    private void PickUpRandomReward()
    {
        _rewardController.PickUpReward();
    }

    private void EndADSTimer()
    {
        if (_autoBot)
        {
            _rewardController.CloseADSPanel();
        }
    }

    private void ClosePanelRandomReward()
    {
        _buttonGetRandomReward.gameObject.SetActive(false);
    }

    private void UpdateBalance(int balance)
    {
        _balance.text = $"Golds:{balance}$";
    }

    private void UpdateLabelSpawn(int minimalHeroLevel)
    {
        _labelSpawnCost.text = $"Spawn({_config.CostSpawn(minimalHeroLevel)}$)";
    }

    private void AutoBot()
    {
        if (_playerData.HasEmptyCell())
        {
            _playerData.TrySpawnHero();
        }

        var weakHero = _playerData.GetVeryWeakFromDung();
        var strongHero = _playerData.GetVeryStrong();
        if (weakHero != null && strongHero != null &&
            strongHero.Level > weakHero.Level)
        {
            var tempHero = new HeroData(weakHero);
            weakHero.Replace(strongHero);
            strongHero.Replace(tempHero);
            _playerData.UpdateLists();
        }

        _playerData.TryMerge();

        if (_autoUpgrade)
        {
            foreach (TypePerk enumValue in Enum.GetValues(typeof(TypePerk)))
            {
                _playerData.UpgradePerk(enumValue);
/*                switch (enumValue)
                {
                    case TypePerk.MinimalLevelHeroCards:
                    case TypePerk.CountMergePlaceCards:
                    case TypePerk.CountDungeonPlaceCards:
                    case TypePerk.DamageWarriorCards:
                    case TypePerk.DamageMageCards:
                    case TypePerk.DamageArcherCards:
                        _playerData.UpgradePerk(enumValue);
                        break;
                }/**/
            }
        }

        if (_autoPickUpAdsReward)
        {
            _rewardController.PickUpReward();
        }
        if (_autoCloseReward &&
            _panelRewards.gameObject.activeSelf)
        {
            _panelRewards.OnClose();
        }
    }
}