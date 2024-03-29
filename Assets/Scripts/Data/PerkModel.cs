using System;

public class PerkModel
{
    public int _level;
    public TypePerk _typePerk;
    public int _CountCards;

    private Config _config;

    public int CostDiamond => _config.GetDiamondCost(_level, _typePerk);
    public int NeedCard => _config.GetNeedCard(_level, _typePerk);
    public Action<PerkModel> _updatePerk;

    public PerkModel(TypePerk typeCurrency, Config config)
    {
        _config = config;
        _typePerk = typeCurrency;
        Reset();
    }

    public void Reset()
    {
        _level = 1;
        _CountCards = 0;
        _updatePerk?.Invoke(this);
    }

    public bool Upgrade()
    {
        var needCard = _config.GetNeedCard(_level, _typePerk);
        if (_CountCards >= needCard)
        {
            _CountCards -= needCard;
            _level++;
            _updatePerk?.Invoke(this);
            return true;
        }
        return false;
    }

    public void AddCards(int count)
    {
        _CountCards += count;
        _updatePerk?.Invoke(this);
    }
}