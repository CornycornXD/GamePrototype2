using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

public class DataManager : MonoBehaviour
{
    //  General set up stats
    [Header("Set up")]
    [SerializeField] private int _dayNum;
    [SerializeField] private float _dayDuration;
    private int _currentDayNum = 0;
    private bool _isTheEndOfWeek, _gameCompleted;

    // Base stats
    [Header("Stats")]
    [SerializeField] private StatParentData _moneyBaseData;
    [SerializeField] private MultiplierRelatedStatsData _healthBaseData;
    [SerializeField] private MultiplierRelatedStatsData _sanityBaseData;

    private StatParentRuntime _moneyRuntimeData;
    private MultiplierRelatedStatsRuntime _healthRuntimeData;
    private MultiplierRelatedStatsRuntime _sanityRuntimeData;

    //  Other stats
    [SerializeField] private int _baseWorkPoints;

    //  Work quotas
    [Header("Work quotas")]
    [SerializeField] private WorkQuotaData _dailyWorkQuotaBaseData;
    [SerializeField] private WorkQuotaData _weeklyWorkQuotaBaseData;

    private WorkQuotaRuntime _dailyWorkQuotaRuntimeData;
    private WorkQuotaRuntime _weeklyWorkQuotaRuntimeData;

    //  Events
    public static event Action<int, int, int> OnStatsChanged;
    public static event Action<int, int> OnWorkProgressChanged;
    public static event Action<int, int> OnWorkQuotaChanged;

    public static event Action<bool, int, int, int> OnGameEnd;

    private void Start()
    {
        _gameCompleted = false;

        InitialiseStatData();
        InitialiseWorkQuotaData();
    }

    private void OnEnable()
    {
        QTEHandler.OnCameToWorkLate += HandleOnCameToWorkLate;

        GameManager.OnWorkingBeforeNoon += HandleOnWorkingBeforeNoon;
        GameManager.OnWorkStatsChange += HandleOnWorkStatsChange;
        GameManager.OnEveningActivitiesStatsChange += HandleOnEveningActivitiesStatsChange;
        GameManager.OnRestingStatsChange += HandleOnRestingStatsChange;
        GameManager.OnDayEnd += HandleOnDayEnd;
    }

    private void OnDisable()
    {
        QTEHandler.OnCameToWorkLate -= HandleOnCameToWorkLate;

        GameManager.OnWorkingBeforeNoon -= HandleOnWorkingBeforeNoon;
        GameManager.OnWorkStatsChange += HandleOnWorkStatsChange;
        GameManager.OnEveningActivitiesStatsChange -= HandleOnEveningActivitiesStatsChange;
        GameManager.OnRestingStatsChange -= HandleOnRestingStatsChange;
        GameManager.OnDayEnd -= HandleOnDayEnd;
    }

    //  Getters
    public float GetDayDuration() => _dayDuration;
    public int GetDayNum() => _dayNum;
    public StatParentRuntime GetMoneyRuntimeData() => _moneyRuntimeData;
    public MultiplierRelatedStatsRuntime GetHealthRuntimeData() => _healthRuntimeData;
    public MultiplierRelatedStatsRuntime GetSanityRuntimeData() => _sanityRuntimeData;
    public int GetCurrentDayNum() => _currentDayNum;

    //  Time period handlers
    private void HandleOnWorkingBeforeNoon(bool flag, bool firstDay)
    {
        if (!firstDay)
        {
            CheckWorkQuota();
            InitialiseWorkQuota();
        }
    }

    private void HandleOnDayEnd()
    {
        _currentDayNum++;
        _isTheEndOfWeek = _currentDayNum % 7 == 0;

        if (_currentDayNum > _dayNum) {
            _gameCompleted = true;

            OnGameEnd(_gameCompleted, (int)_moneyRuntimeData.GetCurrentValue(), (int)_healthRuntimeData.GetCurrentValue(), (int)_sanityRuntimeData.GetCurrentValue());
        }
    }

