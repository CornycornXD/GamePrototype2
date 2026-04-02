using UnityEngine;

public class StatParentRuntime
{
    //  Private fields
    protected float _currentValue, _maxValue;
    protected int _arrivedLatePenalty, _deductionDuringEveningActivities, _replenishCost;

    //  Constructor
    public StatParentRuntime(StatParentData data) {
        _currentValue = data.GetBaseValue();
        _maxValue = data.GetMaxValue();
        _arrivedLatePenalty = data.GetArrivedLatePenalty();
        _deductionDuringEveningActivities = data.GetDeductionDuringEveningActivities();
        _replenishCost = data.GetReplenishCost();
    }

    //  Getter
    public float GetCurrentValue() => _currentValue;
    public float GetMaxValue() => _maxValue;
    public int GetArrivedLatePenalty() => _arrivedLatePenalty;
    public int GetDeductionDuringEveningActivities() => _deductionDuringEveningActivities;
    public int GetReplenishCost() => _replenishCost;

    //  Public methods
    public virtual void GainValue(float value) {
        _currentValue += value;
        if (_currentValue > _maxValue) {
            _currentValue = _maxValue;
        }
    }

    public virtual void DecreaseValue(float value) {
        _currentValue -= value;
        if (_currentValue < 0)
        {
            _currentValue = 0;
        }
    }


}
