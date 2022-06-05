using System.Collections;
using System.Collections.Generic;
using Scrips.EventManager;
using UnityEngine;
using UnityEngine.Video;

public class TimeManager : MonoBehaviour {
    private bool _autoPlayMode;
    
    private double _interval = 1; 
    private double _nextTime = 0;

    private Environment _environment;
    
    // Start is called before the first frame update
    void Start() {
        _environment = GameObject.Find("World").GetComponent<Environment>();
        _environment.Initialize();
    }
    
    // Update is called once per frame
    void Update () {
        if (!_autoPlayMode) return;
        
        // If autoplay is on:
        if (Time.time < _nextTime) return;
    
        Tick();
        _nextTime += _interval;
    }

    public void Tick() {
        _environment.Tick();
        
        TimeEventManager.current.Tick();
    }

    public void SetAutoplayMode(bool mode) {
        _autoPlayMode = mode;

        if (_autoPlayMode) _nextTime = Time.time;
    }

    public void SetInterval(float interval) {
        _interval = interval;
    }
}
