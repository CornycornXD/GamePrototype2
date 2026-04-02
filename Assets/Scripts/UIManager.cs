using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Time period data")]
    [SerializeField] private TimePeriodUIData _morningUIData;
    [SerializeField] private TimePeriodUIData _workBeforeNoonUIData;
    [SerializeField] private TimePeriodUIData _lunchBreakUIData;
    [SerializeField] private TimePeriodUIData _workAfterNoonUIData;
    [SerializeField] private TimePeriodUIData _eveningUIData;
    [SerializeField] private TimePeriodUIData _nightUIData;

    //  Top panel
    [Header("Top panel")]
    [SerializeField] private TextMeshProUGUI _dayNumberText;
    [SerializeField] private TextMeshProUGUI _currentTimeText;

    //  Left panel
    [Header("Left panel")]

    //  Work Quotas
    [SerializeField] private TextMeshProUGUI _dailyWorkQuotaCurrentValueText;
    [SerializeField] private TextMeshProUGUI _weeklyWorkQuotaCurrentValueText;
    [SerializeField] private Slider _dailyWorkQuotaValueProgressTrackerBar;
    [SerializeField] private Slider _weeklyWorkQuotaValueProgressTrackerBar;

    //  Stats
    [SerializeField] private TextMeshProUGUI _moneyCurrentValueText;
    [SerializeField] private TextMeshProUGUI _healthCurrentValueText;
    [SerializeField] private TextMeshProUGUI _sanityCurrentValueText;
    [SerializeField] private Image _moneyStatIcon;
    [SerializeField] private Image _healthStatIcon;
    [SerializeField] private Image _sanityStatIcon;
    [SerializeField] private Slider _moneyValueTrackerBar;
    [SerializeField] private Slider _healthValueTrackerBar;
    [SerializeField] private Slider _sanityValueTrackerBar;


    //  Right panel
    [Header("Right panel")]
    [SerializeField] private TextMeshProUGUI _timePeriodTitleText;
    [SerializeField] private TextMeshProUGUI _timeRangeText;
    [SerializeField] private Image _timePeriodIcon;
    [SerializeField] private Image _circularTrackerImage;

    //  QTEPrefabs
    [Header("QTE Prefabs")]
    [SerializeField] private GameObject _QTECharContainerPrefab;

    //  Others
    private List<GameObject> _QTEPrefabsList = new List<GameObject>();

    private void OnEnable()
    {
        DataManager.OnMoneyChanged += HandleOnMoneyChanged;
        DataManager.OnHealthChanged += HandleOnHealthChanged;
        DataManager.OnSanityChanged += HandleOnSanityChanged;
        DataManager.OnDailyWorkQuotaProgressChanged += HandleOnDailyWorkQuotaProgressChanged;
        DataManager.OnWeeklyWorkQuotaProgressChanged += HandleOnWeeklyWorkQuotaProgressChanged;
        DataManager.OnGameEnd += HandleOnGameEnd;
         
        QTEHandler.OnSequenceQTECombinationGenerated += HandleOnSequenceQTECombinationGenerated;
        QTEHandler.OnQTEInputEntered += HandleOnQTEInputEntered;
        QTEHandler.OnCameToWorkLate += HandleOnCameToWorkLate;

    }

    private void OnDisable()
    {
        DataManager.OnMoneyChanged -= HandleOnMoneyChanged;
        DataManager.OnHealthChanged -= HandleOnHealthChanged;
        DataManager.OnSanityChanged -= HandleOnSanityChanged;
        DataManager.OnDailyWorkQuotaProgressChanged -= HandleOnDailyWorkQuotaProgressChanged;
        DataManager.OnWeeklyWorkQuotaProgressChanged -= HandleOnWeeklyWorkQuotaProgressChanged;
        DataManager.OnGameEnd -= HandleOnGameEnd;

        QTEHandler.OnSequenceQTECombinationGenerated -= HandleOnSequenceQTECombinationGenerated;
        QTEHandler.OnQTEInputEntered -= HandleOnQTEInputEntered;
        QTEHandler.OnCameToWorkLate -= HandleOnCameToWorkLate;
    }

    private void HandleOnMoneyChanged(int currentValue, int maxValue) {
        _moneyCurrentValueText.text = $"{currentValue} / {maxValue}";

        _moneyValueTrackerBar.value = currentValue / maxValue;
    }

    private void HandleOnHealthChanged(int currentValue, int maxValue)
    {
        _healthCurrentValueText.text = $"{currentValue} / {maxValue}";

        _healthValueTrackerBar.value = currentValue / maxValue;
    }

    private void HandleOnSanityChanged(int currentValue, int maxValue)
    {
        _sanityCurrentValueText.text = $"{currentValue} / {maxValue}";

        _sanityValueTrackerBar.value = currentValue / maxValue;
    }

    private void HandleOnWorkProgressChanged(int currentDailyWorkProgress, int currentDailyWorkQuota, int currentWeeklyWorkProgress, int currentWeeklyWorkQuota) {
        HandleOnDailyWorkQuotaProgressChanged(currentDailyWorkProgress, currentDailyWorkQuota);
        HandleOnWeeklyWorkQuotaProgressChanged(currentWeeklyWorkProgress, currentWeeklyWorkQuota);
    }

    private void HandleOnDailyWorkQuotaProgressChanged(int currentWorkProgress, int currentWorkQuota) {
        _dailyWorkQuotaCurrentValueText.text = $"{currentWorkProgress} / {currentWorkQuota}";
        _weeklyWorkQuotaCurrentValueText.text = $"{currentWorkProgress} / {currentWorkQuota}";
    }

    private void HandleOnWeeklyWorkQuotaProgressChanged(int currentWorkProgress, int currentWorkQuota) {
        _dailyWorkQuotaValueProgressTrackerBar.value = currentWorkProgress / currentWorkQuota;
        _weeklyWorkQuotaValueProgressTrackerBar.value = currentWorkProgress / currentWorkQuota;
    }

    private void HandleOnSequenceQTECombinationGenerated(List<char> QTEGeneratedCharCombination) { 
        
    }

    private void HandleOnQTEInputEntered(bool correctInputEntered, int QTEContainerIndex) { 
        
    }

    private void HandleOnCameToWorkLate()
    {

    }

    //  Adjust onGameEnd later
    private void HandleOnGameEnd(bool gameCompleted, int currentMoneyValue, int currentHealthValue, int currentSanityValue) { 
        
    }
}
