using UnityEngine;

public class WorkQuotaRuntime
{
    WorkQuotaData _data;

    protected int _currentWorkProgress, _currentWorkQuota, _workQuotaIncrementalValue, _moneyPoints, _sanityAddition, _sanityDeduction;
    protected bool _quotaMet;

    public WorkQuotaRuntime(WorkQuotaData data)
    {
        _data = data;
        _currentWorkProgress = 0;
        _currentWorkQuota = data.GetBaseWorkQuota();
        _workQuotaIncrementalValue = data.GetWorkQuotaIncrementalValue();
        _moneyPoints = data.GetMoneyPoints();
        _sanityAddition = data.GetSanityAddition();
        _sanityDeduction = data.GetSanityDeduction();
        _quotaMet = false;
    }

    //  Getters
    public int GetCurrentWorkProgress() => _currentWorkProgress;

    public int GetCurrentWorkQuota() => _currentWorkQuota;

    public int GetMoneyPoints() => _moneyPoints;

    public int GetSanityAdditionUponQuotaMet() => _sanityAddition;

    public int GetSanityDeductionUponQuotaFailed() => _sanityDeduction;

    public bool GetQuotaMet() => _quotaMet;

    //  Setters

    public void TaskCompleted() { 
        _quotaMet = true;
    }

    public void InitialiseWorkProgress() {
        _currentWorkProgress = 0;
        _quotaMet = false;
    }

    //  Public methods
    public void IncreaseCurrentWorkProgress(int value)
    {
        _currentWorkProgress += value;
    }

    public void IncreaseWorkQuota() {
        _currentWorkQuota += _workQuotaIncrementalValue;
    }
}
