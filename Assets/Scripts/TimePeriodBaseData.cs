using UnityEngine;

[CreateAssetMenu(fileName = "TimePeriodUIData", menuName = "Scriptable Objects/TimePeriodUIData")]
public class TimePeriodUIData : ScriptableObject
{
    //  Private fields
    [SerializeField] string _timePeriodName;
    [SerializeField] string _timeRange;
    [SerializeField] Sprite _timePeriodIcon;
    [SerializeField] Color _colourTone;

    //  Getters
    public string GetTimePeriodName() => _timePeriodName;
    public string GetTimeRange() => _timeRange;
    public Sprite GetTimePeriodIcon() => _timePeriodIcon;
    public Color GetColourTone() => _colourTone;
}
