using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "WorkQuotaData", menuName = "Scriptable Objects/WorkQuotaData")]
public class WorkQuotaData : ScriptableObject
{
    [SerializeField] private int _baseWorkQuota, _workQuotaIncrementalValue, _moneyPoints, _sanityAddition, _sanityDeduction;

    public int GetBaseWorkQuota() => _baseWorkQuota;

    public int GetWorkQuotaIncrementalValue() => _workQuotaIncrementalValue;

    public int GetMoneyPoints() => _moneyPoints;

    public int GetSanityAddition() => _sanityAddition;

    public int GetSanityDeduction() => _sanityDeduction;
}
