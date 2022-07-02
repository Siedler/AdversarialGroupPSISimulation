using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Exporter : MonoBehaviour {

    private char separator = System.IO.Path.DirectorySeparatorChar;
    private string _pathToData;    
    
    private Environment _environment;
    
    void Start() {
        _environment = GameObject.Find("World").GetComponent<Environment>();
        _pathToData = Application.persistentDataPath + separator + "data";
    }
    
    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.E)) {
            Export();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.O)) {
            OpenInFileSystem();
        }
    }

    private void OpenInFileSystem() {
        Application.OpenURL("file://" + _pathToData);
    }

    private void WriteAgent(Agent agent, string pathToTeam) {
        string pathAgent = pathToTeam + separator + agent.name;
        Directory.CreateDirectory(pathAgent);

        string eventHistoryFileName = "event_history.txt";
        string socialMemoryJsonName = "social_memory.json";
            
        string eventString = agent.GetEventHistoryString();
        string socialMemoryJson = agent.GetAgentSocialMemoryJson();

        StreamWriter streamWriter = new StreamWriter(pathAgent + separator + eventHistoryFileName);
        streamWriter.Write(eventString);
        streamWriter.Close();

        streamWriter = new StreamWriter(pathAgent + separator + socialMemoryJsonName);
        streamWriter.Write(socialMemoryJson);
        streamWriter.Close();
    }

    private void Export() {
        string dateTime = System.DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
        string pathToData = _pathToData + separator + dateTime;

        string pathToTeam1 = pathToData + separator + "team1";
        string pathToTeam2 = pathToData + separator + "team2";
        
        // Create directory
        Directory.CreateDirectory(pathToData);
        Directory.CreateDirectory(pathToTeam1);
        Directory.CreateDirectory(pathToTeam2);
        
        (AgentController team1, AgentController team2) = _environment.getAgentController();
        List<Agent> team1Agents = team1.GetAgents();
        List<Agent> team2Agents = team2.GetAgents();
        
        // Save team1
        foreach (Agent agent in team1Agents) {
            WriteAgent(agent, pathToTeam1);
        }

        // Save team2
        foreach (Agent agent in team2Agents) {
            WriteAgent(agent, pathToTeam2);
        }
    }
}
