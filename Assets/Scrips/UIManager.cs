using Scrips.EventManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private static readonly int MAXLENGTHTIMESTEPSTRING = 6;
    
    public Button autoTimerButton;
    public Slider speedSlider;
    public TMP_InputField speedInputField;
    private bool _autoPlayMode;
    
    public Button nextTimeStepButton;

    public TMP_Text timeStepCounterText;
    
    private TimeManager _timeManager;
    
// Start is called before the first frame update
    void Start() {
        _timeManager = GameObject.Find("Simulation Time Manager").GetComponent<TimeManager>();
        
        autoTimerButton.onClick.AddListener(OnAutoTimerButtonClick);
        speedSlider.onValueChanged.AddListener(OnSpeedSliderValueChange);
        speedSlider.minValue = 1;
        speedSlider.maxValue = 20;
        
        nextTimeStepButton.onClick.AddListener(OnNextTimeStepButtonClick);

        TimeEventManager.current.OnTick += OnTick;
        
        timeStepCounterText.text = GetTimeStepString(0);
    }

    private void OnAutoTimerButtonClick() { 
        TextMeshProUGUI autoTimerButtonText = autoTimerButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
        _autoPlayMode = !_autoPlayMode;

        autoTimerButtonText.text = _autoPlayMode ? "Stop" : "Start";
        nextTimeStepButton.interactable = !_autoPlayMode;
        
        _timeManager.SetAutoplayMode(_autoPlayMode);
    }

    private void OnSpeedSliderValueChange(float value) {
        speedInputField.text = value.ToString();
        
        // Set the timing between two ticks to 1/value (the value is given as 1 to 10 steps per seconds so to
        // account for that we have to passe the ivners to the time manager)
        _timeManager.SetInterval((1/value));
    }

    private void OnNextTimeStepButtonClick() {
        _timeManager.Tick();
    }

    private void OnTick(int timeStep) {
        timeStepCounterText.text = GetTimeStepString(timeStep);
    }

    private string GetTimeStepString(int timeStep) {
        string timeStepAsString = timeStep.ToString();
        string timeStepCounterString = "Time-Step: ";

        if (timeStepAsString.Length < MAXLENGTHTIMESTEPSTRING) {
            for (int i = 0; i < MAXLENGTHTIMESTEPSTRING-timeStepAsString.Length; i++) {
                timeStepCounterString += " ";
            }   
        }

        timeStepCounterString += timeStepAsString;
        return timeStepCounterString;
    }
}
