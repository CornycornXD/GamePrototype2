using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DataManager : MonoBehaviour
{
    //  General set up stats
    [Header("Set up")]
    [SerializeField] private int _dayNum;
    [SerializeField] private float _dayDuration;
    [SerializeField] private List<StatParentData> _statsBaseDataList;
    private List<StatParentRuntime> _statsRuntimeDataList = new List<StatParentRuntime>();

    //  QTE
    [Header("QTE")]
    [SerializeField] int _QTEInputNum;
    private char[] _QTECharArr = {'W', 'A', 'S', 'D'};
    private List<char> _QTEGeneratedInputCombination = new List<char>();
    private List<char> _QTEUserInputCombination = new List<char>();
    private int _currentQTEIndex;
    private bool _QTEAvailable;

    //  'Arrived late' penalties
    [Header("\'Arrived late\' penalties")]
    [SerializeField] private int _moneyPenalty;
    [SerializeField] private int _sanityPenalty;

    //  Work quotas
    [Header("Work quotas\' related stats")]
    [SerializeField] private List<WorkQuotaData> _workQuotaDataList;
    private List<WorkQuotaRuntime> _workQuotaRuntimeList = new List<WorkQuotaRuntime>();

    [Header("Working-related stats")]
    [SerializeField] private int _baseWorkPoints;
    [SerializeField] private int _healthDeductionDuringWork;
    [SerializeField] private int _sanityDeductionDuringWork;

    [Header("Evening activities-related stats")]
    [SerializeField] private int _healthAdditionDuringWorkout;
    [SerializeField] private int _sanityAdditionDuringHangout;
    [SerializeField] private int _moneyDeductionDuringEveningActivities;

    [Header("Resting-related stats")]
    [SerializeField] private int _healthAdditionDuringSleep;
    [SerializeField] private int _sanityAdditionDuringSleep;
    [SerializeField] private int _healthAdditionDuringLunchBreak;
    [SerializeField] private int _sanityAdditionDuringLunchBreak;

    [Header("Managers")]
    [SerializeField] GameManager _gameManager;

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

        foreach (WorkQuotaData workQuotaBaseData in _workQuotaDataList)
        {
            _workQuotaRuntimeList.Add(new WorkQuotaRuntime(workQuotaBaseData));
        }
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
    private void HandlingOnMorning(bool flag) {
        EnablingQTE(flag);
    }

    private void HandlingOnWorkingBeforeNoon(bool flag, bool firstDay) {
        DisablingQTE(flag);
        if (!firstDay) {
            CheckWorkQuota();
        }
        InitialiseWorkQuota(firstDay);
    }

    //  Stats change handlers
    private void HandlingOnWorkStatsChange()
    {
        foreach (WorkQuotaRuntime workQuotaRuntimeData in _workQuotaRuntimeList)
        {
            workQuotaRuntimeData.IncreaseCurrentWorkProgress((int)Mathf.Round((_baseWorkPoints * (_statsRuntimeDataList[1] as MultiplierRelatedStatsRuntime).GetWorkPointsMultiplier() * (_statsRuntimeDataList[2] as MultiplierRelatedStatsRuntime).GetWorkPointsMultiplier())));
        }
        _statsRuntimeDataList[1].DecreaseValue(_healthDeductionDuringWork);
        _statsRuntimeDataList[2].DecreaseValue(_sanityDeductionDuringWork);
    }

    private void HandlingOnEveningActivitiesStatsChange(bool workingOut, bool hangingOut) {
        if (workingOut)
        {
            _statsRuntimeDataList[1].DecreaseValue(_healthAdditionDuringWorkout);
        }
        else if (hangingOut) {
            _statsRuntimeDataList[2].DecreaseValue(_sanityAdditionDuringHangout);
        }
        _statsRuntimeDataList[0].DecreaseValue(_moneyDeductionDuringEveningActivities);
    }

    private void HandlingOnRestingStatsChange(bool havingLunchBreak, bool sleeping)
    {
        if (havingLunchBreak)
        {
            _statsRuntimeDataList[1].GainValue(_healthAdditionDuringSleep);
            _statsRuntimeDataList[2].GainValue(_sanityAdditionDuringSleep);
        }
        else if (sleeping)
        {
            _statsRuntimeDataList[1].GainValue(_healthAdditionDuringLunchBreak);
            _statsRuntimeDataList[2].GainValue(_sanityAdditionDuringLunchBreak);
        }
    }

    //  QTE related methods
    private void EnablingQTE(bool flag) { 
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

    private void InitialiseQTEData() {
        if (_currentQTEIndex != 0) {
            _currentQTEIndex = 0;        
        }
        if (_QTEGeneratedInputCombination.Count > 0)
        {
            _QTEGeneratedInputCombination.Clear();
        }

        if (_QTEUserInputCombination.Count > 0) { 
            _QTEUserInputCombination.Clear();
        }
    }

    private void GenerateSequenceQTECombination()
    {
        for (int i = 0; i < _QTEInputNum; i++) {
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
            else { 
                //  trigger UI event - Change image color of UI box to red
            }
        }
    }

    private bool CheckSequenceQTEInputs()
    {
        return (_QTEUserInputCombination.Count == _QTEGeneratedInputCombination.Count) ? true : false;
    }

    //  Work Quota related methods
    private void CheckWorkQuota() {
        if (_workQuotaRuntimeList[0].GetCurrentWorkProgress() >= _workQuotaRuntimeList[0].GetCurrentWorkQuota())
        {
            _statsRuntimeDataList[0].GainValue(_workQuotaRuntimeList[0].GetMoneyPoints());
            _statsRuntimeDataList[2].GainValue(_workQuotaRuntimeList[0].GetSanityAdditionUponQuotaMet());
            _workQuotaRuntimeList[0].TaskCompleted();
        }
        else
        {
            _statsRuntimeDataList[0].GainValue((int) Mathf.Round((_workQuotaRuntimeList[0].GetMoneyPoints() * ((float)_workQuotaRuntimeList[0].GetCurrentWorkProgress() / _workQuotaRuntimeList[0].GetCurrentWorkQuota()))));
            _statsRuntimeDataList[2].DecreaseValue(_workQuotaRuntimeList[0].GetSanityDeductionUponQuotaFailed());

        }
        if (_gameManager.GetCurrentDayNum() % 7 == 0) {
            if (_workQuotaRuntimeList[1].GetCurrentWorkProgress() >= _workQuotaRuntimeList[1].GetCurrentWorkQuota()) {
                _statsRuntimeDataList[0].GainValue(_workQuotaRuntimeList[1].GetMoneyPoints());
                _statsRuntimeDataList[2].GainValue(_workQuotaRuntimeList[1].GetSanityAdditionUponQuotaMet());
                _workQuotaRuntimeList[0].TaskCompleted();
            }
            else {
                _statsRuntimeDataList[0].GainValue((int)Mathf.Round((_workQuotaRuntimeList[1].GetMoneyPoints() * ((float)_workQuotaRuntimeList[1].GetCurrentWorkProgress() / _workQuotaRuntimeList[0].GetCurrentWorkQuota()))));
                _statsRuntimeDataList[2].DecreaseValue(_workQuotaRuntimeList[1].GetSanityDeductionUponQuotaFailed());
            }
        }
    }

    private void InitialiseWorkQuota(bool firstDay) {
        if (!firstDay) {
            if (_workQuotaRuntimeList[0].GetQuotaMet())
            {
                _workQuotaRuntimeList[0].IncreaseWorkQuota();
            }
            _workQuotaRuntimeList[0].InitialiseWorkProgress();
            if (_gameManager.GetCurrentDayNum() % 7 == 0)
            {
                if (_workQuotaRuntimeList[1].GetQuotaMet())
                {
                    _workQuotaRuntimeList[1].IncreaseWorkQuota();
                }
                _workQuotaRuntimeList[1].InitialiseWorkProgress();
            }
        }
    }


}
