using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class DataManager : MonoBehaviour
{
    //  General set up stats
    [Header("Set up")]
    [SerializeField] private int _dayNum;
    [SerializeField] private float _dayDuration;

    // Base stats - cleaned up
    [SerializeField] private StatParentRuntime _moneyBaseData;
    [SerializeField] private MultiplierRelatedStatsData _healthBaseData;
    [SerializeField] private MultiplierRelatedStatsData _sanityBaseData;

    private StatParentRuntime _moneyRuntimeData;
    private MultiplierRelatedStatsData _healthRuntimeData;
    private MultiplierRelatedStatsData _sanityRuntimeData;


    //  QTE
    // QTEController.cs
    [Header("QTE")]
    [SerializeField] int _QTEInputNum;
    private char[] _QTECharArr = { 'W', 'A', 'S', 'D' };
    private List<char> _QTEGeneratedInputCombination = new List<char>();
    private List<char> _QTEUserInputCombination = new List<char>();
    private int _currentQTEIndex;
    private bool _QTEAvailable;

    //  Work quotas
    [Header("Work quotas\' related stats")]
    [SerializeField] private List<WorkQuotaData> _workQuotaDataList;
    private List<WorkQuotaRuntime> _workQuotaRuntimeList = new List<WorkQuotaRuntime>();

    //  Work quotas - cleaned up
    [SerializeField] private WorkQuotaData _dailyWorkQuotaBaseData;
    [SerializeField] private WorkQuotaData _weeklyWorkQuotaBaseData;

    private WorkQuotaRuntime _dailyWorkQuotaRuntimeData;
    private WorkQuotaRuntime _weeklyWorkQuotaRuntimeData;


    // end
    [Header("Managers")]
    [SerializeField] GameManager _gameManager;

    private void Awake()
    {

    }

    private void InitializeStatData()
    }
        
    }

    private void InitializeWorkQuotaData()
    {
        
    }

    private void OnEnable()
    {
        InputHandler.WASDEntered += HandleSequenceQTEInput;

        GameManager.OnMorning += HandlingOnMorning;
        GameManager.OnWorkingBeforeNoon += HandlingOnWorkingBeforeNoon;
        GameManager.OnWorkStatsChange += HandlingOnWorkStatsChange;
        GameManager.OnEveningActivitiesStatsChange += HandlingOnEveningActivitiesStatsChange;
        GameManager.OnRestingStatsChange += HandlingOnRestingStatsChange;
    }

    private void OnDisable()
    {
        InputHandler.WASDEntered -= HandleSequenceQTEInput;

        GameManager.OnMorning -= HandlingOnMorning;
        GameManager.OnWorkingBeforeNoon -= HandlingOnWorkingBeforeNoon;
        GameManager.OnWorkStatsChange += HandlingOnWorkStatsChange;
        GameManager.OnEveningActivitiesStatsChange -= HandlingOnEveningActivitiesStatsChange;
        GameManager.OnRestingStatsChange -= HandlingOnRestingStatsChange;
    }

    //  Getters
    public float GetDayDuration() => _dayDuration;
    public int GetDayNum() => _dayNum;
    public List<StatParentRuntime> GetStatsRuntimeDataList() => _statsRuntimeDataList;
    public int GetMoneyDeductionDuringEveningActivities() => _moneyDeductionDuringEveningActivities;

    //  Time period handlers
    private void HandlingOnMorning(bool flag)
    {
        EnablingQTE(flag);
    }

    private void HandlingOnWorkingBeforeNoon(bool flag, bool firstDay)
    {
        DisablingQTE(flag);
        if (!firstDay)
        {
            CheckWorkQuota();
        }
        InitialiseWorkQuota(firstDay);
    }

    //  Stats change handlers
    private void HandlingOnWorkStatsChange()
    {
        foreach (WorkQuotaRuntime workQuotaRuntimeData in _workQuotaRuntimeList)
        {
            workQuotaRuntimeData.IncreaseCurrentWorkProgress(GetWorkQuotaValue());
        }
        healthMultiplierData.DecreaseValue(_healthDeductionDuringWork);
        sanityMultiplierData.DecreaseValue(_sanityDeductionDuringWork);
    }



    private int GetWorkQuotaValue()
    {
        return (int)Mathf.Round(_baseWorkPoints * healthMultiplierData.GetWorkPointsMultiplier() * sanityMultiplierData.GetWorkPointsMultiplier());
    }

    private void HandlingOnEveningActivitiesStatsChange(bool workingOut, bool hangingOut)
    {
        if (workingOut)
        {
            healthMultiplierData.DecreaseValue(_healthAdditionDuringWorkout);
        }
        else if (hangingOut)
        {
            sanityMultiplierData.DecreaseValue(_sanityAdditionDuringHangout);
        }
        _statsRuntimeDataList[0].DecreaseValue(_moneyDeductionDuringEveningActivities);
    }

    private void HandlingOnRestingStatsChange(bool havingLunchBreak, bool sleeping)
    {
        if (havingLunchBreak)
        {
            healthMultiplierData.GainValue(_healthAdditionDuringSleep);
            sanityMultiplierData.GainValue(_sanityAdditionDuringSleep);
        }
        else if (sleeping)
        {
            healthMultiplierData.GainValue(_healthAdditionDuringLunchBreak);
            _statsRuntimeDataList[2].GainValue(_sanityAdditionDuringLunchBreak);
        }
    }

    //  QTE related methods
    private void EnablingQTE(bool flag)
    {
        _QTEAvailable = flag;
        InitialiseQTEData();
        GenerateSequenceQTECombination();
    }

    private void DisablingQTE(bool flag)
    {
        _QTEAvailable = flag;

        bool cameToWorkOnTime = CheckSequenceQTEInputs();
        if (!cameToWorkOnTime)
        {
            _statsRuntimeDataList[0].DecreaseValue(_moneyPenalty);
            _statsRuntimeDataList[2].DecreaseValue(_sanityPenalty);
            //  trigger event - UI boss scolds
        }
    }

    private void InitialiseQTEData()
    {
        if (_currentQTEIndex != 0)
        {
            _currentQTEIndex = 0;
        }
        if (_QTEGeneratedInputCombination.Count > 0)
        {
            _QTEGeneratedInputCombination.Clear();
        }

        if (_QTEUserInputCombination.Count > 0)
        {
            _QTEUserInputCombination.Clear();
        }
    }

    private void GenerateSequenceQTECombination()
    {
        for (int i = 0; i < _QTEInputNum; i++)
        {
            _QTEGeneratedInputCombination.Add(_QTECharArr[UnityEngine.Random.Range(0, _QTECharArr.Length - 1)]);
        }

        //  trigger UI event - spawn UI boxes template (container with text: char value)
    }

    private void HandleSequenceQTEInput(char input)
    {
        if (_QTEAvailable && _currentQTEIndex < _QTEGeneratedInputCombination.Count)
        {
            if (input == _QTEGeneratedInputCombination[_currentQTEIndex])
            {
                _QTEUserInputCombination.Add(input);
                _currentQTEIndex++;
                //  trigger UI event - Change color of current UI box to green then destroy it after 0.5s
            }
            else
            {
                //  trigger UI event - Change image color of UI box to red
            }
        }
    }

    private bool CheckSequenceQTEInputs()
    {
        return (_QTEUserInputCombination.Count == _QTEGeneratedInputCombination.Count) ? true : false;
    }

    //  Work Quota related methods
    private void CheckWorkQuota()
    {
        CheckDailyWorkQuota();
        CheckWeeklyWorkQuota();
    }

    private void CheckDailyWorkQuota()
    {
        if (_workQuotaRuntimeList[0].GetCurrentWorkProgress() >= _workQuotaRuntimeList[0].GetCurrentWorkQuota())
        {
            _statsRuntimeDataList[0].GainValue(_workQuotaRuntimeList[0].GetMoneyPoints());
            _statsRuntimeDataList[2].GainValue(_workQuotaRuntimeList[0].GetSanityAdditionUponQuotaMet());
            _workQuotaRuntimeList[0].TaskCompleted();
        }
        else
        {
            _statsRuntimeDataList[0].GainValue((int)Mathf.Round(_workQuotaRuntimeList[0].GetMoneyPoints() * ((float)_workQuotaRuntimeList[0].GetCurrentWorkProgress() / _workQuotaRuntimeList[0].GetCurrentWorkQuota())));
            _statsRuntimeDataList[2].DecreaseValue(_workQuotaRuntimeList[0].GetSanityDeductionUponQuotaFailed());

        }
    }

    private void CheckWeeklyWorkQuota()
    {
        if (!IsEndOfTheWeek)
            return;

        if (_workQuotaRuntimeList[1].GetCurrentWorkProgress() >= _workQuotaRuntimeList[1].GetCurrentWorkQuota())
        {
            _statsRuntimeDataList[0].GainValue(_workQuotaRuntimeList[1].GetMoneyPoints());
            _statsRuntimeDataList[2].GainValue(_workQuotaRuntimeList[1].GetSanityAdditionUponQuotaMet());
            _workQuotaRuntimeList[0].TaskCompleted();
        }
        else
        {
            _statsRuntimeDataList[0].GainValue((int)Mathf.Round(_workQuotaRuntimeList[1].GetMoneyPoints() * ((float)_workQuotaRuntimeList[1].GetCurrentWorkProgress() / _workQuotaRuntimeList[0].GetCurrentWorkQuota())));
            _statsRuntimeDataList[2].DecreaseValue(_workQuotaRuntimeList[1].GetSanityDeductionUponQuotaFailed());
        }
    }

    private bool IsEndOfTheWeek => _gameManager.GetCurrentDayNum() % 7 == 0;

    private void InitialiseWorkQuota(bool firstDay)
    {
        if (firstDay)
            return;

        if (_workQuotaRuntimeList[0].GetQuotaMet())
        {
            _workQuotaRuntimeList[0].IncreaseWorkQuota();
        }

        _workQuotaRuntimeList[0].InitialiseWorkProgress();

        if (!IsEndOfTheWeek)

            return;
        if (_workQuotaRuntimeList[1].GetQuotaMet())
        {
            _workQuotaRuntimeList[1].IncreaseWorkQuota();
        }
        _workQuotaRuntimeList[1].InitialiseWorkProgress();
    }
}
