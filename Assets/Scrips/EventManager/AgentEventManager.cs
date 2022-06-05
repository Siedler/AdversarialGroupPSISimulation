using System;
using System.Diagnostics.Tracing;
using UnityEngine;

public class AgentEventManager : MonoBehaviour {

    public static AgentEventManager current;

    // Start is called before the first frame update
    void Awake() {
        current = this;
    }

    public event Action<Agent> OnAgentSelected;
    public void SelectAgent(Agent agent) {
        OnAgentSelected?.Invoke(agent);
    }

    public event Action OnAgentDeselected;
    public void DeselectAgent() {
        OnAgentDeselected?.Invoke();
    }
    
    public event Action<Agent> OnAgentSpawn;
    public void AgentSpawned(Agent agent) {
        OnAgentSpawn?.Invoke(agent);
    }
    
    public event Action<Agent> OnAgentDespawn;
    public void AgentDespawned(Agent agent) {
        OnAgentDespawn?.Invoke(agent);
    }
    
}
