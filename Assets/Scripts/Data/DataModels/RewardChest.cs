using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RewardChest
{
    public List<RewardModel> rewards = new();

    public RewardChest(List<ChanceRewards> rewardChest, Config config, int currentDungeonLevel)
    {
        List<TypePerk> typesException = new();
        if (currentDungeonLevel < Config.MinimalLevelForArcher)
        {
            typesException.Add(TypePerk.DamageArcherCards);
        }
        if (currentDungeonLevel < Config.MinimalLevelForPaladin)
        {
            typesException.Add(TypePerk.DamagePaladinCards);
        }
        if (currentDungeonLevel < Config.MinimalLevelForDruid)
        {
            typesException.Add(TypePerk.DamageDruidCards);
        }
        if (currentDungeonLevel < Config.MinimalLevelForKnight)
        {
            typesException.Add(TypePerk.DamageKnightDeathCards);
        }

        foreach (var cr in rewardChest)
        {
            var rm = new RewardModel(cr);
            rm.SetTypePerk(config.GetRandomPerk(typesException));
            typesException.Add(rm.GetTypePerk);
            rewards.Add(rm);
        }
    }
}