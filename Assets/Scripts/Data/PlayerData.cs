using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerData : ScriptableObject
{
    private Config _config;

    public bool IsPremium;
    private List<PerkModel> _perkModels = new();
    public List<PerkModel> Perks => _perkModels;
    private int _diamond;
    public int Diamond
    {
        get { return _diamond; }
        set
        {
            _diamond = value;
            _diamondUpdate?.Invoke(_diamond);
        }
    }
    private int _golds = 100;
    public int Golds
    {
        get { return _golds; }
        set
        {
            _golds = value;
            _goldsUpdate?.Invoke(_golds);
        }
    }
    public int CurrentDungeonLevel = 1;
    private int _currentMinimalHeroLevel = 0;
    public int CurrentMinimalHeroLevel
    {
        get { return _currentMinimalHeroLevel + GetLevel(TypePerk.MinimalLevelHeroCards); }
        set
        {
            _currentMinimalHeroLevel = value;
            _minimalHeroLevel?.Invoke(CurrentMinimalHeroLevel);
        }
    }

    private int _cellsForSpawn = 5;
    public int CellsForSpawn
    {
        get { return _cellsForSpawn + GetLevel(TypePerk.CountMergePlaceCards); }
        set
        {
            _cellsForSpawn = value;
        }
    }
    private int _cellsInDungeon = 1;
    public int CellsInDungeon
    {
        get { return _cellsInDungeon + GetLevel(TypePerk.CountDungeonPlaceCards); }
        set
        {
            _cellsInDungeon = value;
        }
    }
    public List<HeroData> HeroesDatas = new();
    public List<HeroData> HeroesInDungeon = new();

    public Action<int> _goldsUpdate;
    public Action<int> _diamondUpdate;
    public Action<int> _minimalHeroLevel;
    public Action<List<HeroData>> _heroesDatas;
    public Action<List<HeroData>> _heroesDungeon;

    public PlayerData(Config config = null, Action<int> goldsUpdate = null, 
        Action<List<HeroData>> heroesDatas = null,
        Action<List<HeroData>> heroesDungeon = null,
        Action<int> minimalHeroLevel = null)
    {
        _config = config;

        foreach (TypePerk enumValue in Enum.GetValues(typeof(TypePerk)))
        {
            _perkModels.Add(new PerkModel(enumValue, _config));
        }
        _golds = _config.GetStartGolds;
        Diamond = _config.GetStartDiamonds;
        CurrentDungeonLevel = 1;
        _minimalHeroLevel = minimalHeroLevel;
        _goldsUpdate = goldsUpdate;
        _heroesDatas = heroesDatas;
        _heroesDungeon = heroesDungeon;

        for (int c = 1; c <= CellsForSpawn; c++)
        {
            HeroesDatas.Add(new HeroData(0, 1));
        }
        for (int c = 1; c <= CellsInDungeon; c++)
        {
            HeroesInDungeon.Add(new HeroData(0, 1));
        }

        _goldsUpdate?.Invoke(_golds);
        UpdateLists();
    }

    public void Reset()
    {
        _perkModels.ForEach(p => p.Reset());

        _golds = _config.GetStartGolds;
        Diamond = _config.GetStartDiamonds;

        HeroesDatas.Clear();
        for (int c = 1; c <= CellsForSpawn; c++)
        {
            HeroesDatas.Add(new HeroData(0, 0));
        }
        HeroesInDungeon.Clear();
        for (int c = 1; c <= CellsInDungeon; c++)
        {
            HeroesInDungeon.Add(new HeroData(0, 0));
        }
        CurrentDungeonLevel = 1;

        _minimalHeroLevel?.Invoke(CurrentMinimalHeroLevel);
        _goldsUpdate?.Invoke(_golds);
        UpdateLists();
    }

    public void TrySpawnHero()
    {
        var cost = _config.CostSpawn(CurrentMinimalHeroLevel);
        if (Golds >= cost && SpawnHero())
        {
            Golds -= cost;
            _heroesDatas?.Invoke(HeroesDatas);
        }
    }

    private bool SpawnHero()
    {
        var empHero = HeroesDatas.FirstOrDefault(h => h.IsEmpty);
        if (empHero != null)
        {
            empHero.Upgrade(CurrentMinimalHeroLevel, true, CurrentDungeonLevel);
            return true;
        }
        return false;
    }

    public void MoveHero(HeroField fromField, int fromIndex,
        HeroField toField, int toIndex)
    {
        if (fromField == toField && fromIndex == toIndex)
            return;

        if (toField == HeroField.Sell)
        {
            var trashHero = GetHeroes(fromField)[fromIndex];
            Golds += _config.CostSellHero(trashHero.Level);
            trashHero.SetEmpty();
            return;
        }

        var heroFrom = GetHeroes(fromField)[fromIndex];
        var heroTo = GetHeroes(toField)[toIndex];

        MoveHero(heroFrom, heroTo);
    }

    public void MoveHero(HeroData heroFrom, HeroData heroTo)
    {
        if (heroFrom.IsIdentity(heroTo))
        {
            heroTo.Upgrade();
            heroFrom.SetEmpty();
        }
        else
        {
            var frLv = heroFrom.Level;
            var frHC = heroFrom._heroClass;

            heroFrom.Replace(heroTo);
            heroTo.Replace(frLv, frHC);
        }
        UpdateLists();
    }

    public void UpdateLists()
    {
        CheckElements(HeroesDatas, CellsForSpawn);
        CheckElements(HeroesInDungeon, CellsInDungeon);
        _heroesDatas?.Invoke(HeroesDatas);
        _heroesDungeon?.Invoke(HeroesInDungeon);
    }

    private void CheckElements(List<HeroData> heroes, int count)
    {
        var dif = count - heroes.Count;
        if (dif > 0)
            for (int i = 0; i < dif; i++)
                heroes.Add(new HeroData(0, 0));
    }

    public void UpgradeRandomHero(int upgrade)
    {
       var heroes = HeroesDatas.Where(h => !h.IsEmpty);
        if (heroes.Count() > 0)
        {
            heroes.GetRandom().Upgrade(upgrade);
        }
        UpdateLists();
    }

    private List<HeroData> GetHeroes(HeroField typeHero)
    {
        switch (typeHero)
        {
            case HeroField.Dungeon: return HeroesInDungeon;
            case HeroField.Spawn: return HeroesDatas;
            default: return HeroesDatas;
        }
    }

    private int GetLevel(TypePerk typePerk)
    {
        return _perkModels.FirstOrDefault(p => p._typePerk == typePerk)._level;
    }

    public int GetPerkLevelByClass(HeroClass heroClass)
    {
        return GetLevel(heroClass.GetPerk());
    }

    public void UpgradePerk(TypePerk typePerk)
    {
        var perk = _perkModels.FirstOrDefault(p => p._typePerk == typePerk);
        var needDiamond = _config.GetDiamondCost(perk._level, typePerk);
        if (Diamond >= _config.GetDiamondCost(perk._level, typePerk) &&
            perk.Upgrade())
        {
            Diamond -= needDiamond;
            UpdatePerk(perk);
        }
    }

    private void UpdatePerk(PerkModel perkModel)
    {
        switch (perkModel._typePerk)
        {
            case TypePerk.MinimalLevelHeroCards:
                UpdateMinimalLevelHeroes();
                break;
            case TypePerk.CountMergePlaceCards:
                UpdateLists();
                break;
            case TypePerk.CountDungeonPlaceCards:
                UpdateLists();
                break;
            case TypePerk.DamageWarriorCards:
            case TypePerk.DamageMageCards:
            case TypePerk.DamageArcherCards:
            case TypePerk.DamagePaladinCards:
            case TypePerk.DamageDruidCards:
            case TypePerk.DamageKnightDeathCards:
                _heroesDungeon?.Invoke(HeroesInDungeon);
                break;
        }
    }

    private void UpdateMinimalLevelHeroes()
    {
        CheckMinimalLevelHero(HeroesDatas);
        CheckMinimalLevelHero(HeroesInDungeon);
        UpdateLists();
        _minimalHeroLevel?.Invoke(CurrentMinimalHeroLevel);
    }

    private void CheckMinimalLevelHero(List<HeroData> heroes)
    {
        foreach (var h in heroes)
        {
            if (h.Level > 0 && h.Level < CurrentMinimalHeroLevel)
            {
                var dif = CurrentMinimalHeroLevel - h.Level;
                h.Upgrade(dif);
            }
        }
    }

    //===================================AUTOBOT=========================

    public bool HasEmptyCell()
    {
        return HeroesDatas.Any(h => h.IsEmpty);
    }

    public HeroData GetVeryWeakFromDung()
    {
        return HeroesInDungeon.OrderBy(h => h.Level).First();
    }

    public HeroData GetVeryStrong()
    {
        return HeroesDatas.OrderByDescending(h => h.Level).First();
    }

    public bool HasMinimalEqualHeroes()
    {
        return HeroesDatas.Count(h => h.Level == CurrentMinimalHeroLevel) > 1;
    }

    public void TryMerge()
    {
        List<HeroData> tempHeroes = new();
        tempHeroes.AddRange(HeroesDatas);
        tempHeroes.AddRange(HeroesInDungeon);

        for (int x = 0; x < CellsForSpawn; x++)
        {
            if (TryMerge(tempHeroes))
            {
                break;
            }
        }
    }

    private bool TryMerge(List<HeroData> tempHeroes)
    {
        for (int j = 0; j < tempHeroes.Count; j++)
            for (int i = 0; i < tempHeroes.Count; i++)
            {
                if (!tempHeroes[i].IsEmpty &&
                    tempHeroes[i] != tempHeroes[j] &&
                    tempHeroes[i].IsIdentity(tempHeroes[j]))
                {
                    MoveHero(tempHeroes[i], tempHeroes[j]);
                    return false;
                }
            }
        return true;
    }
}