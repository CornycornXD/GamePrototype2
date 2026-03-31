using UnityEngine;

[CreateAssetMenu(fileName = "StatParentData", menuName = "Scriptable Objects/StatParentData")]
public class StatParentData : ScriptableObject
{
    //  Private fields
    [SerializeField] float _baseValue;
    [SerializeField] float _maxValue;
    [SerializeField] int _arrivedLatePenalty;
    [SerializeField] int _deductionDuringEveningActivities;

    //  Public methods
    public float GetBaseValue() => _baseValue;
    public float GetMaxValue() => _maxValue;
    public int GetArrivedLatePenalty() => _arrivedLatePenalty;
    public int GetDeductionDuringEveningActivities() => _deductionDuringEveningActivities;
}
