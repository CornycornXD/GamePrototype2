using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MultiplierRelatedStatsRuntime : StatParentRuntime
{
    protected float _workPointsMultiplier;

    public MultiplierRelatedStatsRuntime(MultiplierRelatedStatsData data) : base(data) {
        _workPointsMultiplier = 1f;
    }

    public override void GainValue(float value)
    {
        base.GainValue(value);
        UpdateWorkPointsMultiplier();
    }

    public override void DecreaseValue(float value)
    {
        base.DecreaseValue(value);
        UpdateWorkPointsMultiplier();
    }

    private void UpdateWorkPointsMultiplier() {
        if (_currentValue > 0)
        {
            _workPointsMultiplier = 0.5f;
        }
        else if (_currentValue >= 25)
        {
            _workPointsMultiplier = 0.75f;
        }
        else if (_currentValue >= 50)
        {
            _workPointsMultiplier = 1f;
        }
        else if (_currentValue >= 75)
        {
            _workPointsMultiplier = 1.5f;
        }
        else {
            _workPointsMultiplier = 0.25f;
        }
    }
}
