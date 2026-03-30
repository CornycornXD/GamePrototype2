using UnityEngine;

public class StatParentRuntime
{
    StatParentData _data;

    protected float _currentValue, _maxValue;

    public StatParentRuntime(StatParentData data) {
        _data = data;
        _currentValue = data.GetBaseValue();
        _maxValue = data.GetMaxValue();
    }

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

    public float GetCurrentValue() => _currentValue;
}
