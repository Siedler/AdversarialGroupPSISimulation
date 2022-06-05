using System.Collections;
using System.Collections.Generic;
using Scrips.EventManager;
using Scrips.UI;
using Unity.VisualScripting;
using UnityEngine;

public class UINeedTankGroupController : MonoBehaviour {
    public UINeedTankController[] needTanks = new UINeedTankController[5];

    private Agent _selectedAgent;

    // Start is called before the first frame update
    void Start() {
        AgentEventManager.current.OnAgentSelected += OnAgentSelected;
        AgentEventManager.current.OnAgentDeselected += OnAgentDeselected;

        TimeEventManager.current.OnTick += OnTick;
    }

    private void OnAgentSelected(Agent agent) {
        _selectedAgent = agent;
        
        UpdateUI();
    }

    private void OnAgentDeselected() {
        _selectedAgent = null;
    }

    private bool IsAgentSelected() { return _selectedAgent != null; }

    private void OnTick() {
        UpdateUI();
    }

    private void UpdateUI() {
        if (!IsAgentSelected()) return;

        double[][] agentNeedTankSummary = _selectedAgent.GetNeedTankSummary();
        
        for (int i = 0; i < 5; i++) {
            needTanks[i].SetValues(agentNeedTankSummary[i][0], agentNeedTankSummary[i][1], agentNeedTankSummary[i][2]);
        }
    }
}