    //  Stats change handlers
    private void HandleOnCameToWorkLate() {
        _moneyRuntimeData.DecreaseValue(_moneyRuntimeData.GetArrivedLatePenalty());
        _sanityRuntimeData.DecreaseValue(_sanityRuntimeData.GetArrivedLatePenalty());

        UponStatsChanged();
    }

    private void HandleOnWorkStatsChange()
    {
        _dailyWorkQuotaRuntimeData.IncreaseCurrentWorkProgress(GetWorkProgressIncrementalValue());
        _weeklyWorkQuotaRuntimeData.IncreaseCurrentWorkProgress(GetWorkProgressIncrementalValue());

        _healthRuntimeData.DecreaseValue(_healthRuntimeData.GetDeductionDuringWork());
        _sanityRuntimeData.DecreaseValue(_sanityRuntimeData.GetDeductionDuringWork());

        UponStatsChanged();
        OnWorkProgressChanged?.Invoke(_dailyWorkQuotaRuntimeData.GetCurrentWorkProgress(), _weeklyWorkQuotaRuntimeData.GetCurrentWorkProgress());
    }

    private void HandleOnEveningActivitiesStatsChange(bool workingOut, bool hangingOut)
    {
        if (workingOut)
        {
            _healthRuntimeData.GainValue(_healthRuntimeData.GetAdditionDuringEveningActivity());
        }
        else if (hangingOut)
        {
            _sanityRuntimeData.GainValue(_sanityRuntimeData.GetAdditionDuringEveningActivity());
        }
        _moneyRuntimeData.DecreaseValue(_moneyRuntimeData.GetDeductionDuringEveningActivities());

        UponStatsChanged();
    }

    private void HandleOnRestingStatsChange(bool havingLunchBreak, bool sleeping)
    {
        if (havingLunchBreak)
        {
            _healthRuntimeData.GainValue(_healthRuntimeData.GetAdditionDuringSleep());
            _sanityRuntimeData.GainValue(_sanityRuntimeData.GetAdditionDuringSleep());
        }
        else if (sleeping)
        {
            _healthRuntimeData.GainValue(_healthRuntimeData.GetAdditionDuringLunchBreak());
            _sanityRuntimeData.GainValue(_sanityRuntimeData.GetAdditionDuringLunchBreak());
        }

        UponStatsChanged();
    }

    //  Work Quota related methods
    private void CheckWorkQuota()
    {
        CheckDailyWorkQuota();
        CheckWeeklyWorkQuota();
    }

    private void CheckDailyWorkQuota()
    {
        if (_dailyWorkQuotaRuntimeData.GetCurrentWorkProgress() >= _dailyWorkQuotaRuntimeData.GetCurrentWorkQuota())
        {
            _sanityRuntimeData.GainValue(_dailyWorkQuotaRuntimeData.GetSanityAdditionUponQuotaMet());
            _dailyWorkQuotaRuntimeData.TaskCompleted();
        }
        else
        {
            _sanityRuntimeData.DecreaseValue(_dailyWorkQuotaRuntimeData.GetSanityDeductionUponQuotaFailed());
        }
        _moneyRuntimeData.GainValue(CalculateMoneyReward(_dailyWorkQuotaRuntimeData));

        UponStatsChanged();
    }

    private void CheckWeeklyWorkQuota()
    {
        if (!_isTheEndOfWeek)
            return;

        if (_weeklyWorkQuotaRuntimeData.GetCurrentWorkProgress() >= _weeklyWorkQuotaRuntimeData.GetCurrentWorkQuota())
        {
            _sanityRuntimeData.GainValue(_weeklyWorkQuotaRuntimeData.GetSanityAdditionUponQuotaMet());
            _weeklyWorkQuotaRuntimeData.TaskCompleted();
        }
        else
        {
            _sanityRuntimeData.DecreaseValue(_weeklyWorkQuotaRuntimeData.GetSanityDeductionUponQuotaFailed());
        }
        _moneyRuntimeData.GainValue(CalculateMoneyReward(_weeklyWorkQuotaRuntimeData));

        UponStatsChanged();
    }

