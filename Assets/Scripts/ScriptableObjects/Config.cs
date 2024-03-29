using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObjects/Config", order = 2, fileName = "Config")]
public class Config : ScriptableObject
{
    [Header("Стартовые параметры")]
    [SerializeField] private int startGolds;
    [SerializeField] private int startDiamonds;
    [Header("Коэф. HP мобов (Level^K)")]
    [SerializeField] private float KIncreaseHPMobs = 1f;
    [Header("Коэф. HP Босса(кажд.10 ур.) (HP mob * K)")]
    [SerializeField] private float KIncreaseHPBoss = 1f;

    [Header("Коэф. награды за мобов (Level*K+1)")]
    [SerializeField] private float KIncreaseRewardMobs = 1f;
    [Header("Коэф. награды за босса (Level*K)")]
    [SerializeField] private float KIncreaseRewardBoss = 1f;

    [Header("Коэф. урона героев (LevelHero^K)")]
    [SerializeField] private float KDamageByLevelHero = 1f;
    [Header("Коэф. множителя урона героев от прокачек (DamageHero*K)")]
    [SerializeField] private float KMultipleDamageByClass = 1.5f;
    [Header("Коэф. цены спавна героев (MinLevelHero*K-MinLevelHero)")]
    [SerializeField] private float KCostForSpawn = 1f;

    [Header("Коэф. требуемых карт для прокачек (LevelPerk+K)")]
    [SerializeField] private int KCardsNeed;
    [Header("Настройка случайных наград")]
    [SerializeField] private List<ChanceRewards> RandomRewards;
    [Header("Настройка наград выпадаемых с босса")]
    [SerializeField] private List<ChanceRewards> _rewardBossChest;

    [Header("Шансы распределения карточек")]
    [SerializeField] private List<ChancePerk> RandomPerks;

    [Header("Спавн рандомной награды за каждый К пройденный уровень")]
    [SerializeField] private int RewardEveryDoneLevels = 8;
    [Header("Спавн рандомной награды за каждый К проигранный уровень")]
    [SerializeField] private int RewardEveryFailLevels = 2;
    [Header("Спавн рандомной награды за каждый К пройденный уровень(премиум)")]
    [SerializeField] private int RewardEveryDoneLevelsPremium = 2;
    [Header("Спавн рандомной награды за каждый К проигранный уровень(премиум)")]
    [SerializeField] private int RewardEveryFailLevelsPremium = 1;

    [Header("Максимальное время для прохождения уровня")]
    public float MaxTimeForDoneLevel;
    [Header("Время спавна между мобами")]
    public float TimeSpawnMobs;
    [Header("Время подготовки уровня(для красивых анимаций и прочего)")]
    public float TimePrepareLevel;
    [Header("Среднее время рекламы")]
    public float TimeADS;

    public static int MinimalLevelForArcher = 10;
    public static int MinimalLevelForPaladin = 50;
    public static int MinimalLevelForDruid = 100;
    public static int MinimalLevelForKnight = 200;

    public int GetStartGolds => startGolds;
    public int GetStartDiamonds => startDiamonds;
    public int GetRewardEveryDoneLevel(bool isPrem)
    {
        return isPrem ? RewardEveryDoneLevelsPremium : RewardEveryDoneLevels;
    }
    public int GetRewardEveryFailLevel(bool isPrem)
    {
        return isPrem ? RewardEveryFailLevelsPremium : RewardEveryFailLevels;
    }
    public List<ChanceRewards> RewardBossChest => _rewardBossChest;

    public int GetHPMonster(int level)
    {
        var result = Mathf.Pow(level, KIncreaseHPMobs) * 0.1f * (level.IsBossLevel() ?
            KIncreaseHPBoss : 1);
        /*            Mathf.Pow(level, level.IsBossLevel() 
                    ? KIncreaseHPBoss 
                    : KIncreaseHPMobs);/**/
        if (result < 1)
            result = 1;
        return (int)result;
    }

    public int GetRewardByMonster(int level)
    {
        int result;
        if (level.IsBossLevel())
        {
            result = (int)(level * KIncreaseRewardBoss);
        }
        else
        {
            result = (int)(level * KIncreaseRewardMobs + 1);
        }

        return result;
    }

    public int GetDamageHero(int levelHero)
    {
        var result = Mathf.Pow(levelHero, KDamageByLevelHero);
        return (int)result;
    }

    public float GetMultipleDamage(int level)
    {
        return level * KMultipleDamageByClass;
    }

    public int CostSpawn(int levelHero)
    {
        var result = levelHero * KCostForSpawn - (levelHero - 1);//Mathf.Pow(levelHero, 1.1f);
            //Mathf.Pow(levelHero * 10, KCostForSpawn);
        return (int)result;
    }

    public int CostSellHero(int levelHero)
    {
        return CostSpawn(levelHero) / 2;
    }

//    public int CostAddCell(int currentCells, HeroField heroField = HeroField.Spawn)
//    {
//        var result = Mathf.Pow(currentCells, KCostAddCell);
//        return (int)result;
//    }

    public int GetDiamondCost(int levelPerk, TypePerk typePerk = TypePerk.CountDungeonPlaceCards)
    {
        return levelPerk * 10;
    }

    public int GetNeedCard(int levelPerk, TypePerk typePerk = TypePerk.CountDungeonPlaceCards)
    {
        switch (typePerk)
        {
            case TypePerk.MinimalLevelHeroCards: return 3;
            case TypePerk.CountDungeonPlaceCards: return (int)(levelPerk * 1.5f);
            case TypePerk.CountMergePlaceCards: return 8;
            case TypePerk.DamageWarriorCards:
            case TypePerk.DamageMageCards:
            case TypePerk.DamageArcherCards:
            case TypePerk.DamagePaladinCards:
            case TypePerk.DamageDruidCards:
            case TypePerk.DamageKnightDeathCards:
                return levelPerk + KCardsNeed;
        }

        return levelPerk + KCardsNeed;
    }

    public ChanceRewards GetRandomReward(bool isPremium)
    {
        List<TypeReward> tempType = new();
        foreach (var rew in RandomRewards.Where(r => r.IsPremium == isPremium))
        {
            for (int k = 0; k < rew.Weight; k++)
            {
                tempType.Add(rew.typeReward);
            }
        }
        var randomType = tempType.GetRandom();
        return RandomRewards.FirstOrDefault(r => r.typeReward == randomType);
    }

    public TypePerk GetRandomPerk()
    {
        return GetRandomPerk(new());
    }

    public TypePerk GetRandomPerk(List<TypePerk> exception)
    {
        if (exception == null)
            exception = new();

        List<TypePerk> tempPerk = new();
        foreach (var perk in RandomPerks)
        {
            if (!exception.Contains(perk.typePerk))
            {
                for (int k = 0; k < perk.Weight; k++)
                {
                    tempPerk.Add(perk.typePerk);
                }
            }
        }
        return tempPerk.GetRandom();
    }
}

[System.Serializable]
public class ChanceRewards
{
    public bool IsPremium;
    public TypeReward typeReward;
    public int Weight;
    public int From;
    public int To;
}

[System.Serializable]
public class ChancePerk
{
    public TypePerk typePerk;
    public int Weight;
}