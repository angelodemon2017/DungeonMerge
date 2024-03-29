using System.Collections.Generic;
using UnityEngine;

public static class EnumExtensions
{
    public static HeroClass GetRandomClass(int level)
    {
        if (level <= 0)
        {
            return HeroClass.None;
        }

        List<HeroClass> heroClasses = new()
        {
            HeroClass.Warrion,
            HeroClass.Mage,
        };

        if (level > Config.MinimalLevelForArcher)
        {
            heroClasses.Add(HeroClass.Archer);
        }
        if (level > Config.MinimalLevelForPaladin)
        {
            heroClasses.Add(HeroClass.Paladin);
        }
        if (level > Config.MinimalLevelForDruid)
        {
            heroClasses.Add(HeroClass.Druid);
        }
        if (level > Config.MinimalLevelForKnight)
        {
            heroClasses.Add(HeroClass.KnightDeath);
        }

        return heroClasses.GetRandom();
    }

    public static Color GetColor(this TypeReward typeRew)
    {
        switch (typeRew)
        {
            case TypeReward.Golds: return Color.yellow;
            case TypeReward.Diamonds: return Color.cyan;
            case TypeReward.Cards: return Color.red;
            case TypeReward.RandomUpgrade: return Color.green;
        }
        return Color.white;
    }

    public static string GetName(this TypeReward typeRew)
    {
        switch (typeRew)
        {
            case TypeReward.Golds: return "Золото";
            case TypeReward.Diamonds: return "Алмазы";
            case TypeReward.Cards: return "Карточки";
            case TypeReward.RandomUpgrade: return "Левел ап случайного героя";
        }
        return "";
    }

    public static string GetName(this TypePerk typeRew)
    {
        switch (typeRew)
        {
            case TypePerk.MinimalLevelHeroCards:
                return "Минимальный уровень героя";
            case TypePerk.CountMergePlaceCards:
                return "Доп.ячейка для создания героя";
            case TypePerk.CountDungeonPlaceCards:
                return "Доп.ячейка для отряда";
            case TypePerk.DamageWarriorCards:
                return "Урон воина";
            case TypePerk.DamageMageCards:
                return "Урон мага";
            case TypePerk.DamageArcherCards:
                return "Урон лучника";
            case TypePerk.DamagePaladinCards:
                return "Урон паладина";
            case TypePerk.DamageDruidCards:
                return "Урон друида";
            case TypePerk.DamageKnightDeathCards:
                return "Урон рыцаря смерти";
        }
        return "";
    }

    public static TypePerk GetPerk(this HeroClass heroClass)
    {
        switch (heroClass)
        {
            case HeroClass.None:
                return TypePerk.CountDungeonPlaceCards;
            case HeroClass.Warrion:
                return TypePerk.DamageWarriorCards;
            case HeroClass.Mage:
                return TypePerk.DamageMageCards;
            case HeroClass.Archer:
                return TypePerk.DamageArcherCards;
            case HeroClass.Paladin:
                return TypePerk.DamagePaladinCards;
            case HeroClass.Druid:
                return TypePerk.DamageDruidCards;
            case HeroClass.KnightDeath:
                return TypePerk.DamageKnightDeathCards;
        }
        return TypePerk.CountDungeonPlaceCards;
    }

    public static HeroClass GetClass(this TypePerk typePerk)
    {
        switch (typePerk)
        {
            case TypePerk.MinimalLevelHeroCards:
            case TypePerk.CountMergePlaceCards:
            case TypePerk.CountDungeonPlaceCards:
                return HeroClass.None;
            case TypePerk.DamageWarriorCards:
                return HeroClass.Warrion;
            case TypePerk.DamageMageCards:
                return HeroClass.Mage;
            case TypePerk.DamageArcherCards:
                return HeroClass.Archer;
            case TypePerk.DamagePaladinCards:
                return HeroClass.Paladin;
            case TypePerk.DamageDruidCards:
                return HeroClass.Druid;
            case TypePerk.DamageKnightDeathCards:
                return HeroClass.KnightDeath;
        }
        return HeroClass.None;
    }

    public static Color GetColor(this HeroClass heroClass)
    {
        switch (heroClass)
        {
            case HeroClass.None: return Color.white;
            case HeroClass.Warrion: return Color.red;
            case HeroClass.Mage: return Color.blue;
            case HeroClass.Archer: return Color.green;
            case HeroClass.Paladin: return Color.yellow;
            case HeroClass.Druid: return Color.magenta;
            case HeroClass.KnightDeath: return Color.black;
        }
        return Color.white;
    }
}