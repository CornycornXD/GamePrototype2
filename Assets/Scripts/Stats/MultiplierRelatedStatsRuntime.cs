using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MultiplierRelatedStatsRuntime : StatParentRuntime
{
    //  Private fields
    protected float _workPointsMultiplier;
    private int _deductionDuringWork, _additionDuringLunchBreak, _additionDuringEveningActivity, _additionDuringSleep, _replenishValueUponDepletion;

    //  Constructor
    public MultiplierRelatedStatsRuntime(MultiplierRelatedStatsData data) : base(data) {
        _workPointsMultiplier = 1f;
        _deductionDuringWork = data.GetDeductionDuringWork();
        _additionDuringLunchBreak = data.GetAdditionDuringLunchBreak();
        _additionDuringEveningActivity = data.GetAdditionDuringEveningActivity();
        _additionDuringSleep = data.GetAdditionDuringSleep();
        _replenishValueUponDepletion = data.GetReplenishValueUponDepletion();
    }

    //  Getters
    public float GetWorkPointsMultiplier() => _workPointsMultiplier;
    public int GetDeductionDuringWork() => _deductionDuringWork;
    public int GetAdditionDuringLunchBreak() => _additionDuringLunchBreak;
    public int GetAdditionDuringEveningActivity() => _additionDuringEveningActivity;
    public int GetAdditionDuringSleep() => _additionDuringSleep;
    public int GetReplenishValueUponDepletion () => _replenishValueUponDepletion;

    //  Public methods
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
