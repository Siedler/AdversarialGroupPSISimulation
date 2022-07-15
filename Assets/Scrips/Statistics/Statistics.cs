using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour {

    public static Statistics _current;

    private List<EngageEvent> _engageEvents;
    
    public void Awake() {
        _current = this;

        _engageEvents = new List<EngageEvent>();
    }

    public void LogEngage(Agent attackingAgent, Agent attackedAgent, int timeStep) {
        _engageEvents.Add(new EngageEvent(attackingAgent, attackedAgent, timeStep));
    }

    private string GetAllEngageEventObjectsJson() {
        string allObjects = "";

        for (int i = 0; i < _engageEvents.Count; i++) {
            allObjects += _engageEvents[i].GetEngageEventObjectJson();
            allObjects += i != _engageEvents.Count - 1 ? ",\n" : "\n";
        }

        return allObjects;
    }
    
    public string GetStatisticsJson() {
        return "{"
               + "\"engage_events\" : [\n"
               + GetAllEngageEventObjectsJson()
               + "\n]"
               + "}";
    }
}
