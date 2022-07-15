using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour {

    public static Statistics _current;

    private List<EngageEvent> _engageEvents;
    private List<SocialEvent> _socialEvents;
    
    public void Awake() {
        _current = this;

        _engageEvents = new List<EngageEvent>();
        _socialEvents = new List<SocialEvent>();
    }

    public void LogEngage(Agent attackingAgent, Agent attackedAgent, int timeStep) {
        _engageEvents.Add(new EngageEvent(attackingAgent, attackedAgent, timeStep));
    }

    public void LogSocialEvent(Agent agent1, Agent agent2, int timeStep, SocialEventType eventType) {
        _socialEvents.Add(new SocialEvent(agent1, agent2, timeStep, eventType));
    }

    private string GetAllEngageEventObjectsJson() {
        string allObjects = "";

        for (int i = 0; i < _engageEvents.Count; i++) {
            allObjects += _engageEvents[i].GetEngageEventObjectJson();
            allObjects += i != _engageEvents.Count - 1 ? ",\n" : "\n";
        }

        return allObjects;
    }

    private string GetAllSocialEventObjectsJson() {
        string allObjects = "";
        for (int i = 0; i < _socialEvents.Count; i++) {
            allObjects += _socialEvents[i].GetSocialEventJson();
            allObjects += i != _socialEvents.Count - 1 ? ",\n" : "\n";
        }

        return allObjects;
    }
    
    public string GetStatisticsJson() {
        return "{"
               + "\"engage_events\" : [\n"
               + GetAllEngageEventObjectsJson()
               + "\n],"
               + "\"social_events\" : [\n"
               + GetAllSocialEventObjectsJson()
               + "\n]"
               + "}";
    }
}
