using Scrips.EventManager;
using TMPro;
using UnityEngine;

public class UIGeneralAgentInfo : MonoBehaviour {
    
    private Agent _selectedAgent;

    private TMP_Text _agentNameText;
    private TMP_Text _agentHealthText;
    
    // Start is called before the first frame update
    void Start() {
        AgentEventManager.current.OnAgentSelected += OnAgentSelected;
        AgentEventManager.current.OnAgentDeselected += OnAgentDeselected;

        TimeEventManager.current.OnTick += OnTick;

        _agentNameText = transform.GetChild(0).GetComponent<TMP_Text>();
        _agentHealthText = transform.GetChild(1).GetComponent<TMP_Text>();

        _agentNameText.enabled = false;
        _agentHealthText.enabled = false;
    }

    private void OnAgentSelected(Agent agent) {
        _selectedAgent = agent;

        _agentNameText.enabled = true;
        _agentHealthText.enabled = true;

        _agentNameText.text = agent.name;
        SetHP();
    }

    private void OnAgentDeselected() {
        _selectedAgent = null;
        
        _agentNameText.enabled = false;
        _agentHealthText.enabled = false;
    }

    private bool IsAgentSelected() {
        return _selectedAgent != null;
    }

    private void OnTick(int timeStep) {
        UpdateGUI();
    }

    private void SetHP() {
        _agentHealthText.text = _selectedAgent.GetHealth() + "HP";
    }
    
    private void UpdateGUI() {
        if (!IsAgentSelected()) return;
        SetHP();
    }
}
