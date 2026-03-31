using UnityEngine;

[CreateAssetMenu(fileName = "MultiplierRelated", menuName = "Scriptable Objects/MultiplierRelated")]
public class MultiplierRelatedStatsData : StatParentData
{
    //  Private fields
    [SerializeField] int _deductionDuringWork;
    [SerializeField] int _additionDuringLunchBreak;
    [SerializeField] int _additionDuringEveningActivity;
    [SerializeField] int _additionDuringSleep;

    //  Getters
    public int GetDeductionDuringWork() => _deductionDuringWork;
    public int GetAdditionDuringLunchBreak() => _additionDuringLunchBreak;
    public int GetAdditionDuringEveningActivity() => _additionDuringEveningActivity;
    public int GetAdditionDuringSleep() => _additionDuringSleep;
}
