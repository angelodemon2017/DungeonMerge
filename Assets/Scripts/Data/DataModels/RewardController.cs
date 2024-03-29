using System.Linq;
using System;
using System.Collections.Generic;

public class RewardController
{
    private Config _config;
    private PlayerData _playerData;
    private RewardModel _randomReward;
    private bool _isShowRandomReward;

    private int _fails;
    private int _dones;
    private int _mobs;

    private float _timeADS;
    private bool _isShowADS;

    public Action<float> _updateADSTimer;
    public Action _endADSTimer;
    public Action _pickUpRandomReward;
    private Action<RewardModel> spawnRandomReward;
    public Action _closeADSPanel;
    public Action<List<RewardModel>> _getRewards;

    public bool IsADS => _isShowADS && _timeADS > 0;
    public bool IsPremium => _playerData.IsPremium;

    public RewardController(Config config, PlayerData playerData,
        Action<RewardModel> callBackSpawnRandomReward,
        Action callBackPickUpRandomReward,
        Action callBackEndADS)
//        Action<List<RewardModel>> callBackGetRewards)
    {
        _config = config;
        _playerData = playerData;
        spawnRandomReward = callBackSpawnRandomReward;
        _pickUpRandomReward = callBackPickUpRandomReward;
        _endADSTimer = callBackEndADS;
//        _getRewards = callBackGetRewards;
    }

    public void ApplyTime(float deltaTime)
    {
        if (_timeADS <= 0)
            return;

        _timeADS -= deltaTime;
        _updateADSTimer?.Invoke(_timeADS);
        if (_timeADS <= 0)
        {
            _endADSTimer?.Invoke();
        }
    }

    public void AddCounters(int done = 0, int fail = 0, int mob = 0)
    {
        if (_isShowRandomReward)
            return;

        _fails += fail;
        _dones += done;
        _mobs += mob;

        if (_fails > _config.GetRewardEveryFailLevel(IsPremium))
        {
            StartRandom();
            _fails = 0;
            _dones = 0;
            return;
        }

        if (_dones >= _config.GetRewardEveryDoneLevel(IsPremium))
        {
            StartRandom();
            _fails = 0;
            _dones = 0;
        }
    }

    private void StartRandom()
    {
        if (_isShowRandomReward)
        {
            _isShowRandomReward = false;
            _pickUpRandomReward?.Invoke();
        }

        _isShowRandomReward = true;
        _randomReward = new RewardModel(_config.GetRandomReward(_playerData.IsPremium));
        spawnRandomReward?.Invoke(_randomReward);
    }

    public void CloseADSPanel()
    {
        _timeADS = -1;
        _isShowADS = false;
        _closeADSPanel?.Invoke();
    }

    public void PickUpReward()
    {
        if (!_isShowRandomReward)
            return;

        _isShowRandomReward = false;
        if (!IsPremium)
        {
            _timeADS = _config.TimeADS;
            _isShowADS = true;
        }

        ApplyReward(_randomReward);
        _pickUpRandomReward?.Invoke();
    }

    public void GetBossChest(bool isPremium)
    {
        var rc = new RewardChest(_config.RewardBossChest.Where(r => r.IsPremium == isPremium).ToList(), _config, _playerData.CurrentDungeonLevel);
        foreach (var r in rc.rewards)
        {
            ApplyReward(r);
        }
        _getRewards?.Invoke(rc.rewards);
    }

    private void ApplyReward(RewardModel rewardModel)
    {
        switch (rewardModel.typeReward)
        {
            case TypeReward.Golds:
                _playerData.Golds += rewardModel.Amount;
                break;
            case TypeReward.Diamonds:
                _playerData.Diamond += rewardModel.Amount;
                break;
            case TypeReward.Cards:
                _playerData.Perks.FirstOrDefault(p => p._typePerk == rewardModel.GetTypePerk).AddCards(rewardModel.Amount);
                break;
            case TypeReward.RandomUpgrade:
                _playerData.UpgradeRandomHero(rewardModel.Amount);
                break;
        }
    }
}