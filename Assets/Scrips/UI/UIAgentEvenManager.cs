using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using Scrips.EventManager;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIAgentEvenManager : MonoBehaviour  {
    
    private Agent _selectedAgent = null;

    private ScrollRect _scrollView;
    private TMP_Text _agentEventText;
    
    // Start is called before the first frame update
    void Start() {
        AgentEventManager.current.OnAgentSelected += OnAgentSelected;
        AgentEventManager.current.OnAgentDeselected += OnAgentDeselected;

        TimeEventManager.current.OnTick += Tick;

        _scrollView = this.transform.Find("Scroll View").GetComponent<ScrollRect>();
        _agentEventText = this.transform.Find("Scroll View/Viewport/Content/Event Text").GetComponent<TMP_Text>();
        
        transform.gameObject.SetActive(false);
    }

    public void OnGUI() {
        if (!IsAgentSelected()) return;
        
        if(_agentEventText.text == _selectedAgent.GetEventHistoryString()) return;

        _agentEventText.SetText(_selectedAgent.GetEventHistoryString());
        _scrollView.normalizedPosition = new Vector2(0, 0);
    }

    private void Tick(int _) {
        
    }

    private void OnAgentSelected(Agent agent) {
        _selectedAgent = agent;
        _agentEventText.SetText(_selectedAgent.GetEventHistoryString());
        transform.gameObject.SetActive(true);
    }

    private void OnAgentDeselected() {
        _selectedAgent = null;
        transform.gameObject.SetActive(false);
    }

    private bool IsAgentSelected() {
        return _selectedAgent != null;
    }
}
