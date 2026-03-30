using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int _currentHour, _currentDayNum;
    private float _dayDuration, _timer, _workingStateTimer, _workingOutStateTimer, _hangingOutStateTimer;

    //  General game set-up flag
    private bool _gameStarted, _firstDay;

    //  Time period flags
    private bool _morning, _workingBeforeNoon, _lunchBreak, _workingAfterNoon, _evening, _night, _working;

    //  Activity state flags
    private bool _workingOut, _hangingOut;

    //  Event flags
    private bool _onMorningTriggered, _onWorkingBeforeNoonTriggered; 

    [SerializeField] private float _firstDayStartingTime;
    
    [SerializeField] private DataManager _dataManager;

    public static event Action<bool> OnMorning;
    public static event Action<bool, bool> OnWorkingBeforeNoon;

    public static event Action<bool, bool, bool> OnWork;
    public static event Action OnWorkStatsChange;
    
    public static event Action<bool, bool> OnEveningActivities;
    public static event Action<bool, bool> OnEveningActivitiesStatsChange;

    public static event Action<bool, bool> OnRestingStatsChange;

    public static event Action<int> OnDayEnd;
    public static event Action<List<StatParentRuntime>> OnGameEnd;

    public int GetCurrentDayNum() => _currentDayNum;

    private void OnEnable()
    {
        InputHandler.OnClickPerformed += HandleClick;
    }

    private void OnDisable()
    {
        InputHandler.OnClickPerformed -= HandleClick;
    }

    private void Start()
    {
        _currentDayNum = 0;
        _dayDuration = _dataManager.GetDayDuration();
        _timer = _firstDayStartingTime;
        Time.timeScale = 0f;

        //  Initialise time period flags
        _morning = false;
        _workingBeforeNoon = false;
        _lunchBreak = false;
        _workingAfterNoon = false;
        _evening = false;
        _night = false;

        //  Initialise event flags
        _onMorningTriggered = false;
        _onWorkingBeforeNoonTriggered = false;
    }

    private void Update()
    {
        if ((_timer >= _dayDuration / 24 * 6 && _timer < _dayDuration / 24 * 8) && !_morning) // Morning
        {
            _night = false;
            _morning = true;
            if (!_onMorningTriggered) {
                _onMorningTriggered = true;
                OnMorning?.Invoke(_morning);
            }
        }
        else if ((_timer >= _dayDuration / 24 * 8 && _timer < _dayDuration / 24 * 12) && !_workingBeforeNoon) // WorkingBeforeNoon
        {
            _morning = false;
            _workingBeforeNoon = true;
            if (!_onWorkingBeforeNoonTriggered)
            {
                _onWorkingBeforeNoonTriggered = true;
                OnWorkingBeforeNoon?.Invoke(_morning, _firstDay);
            }
        }
        else if ((_timer >= _dayDuration / 24 * 12 && _timer < _dayDuration / 24 * 13) && !_lunchBreak) // LunchBreak
        {
            _workingBeforeNoon = false;
            _lunchBreak = true;
        }
        else if ((_timer >= _dayDuration / 24 * 13 && _timer < _dayDuration / 24 * 17) && !_workingAfterNoon) // WorkingAfterNoon
        {
            _lunchBreak = false;
            _workingAfterNoon = true;
        }
        else if ((_timer >= _dayDuration / 24 * 17 && _timer < _dayDuration / 24 * 21) && !_evening) // Evening
        {
            _workingAfterNoon = false;
            _evening = true;
        }
        else if (((_timer >= _dayDuration / 24 * 21 && _timer < _dayDuration / 24 * 24) || (_timer >= _dayDuration / 24 * 0 && _timer < _dayDuration / 24 * 6)) && !_night) // Night
        {
            _evening = false;
            _night = true;
        }

        if (_timer >= _dayDuration) {
            if (_currentDayNum <= _dataManager.GetDayNum())
            {
                if (_firstDay)
                {
                    _firstDay = false;
                }

                _currentDayNum++;
                
                _timer = 0f;

                _onMorningTriggered = false;

                _onWorkingBeforeNoonTriggered = false;
            }
            else {
                Time.timeScale = 0;
                OnGameEnd?.Invoke(_dataManager.GetStatsRuntimeDataList());
            }
        }
        _timer += Time.deltaTime;
    }

    private void HandleClick(int mouseValue) {
        if (mouseValue == 0)
        {
            if (!_gameStarted)
            {
                Time.timeScale = 1f;
                _gameStarted = true;
            }
            else {
                if (!_morning) {
                    if (!_evening)
                    {
                        StartCoroutine(EnableWorkingState());
                    }
                    else
                    {
                        if (_dataManager.GetStatsRuntimeDataList()[0].GetCurrentValue() >= _dataManager.GetMoneyDeductionDuringEveningActivities()) {
                            StartCoroutine(EnableWorkingOutState());
                        }
                    }
                }
            }
        }
        else {
            if (_evening)
            {
                if (_dataManager.GetStatsRuntimeDataList()[0].GetCurrentValue() >= _dataManager.GetMoneyDeductionDuringEveningActivities())
                {
                    StartCoroutine(EnableHangingOutState());
                }
            }
        }
    }

    private IEnumerator EnableWorkingState() {
        _workingStateTimer = 0f;
        OnWorkStatsChange?.Invoke();
        if (!_working) {
            _working = true;
            StartCoroutine(WorkingState());
        }
        _working = true;
        while (_workingStateTimer < 1f) {
            _workingStateTimer += Time.deltaTime;
            yield return null;
        }
        _working = false;
    }

    private IEnumerator EnableWorkingOutState()
    {
        _workingOutStateTimer = 0f;
        if (!_workingOut)
        {
            _workingOut = true;
            StartCoroutine(WorkingOutState());
        }
        while (_workingOutStateTimer < 1f)
        {
            _workingOutStateTimer += Time.deltaTime;
            yield return null;
        }
        _workingOut = false;
    }

    private IEnumerator EnableHangingOutState()
    {
        _hangingOutStateTimer = 0f;
        if (!_hangingOut)
        {
            _hangingOut = true;
            StartCoroutine(HangingOutState());
        }
        while (_hangingOutStateTimer < 1f)
        {
            _hangingOutStateTimer += Time.deltaTime;
            yield return null;
        }
        _hangingOut = false;
    }

    private IEnumerator WorkingState()
    {
        //  Event load working scene (_typing, _duringLunchBreak, _duringNight) - it doesnt matter at the start since the typing scenes for every work-able time periods are the same
        OnWork?.Invoke(_working, _lunchBreak, _night);
        while (_working) {
            yield return null;
        }
        //  Idle scenes are different for (_workingBeforeNoon + _workingAfterNoon), _lunchBreak, and _night
        OnWork?.Invoke(_working, _lunchBreak, _night);

        //  Entering resting state
        if (!_working) {
            StartCoroutine(RestingState(_lunchBreak, _night));
        }
    }

    //  fix this sht
    private IEnumerator RestingState(bool havingLunchBreak, bool sleeping) {
        float passivePointsTimer = 0f;
        while (!_working )
        {
            if (passivePointsTimer < 1)
            {
                passivePointsTimer += Time.deltaTime;
                yield return null;
            }
            else
            {
                passivePointsTimer = 0f;
                OnRestingStatsChange?.Invoke(havingLunchBreak, sleeping); 
            }
        }
    }

    private IEnumerator WorkingOutState()
    {
        //  event
        OnEveningActivities?.Invoke(_workingOut, false);
        float passivePointsTimer = 0f;
        while (_workingOut && !_hangingOut) {
            if (passivePointsTimer < 1) {
                passivePointsTimer += Time.deltaTime;
                yield return null;
            } else {
                passivePointsTimer = 0f;
                OnEveningActivitiesStatsChange?.Invoke(_workingOut, _hangingOut);
            }
        }

        //  Allow for scene and timer interruption
        if (_hangingOut) {
            _workingOut = false;
        }

        // another event to switch to idle scene
        if (!_workingOut && !_hangingOut) {
            OnEveningActivities?.Invoke(false, false);
        }
    }

    private IEnumerator HangingOutState() {
        //  event
        OnEveningActivities?.Invoke(false, _hangingOut);
        float passivePointsTimer = 0f;
        while (_hangingOut && !_workingOut)
        {
            if (passivePointsTimer < 1)
            {
                passivePointsTimer += Time.deltaTime;
                yield return null;
            }
            else
            {
                passivePointsTimer = 0f;
                OnEveningActivitiesStatsChange?.Invoke(_workingOut, _hangingOut);
            }
        }

        //  Allow for scene and timer interruption
        if (_workingOut)
        {
            _hangingOut = false;
        }

        // another event to switch to idle scene
        if (!_workingOut && !_hangingOut)
        {
            OnEveningActivities?.Invoke(false, false);
        }
    }
}
