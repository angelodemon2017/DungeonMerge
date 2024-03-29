using UnityEngine;

public class RewardModel
{
    public TypeReward typeReward;
    public int Amount;

    private TypePerk _perkForCardReward;

    public TypePerk GetTypePerk => _perkForCardReward;

    public RewardModel(ChanceRewards chance)
    {
        typeReward = chance.typeReward;
        Amount = Random.Range(chance.From, chance.To);
    }

    public void SetTypePerk(TypePerk typePerk)
    {
        _perkForCardReward = typePerk;
    }
}