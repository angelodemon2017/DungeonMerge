using UnityEditor;
using UnityEngine;

public class GameProcess
{
    private Config _config;
    private PlayerData _playerData;
    private Dungeon _dungeon;
    private RewardController _rewardController;

    private float _timeDamage = 0;
    private float TIMEDamage = 1f;
    private int _lastBoss = 0;

    public GameProcess(PlayerData playerData, Dungeon dungeon, RewardController rewardController,  Config config)
    {
        _config = config;
        _dungeon = dungeon;
        _playerData = playerData;
        _rewardController = rewardController;

        _dungeon.InitLevel(1);
    }

    public void Reset()
    {
        _timeDamage = 0;
        _lastBoss = 0;
    }

    public void SpendTime(float time)
    {
        _rewardController.ApplyTime(time);
        if (_rewardController.IsADS)
        {
            return;
        }

        _dungeon.ApplyTime(time);

        if (_dungeon.IsPreparing)
        {
            return;
        }
        else
        {
            _timeDamage += time;
        }

        while (_timeDamage > TIMEDamage)
        {
            _timeDamage -= TIMEDamage;
            foreach (var h in _playerData.HeroesInDungeon)
            {
                var levelPerk = _playerData.GetPerkLevelByClass(h._heroClass);
                var damage = (int)(_config.GetDamageHero(h.Level) * _config.GetMultipleDamage(levelPerk));
                var mobs = _dungeon.TakeDamagePerSecond(damage, h._heroClass);
                if (mobs > 0)
                {
                    if (_playerData.CurrentDungeonLevel.IsBossLevel() && _lastBoss < _playerData.CurrentDungeonLevel)
                    {
                        _lastBoss = _playerData.CurrentDungeonLevel;
                        _rewardController.GetBossChest(_playerData.IsPremium);
                    }
                    else
                    {
                        _playerData.Golds += mobs * _config.GetRewardByMonster(_playerData.CurrentDungeonLevel);
                    }
                }
            }
        }

        if (_dungeon.IsDone)
        {
            _playerData.CurrentDungeonLevel++;
            _dungeon.InitLevel(_playerData.CurrentDungeonLevel);
            _timeDamage = 0;
            _rewardController.AddCounters(1);
        }
        else
        {
            _dungeon.ApplyTimeAfterFight(time);
            if (_dungeon.IsFail)
            {
                _playerData.CurrentDungeonLevel--;
                if (_playerData.CurrentDungeonLevel < 1)
                {
                    _playerData.CurrentDungeonLevel = 1;
                }
                _dungeon.InitLevel(_playerData.CurrentDungeonLevel);
                _timeDamage = 0;
                _rewardController.AddCounters(fail: 1);
            }
        }
    }
}