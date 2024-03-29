using UnityEditor;
using UnityEngine;

public class HeroData
{
    public int Level;
    public HeroClass _heroClass;

    public Color HeroColor => _heroClass.GetColor();// GetHeroColor();
    public bool IsEmpty => Level == 0;

    public HeroData(int minLevel = 1, int levelDungeon = 1)
    {
        Level = minLevel;
        _heroClass = EnumExtensions.GetRandomClass(levelDungeon);
    }

    public HeroData(HeroData tempHero)
    {
        Level = tempHero.Level;
        _heroClass = tempHero._heroClass;
    }

    public void SetEmpty()
    {
        Level = 0;
        _heroClass = HeroClass.None;
    }

    public void Upgrade(int levelUpgrade = 1, bool changeClass = false, int levelDungeon = 1)
    {
        Level += levelUpgrade;
        if (changeClass)
        {
            _heroClass = EnumExtensions.GetRandomClass(levelDungeon);
        }
    }

    private Color GetHeroColor()
    {
        if (Level <= 0)
        {
            return Color.white;
        }
        switch (_heroClass)
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

    public void Replace(HeroData heroData)
    {
        Replace(heroData.Level, heroData._heroClass);
    }

    public void Replace(int level, HeroClass heroClass)
    {
        Level = level;
        _heroClass = heroClass;
    }

    public bool IsIdentity(HeroData hero)
    {
        return Level > 0 && Level == hero.Level && _heroClass == hero._heroClass;
    }
}