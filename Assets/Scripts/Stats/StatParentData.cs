using UnityEngine;

[CreateAssetMenu(fileName = "StatParentData", menuName = "Scriptable Objects/StatParentData")]
public class StatParentData : ScriptableObject
{
    //  Private fields
    [SerializeField] float _baseValue;
    [SerializeField] float _maxValue;
    [SerializeField] int _arrivedLatePenalty;
    [SerializeField] int _deductionDuringEveningActivities;
    [SerializeField] int _replenishCost;

    //  Public methods
    public float GetBaseValue() => _baseValue;
    public float GetMaxValue() => _maxValue;
    public int GetArrivedLatePenalty() => _arrivedLatePenalty;
    public int GetDeductionDuringEveningActivities() => _deductionDuringEveningActivities;
    public int GetReplenishCost() => _replenishCost;
}
