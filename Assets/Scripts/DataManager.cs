using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    [SerializeField] private int _dayNum;
    [SerializeField] private float _dayDuration;
    [SerializeField] private List<StatParentData> _statsBaseDataList;
    private List<StatParentRuntime> _statsRuntimeDataList = new List<StatParentRuntime>();

    private void Awake()
    {
        foreach (StatParentData statsBaseData in _statsBaseDataList) {
            if (statsBaseData is MultiplierRelatedStatsData)
            {
                _statsRuntimeDataList.Add(new MultiplierRelatedStatsRuntime(statsBaseData as MultiplierRelatedStatsData));
            }
            else {
                _statsRuntimeDataList.Add(new StatParentRuntime(statsBaseData));
            }
        }
    }

    public float GetDayDuration() => _dayDuration;
    public int GetDayNum() => _dayNum;
    public List<StatParentRuntime> GetStatsRuntimeDataList() => _statsRuntimeDataList;
}
