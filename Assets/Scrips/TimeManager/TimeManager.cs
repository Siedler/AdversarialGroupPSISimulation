using System;
using Scrips.EventManager;
using Scrips.TimeManager;
using UnityEngine;

public class TimeManager : MonoBehaviour {
    public static TimeManager current;
    
    private TimeManagerStates _state;

    private int _timeStep;
    private int _goalTimeStep;
    
    private double _interval = 1; 
    private double _nextTime = 0;

    private Environment _environment;

    private void Awake() {
        current = this;
    }

    // Start is called before the first frame update
    void Start() {
        _environment = GameObject.Find("World").GetComponent<Environment>();
        _environment.Initialize();

        SetState(TimeManagerStates.Manual);

        _timeStep = 0;
    }
    
    // Update is called once per frame
    void Update () {
        if (_state == TimeManagerStates.Manual) return;
        
        // If autoplay is on:
        if (_state == TimeManagerStates.AutomaticToTimeStep && _goalTimeStep <= _timeStep) {
            SetState(TimeManagerStates.Manual);
            return;
        }
        
        if (_state == TimeManagerStates.AutomaticTimeBased && Time.time < _nextTime) return;
        
        Tick();
        
        if(_state == TimeManagerStates.AutomaticTimeBased) _nextTime += _interval;
    }

    public void Tick() {
        _timeStep++;
        
        _environment.Tick();
        
        TimeEventManager.current.Tick(_timeStep);
    }

    public void SetAutoplayMode(TimeManagerStates state, int goalTimeStep = -1) {
        if (state == TimeManagerStates.AutomaticToTimeStep) {
            if (goalTimeStep <= _timeStep) {
                SetState(TimeManagerStates.Manual);
                return;
            }
            
            // if goalTimeStep is valid:
            _goalTimeStep = goalTimeStep;
        }
        
        if (state == TimeManagerStates.AutomaticTimeBased) _nextTime = Time.time;
        
        SetState(state);
    }

    public void SetInterval(float interval) {
        _interval = interval;
    }

    public TimeManagerStates GetState() {
        return _state;
    }

    public event Action<TimeManagerStates> OnTimeManagerStateChange;
    private void SetState(TimeManagerStates state) {
        _state = state;
        OnTimeManagerStateChange?.Invoke(_state);
    }

    public int GetCurrentTimeStep() {
        return _timeStep;
    }
}
