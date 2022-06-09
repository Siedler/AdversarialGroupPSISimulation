using System;
using Scrips.EventManager;
using Scrips.TimeManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private static readonly int MAXLENGTHTIMESTEPSTRING = 6;
    
    public Button autoTimerButton;
    public Slider speedSlider;
    public TMP_InputField speedInputField;

    public Button nextTimeStepButton;

    public TMP_Text timeStepCounterText;

    private bool _showDialogue;
    public GameObject skipToDialogue;
    private Button _skipToDialogueButton;
    private TMP_InputField _skipToDialogueInputField;

// Start is called before the first frame update
    void Start() {
        TimeManager.current.OnTimeManagerStateChange += OnTimeManagerStateChange;
        TimeEventManager.current.OnTick += OnTick;

        autoTimerButton.onClick.AddListener(OnAutoTimerButtonClick);
        speedSlider.onValueChanged.AddListener(OnSpeedSliderValueChange);
        speedSlider.minValue = 1;
        speedSlider.maxValue = 20;
        
        nextTimeStepButton.onClick.AddListener(OnNextTimeStepButtonClick);

        timeStepCounterText.text = GetTimeStepString(0);

        _skipToDialogueButton = skipToDialogue.transform.Find("Skip To Button").GetComponent<Button>();
        _skipToDialogueButton.onClick.AddListener(OnSkipToButtonClick);
        
        _skipToDialogueInputField = skipToDialogue.transform.Find("Skip To Input").GetComponent<TMP_InputField>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            if (TimeManager.current.GetState() == TimeManagerStates.Manual) {
                _showDialogue = !_showDialogue;
                skipToDialogue.SetActive(_showDialogue);   
            }
        }
    }

    private void OnAutoTimerButtonClick() {
        TimeManagerStates currentState = TimeManager.current.GetState();
        if (currentState == TimeManagerStates.Manual) {
            TimeManager.current.SetAutoplayMode(TimeManagerStates.AutomaticTimeBased);
            return;
        }
        
        TimeManager.current.SetAutoplayMode(TimeManagerStates.Manual);
    }

    private void OnSpeedSliderValueChange(float value) {
        speedInputField.text = value.ToString();
        
        // Set the timing between two ticks to 1/value (the value is given as 1 to 10 steps per seconds so to
        // account for that we have to passe the ivners to the time manager)
        TimeManager.current.SetInterval(1/value);
    }

    private void OnNextTimeStepButtonClick() {
        TimeManager.current.Tick();
    }

    private void OnSkipToButtonClick() {
        int timeStepToSkipTo = Int32.Parse(_skipToDialogueInputField.text);

        if (timeStepToSkipTo < TimeManager.current.GetCurrentTimeStep()) return;
        
        _showDialogue = !_showDialogue;
        skipToDialogue.SetActive(_showDialogue);
        TimeManager.current.SetAutoplayMode(TimeManagerStates.AutomaticToTimeStep, timeStepToSkipTo);
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

    private void OnTimeManagerStateChange(TimeManagerStates state) {
        TextMeshProUGUI autoTimerButtonText = autoTimerButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        Debug.Log("STATECHANGE" + state);
        switch (state) {
            case TimeManagerStates.Manual:
                autoTimerButtonText.text = "Start";
                nextTimeStepButton.interactable = true;
                break;
            case TimeManagerStates.AutomaticTimeBased:
            case TimeManagerStates.AutomaticToTimeStep:
                autoTimerButtonText.text = "Stop";
                nextTimeStepButton.interactable = false;
                break;
        }
    }
}
