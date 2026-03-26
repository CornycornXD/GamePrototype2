using UnityEngine;

[CreateAssetMenu(fileName = "StatParentData", menuName = "Scriptable Objects/StatParentData")]
public class StatParentData : ScriptableObject
{
    [SerializeField] float _baseValue, _maxValue;

    public float GetBaseValue() {
        return _baseValue;
    }

    public float GetMaxValue() {
        return _maxValue;
    }
}
