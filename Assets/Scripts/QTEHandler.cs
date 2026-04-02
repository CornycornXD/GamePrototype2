using System;
using System.Collections.Generic;
using UnityEngine;

public class QTEHandler : MonoBehaviour
{
    [SerializeField] int _QTEInputNum;
    private char[] _QTECharArr = { 'W', 'A', 'S', 'D' };
    private List<char> _QTEGeneratedInputCombination = new List<char>();
    private List<char> _QTEUserInputCombination = new List<char>();
    private int _currentQTEIndex;
    private bool _QTEAvailable;

    public static event Action OnWakingUp;
    public static event Action OnCameToWorkLate;
    public static event Action<List<char>> OnSequenceQTECombinationGenerated;
    public static event Action<bool, int> OnQTEInputEntered;

    private void OnEnable()
    {
        InputHandler.WASDEntered += HandleSequenceQTEInput;

        GameManager.OnMorning += HandlingOnMorning;
        GameManager.OnWorkingBeforeNoon += HandlingOnWorkingBeforeNoon;
    }

    private void OnDisable()
    {
        InputHandler.WASDEntered -= HandleSequenceQTEInput;

        GameManager.OnMorning -= HandlingOnMorning;
        GameManager.OnWorkingBeforeNoon -= HandlingOnWorkingBeforeNoon;
    }

    private void HandlingOnMorning(bool flag)
    {
        EnablingQTE(flag);
    }

    private void HandlingOnWorkingBeforeNoon(bool flag, bool firstDay)
    {
        DisablingQTE(flag);
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
            OnCameToWorkLate?.Invoke();
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
        OnSequenceQTECombinationGenerated?.Invoke(_QTEGeneratedInputCombination);
    }

    private void HandleSequenceQTEInput(char input)
    {
        if (_QTEAvailable && _currentQTEIndex < _QTEGeneratedInputCombination.Count)
        {
            OnWakingUp?.Invoke();

            bool correctInputEntered = input == _QTEGeneratedInputCombination[_currentQTEIndex];

            //  trigger UI event - Change color of current UI box to green then destroy it after 0.5s - true
            //  trigger UI event - Change image color of UI box to red - false

            OnQTEInputEntered?.Invoke(correctInputEntered, _currentQTEIndex);

            if (correctInputEntered)
            {
                _QTEUserInputCombination.Add(input);
                _currentQTEIndex++;
            }
        }
    }

    private bool CheckSequenceQTEInputs()
    {
        return _QTEUserInputCombination.Count == _QTEGeneratedInputCombination.Count;
    }
}
