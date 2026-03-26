using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float _morningDuration, _workingDurationBeforeNoon, _lunchBreakDuration, _workingDurationAfternoon, _eveningDuration, _nightDuration, _timer;

    [SerializeField] private DataManager _dataManager;

    private void Start()
    {
        _timer = 0f;

        //  Initialising time periods
        float dayDuration = _dataManager.GetDayDuration();
        _morningDuration = dayDuration / 24 * 2;
        _workingDurationBeforeNoon = dayDuration / 24 * 4;
        _lunchBreakDuration = dayDuration / 24;
        _workingDurationAfternoon = dayDuration / 24 * 4;
        _eveningDuration = dayDuration / 24 * 4;
        _nightDuration = dayDuration / 24 * 9; 
    }

    private void Update()
    { 
        _timer += Time.deltaTime;
    }
}
