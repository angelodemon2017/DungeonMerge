using System;
using UnityEditor;
using UnityEngine;

public class Dungeon
{
    private Config _config;

    private HeroClass _heroClass;
    private int _leftMonsters;
    private int __countMonsters;
    private int _countMonsters
    {
        get { return __countMonsters; }
        set
        {
            __countMonsters = value;
            _Mobs?.Invoke(_leftMonsters, __countMonsters);
        }
    }
    private int _HPOneMonster;
    private int _currentHp;
    private int CurrentHp
    {
        get { return _currentHp; }
        set
        {
            _currentHp = value;
            _mobsHP?.Invoke(_currentHp, ALLHP);
        }
    }
    private int ALLHP;
    private float __currentLevelPrepare;
    private float _currentLevelPrepare 
    {
        get { return __currentLevelPrepare; }
        set 
        {
            __currentLevelPrepare = value;
            _timePrepare?.Invoke(_currentLevelPrepare);
        }
    }
    private float __leftTimeLevel;
    private float _leftTimeLevel
    {
        get { return __leftTimeLevel; }
        set
        {
            __leftTimeLevel = value;
            _timeLeft?.Invoke(__leftTimeLevel);
        }
    }
    private float _spawnTime;
    private int _currentLevel;
    public int CurrentLevel => _currentLevel;

    private readonly float _timerSpawn;
    private readonly float _TIMELevelPrepare;
    private readonly float _MAXTimeLevel;
    public float MaxTimeLevel => _MAXTimeLevel;

    public bool IsPreparing => _currentLevelPrepare > 0;
    public bool IsDone => _leftTimeLevel > 0f &&
        _leftMonsters <= 0 &&
        _countMonsters <= 0 &&
        CurrentHp <= 0;
    public bool IsFail => _leftTimeLevel <= 0f &&
        (_leftMonsters > 0 || _countMonsters > 0);

    public Action<float> _timePrepare;
    public Action<float> _timeLeft;
    public Action<int,int> _mobsHP;
    public Action<int,int> _Mobs;

    public Dungeon(Config config)
    {
        _config = config;
        _MAXTimeLevel = _config.MaxTimeForDoneLevel;
        _TIMELevelPrepare = _config.TimePrepareLevel;
        _timerSpawn = _config.TimeSpawnMobs;
    }

    public void Reset()
    {
        InitLevel(1);
    }

    public void InitLevel(int level)
    {
        _currentLevel = level;
        _heroClass = HeroClass.None;
        _leftMonsters = level.CountMobsByLevel();
        _countMonsters = 0;
        _spawnTime = 0;
        _HPOneMonster = _config.GetHPMonster(level);
        ALLHP = _leftMonsters * _HPOneMonster;
        CurrentHp = ALLHP;
        _leftTimeLevel = _MAXTimeLevel;
        _currentLevelPrepare = _TIMELevelPrepare;
//        Debug.Log($"Init level allHP:{ALLHP}");
    }

    public void ApplyTime(float time)
    {
        if (_currentLevelPrepare > 0)
        {
            _currentLevelPrepare -= time;
        }
        if (IsPreparing)
            return;

        _spawnTime += time;
        while (_spawnTime > _timerSpawn && _leftMonsters > 0)
        {
            _spawnTime -= _timerSpawn;
            _countMonsters++;
            _leftMonsters--;
        }
    }

    public int TakeDamagePerSecond(int damage, HeroClass typeDamage = HeroClass.None)
    {
        if (_countMonsters <= 0)
        {
            return 0;
        }

        var availableGetDamage = _HPOneMonster * _countMonsters;
        CurrentHp -= availableGetDamage > damage ? damage : availableGetDamage;
        var HPleftMobs = (CurrentHp - _leftMonsters * _HPOneMonster);
        var leftMobs = HPleftMobs / _HPOneMonster;
        if (HPleftMobs % _HPOneMonster > 0)
        {
            leftMobs++;
        }
        int killMobs = _countMonsters - leftMobs;
        _countMonsters = leftMobs;

        return killMobs;
    }

    public void ApplyTimeAfterFight(float time)
    {
        _leftTimeLevel -= time;
    }
}