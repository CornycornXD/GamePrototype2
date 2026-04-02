using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    //  Top panel
    [Header("Top panel")]
    [SerializeField] private TextMeshProUGUI _dayNumberText;
    [SerializeField] private TextMeshProUGUI _currentTimeText;

    //  Right panel
    [Header("Right panel")]
    [SerializeField] private TextMeshProUGUI _timePeriodTitleText;
    [SerializeField] private TextMeshProUGUI _timeRangeText;
    [SerializeField] private Image _timePeriodIcon;
    [SerializeField] private Image _circularTrackerImage;

    //  Left panel
    [Header("Left panel")]

    //  Work Quotas
    [SerializeField] private TextMeshProUGUI _dailyWorkiQuotaCurrentValueText;
    [SerializeField] private TextMeshProUGUI _weeklyWorkiQuotaCurrentValueText;
    [SerializeField] private Slider _dailyWorkQuotaValueProgressTrackerBar;
    [SerializeField] private Slider _weeklyWorkQuotaValueProgressTrackerBar;


    //  Stats
    [SerializeField] private TextMeshProUGUI _moneyCurrentValueText;
    [SerializeField] private TextMeshProUGUI _healthCurrentValueText;
    [SerializeField] private TextMeshProUGUI _sanityCurrentValueText;
    [SerializeField] private Image _moneyStatIcon;
    [SerializeField] private Image _healthStatIcon;
    [SerializeField] private Image _sanityStatIcon;

    //  QTEPrefabs
    [Header("QTE Prefabs")]
    [SerializeField] private GameObject _QTECharContainerPrefab;

    //  Others
    private List<GameObject> _QTEPrefabsList = new List<GameObject>();

    private void OnEnable()
    {
        DataManager.OnStatsChanged += HandleOnStatsChanged;
        DataManager.OnWorkProgressChanged += HandleOnWorkProgressChanged;
        DataManager.OnWorkQuotaChanged += HandleOnWorkQuotaChanged;
         
        QTEHandler.OnSequenceQTECombinationGenerated += HandleOnSequenceQTECombinationGenerated;
        QTEHandler.OnQTEInputEntered += HandleOnQTEInputEntered;
        QTEHandler.OnCameToWorkLate += HandleOnCameToWorkLate;

    }

    private void OnDisable()
    {
        DataManager.OnStatsChanged -= HandleOnStatsChanged;
        DataManager.OnWorkProgressChanged -= HandleOnWorkProgressChanged;
        DataManager.OnWorkQuotaChanged -= HandleOnWorkQuotaChanged;

        QTEHandler.OnSequenceQTECombinationGenerated -= HandleOnSequenceQTECombinationGenerated;
        QTEHandler.OnQTEInputEntered -= HandleOnQTEInputEntered;
        QTEHandler.OnCameToWorkLate -= HandleOnCameToWorkLate;
    }

    private void HandleOnStatsChanged() {
         
    }

    private void HandleOnWorkProgressChanged() { 
        
    }

    private void HandleOnWorkQuotaChanged() { 
        
    }

    private void HandleOnSequenceQTECombinationGenerated() { 
        
    }

    private void HandleOnQTEInputEntered() { 
        
    }

    private void HandleOnCameToWorkLate()
    {

    }
}