    private int CalculateMoneyReward(WorkQuotaRuntime workQuota) {
        if (workQuota.GetQuotaMet()) { 
            return workQuota.GetMoneyPoints();
        }
        return (int)Mathf.Round(workQuota.GetMoneyPoints() * ((float)workQuota.GetCurrentWorkProgress() / _weeklyWorkQuotaRuntimeData.GetCurrentWorkQuota()));
    }

    private void InitialiseWorkQuota()
    {
        if (_dailyWorkQuotaRuntimeData.GetQuotaMet())
        {
            _dailyWorkQuotaRuntimeData.IncreaseWorkQuota();
        }

        _dailyWorkQuotaRuntimeData.InitialiseWorkProgress();

        if (!_isTheEndOfWeek)
            return;

        if (_weeklyWorkQuotaRuntimeData.GetQuotaMet())
        {
            _weeklyWorkQuotaRuntimeData.IncreaseWorkQuota();
        }
        _weeklyWorkQuotaRuntimeData.InitialiseWorkProgress();

        //  UI
        OnWorkProgressChanged?.Invoke(_dailyWorkQuotaRuntimeData.GetCurrentWorkProgress(), _weeklyWorkQuotaRuntimeData.GetCurrentWorkProgress());
        OnWorkQuotaChanged?.Invoke(_dailyWorkQuotaRuntimeData.GetCurrentWorkQuota(), _weeklyWorkQuotaRuntimeData.GetCurrentWorkQuota());
    }

    //  Other methods
    private void InitialiseStatData()
    {
        _moneyRuntimeData = new StatParentRuntime(_moneyBaseData);
        _healthRuntimeData = new MultiplierRelatedStatsRuntime(_healthBaseData);
        _sanityRuntimeData = new MultiplierRelatedStatsRuntime(_sanityBaseData);
    }

    private void InitialiseWorkQuotaData()
    {
        _dailyWorkQuotaRuntimeData = new WorkQuotaRuntime(_dailyWorkQuotaBaseData);
        _weeklyWorkQuotaRuntimeData = new WorkQuotaRuntime(_weeklyWorkQuotaBaseData);
    }

    private int GetWorkProgressIncrementalValue()
    {
        return (int)Mathf.Round(_baseWorkPoints * _healthRuntimeData.GetWorkPointsMultiplier() * _sanityRuntimeData.GetWorkPointsMultiplier());
    }

    private void UponStatsChanged() {
        if (CheckStatDepletion(_moneyRuntimeData))
        {
            if (CheckStatDepletion(_healthRuntimeData) && CheckStatDepletion(_sanityRuntimeData))
            {
                //  GameOver
                OnGameEnd(_gameCompleted, (int)_moneyRuntimeData.GetCurrentValue(), (int)_healthRuntimeData.GetCurrentValue(), (int)_sanityRuntimeData.GetCurrentValue());
            }
        }
        else {
            if (CheckStatDepletion(_healthRuntimeData)) {
                ReplenishNonMoneyStat(_healthRuntimeData);
            }

            if (CheckStatDepletion(_sanityRuntimeData)) {
                ReplenishNonMoneyStat(_sanityRuntimeData);
            }
        }
        //  UI 
        OnStatsChanged?.Invoke((int)_moneyRuntimeData.GetCurrentValue(), (int)_healthRuntimeData.GetCurrentValue(), (int)_sanityRuntimeData.GetCurrentValue());
    }

    private bool CheckStatDepletion(StatParentRuntime stat) {
        if (stat.GetCurrentValue() == 0) {
            return true;
        }
        return false;
    }

    private void ReplenishNonMoneyStat(MultiplierRelatedStatsRuntime stat) {
        if (_moneyRuntimeData.GetCurrentValue() >= _moneyRuntimeData.GetReplenishCost()) {
            _moneyRuntimeData.DecreaseValue(_moneyRuntimeData.GetReplenishCost());
            stat.GainValue(stat.GetReplenishValueUponDepletion());
        }
    }
}
