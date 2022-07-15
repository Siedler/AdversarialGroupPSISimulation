using System;
using System.Collections.Generic;
using System.IO;
using Scrips.EventManager;
using UnityEngine;

public class Exporter : MonoBehaviour {

    private char separator = Path.DirectorySeparatorChar;
    private string _pathToData;
    
    private Environment _environment;

    void Start() {
        _environment = GameObject.Find("World").GetComponent<Environment>();
        
        SetupExport();

        TimeEventManager.current.OnTick += Tick;
    }
    
    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.E)) {
            ExportEventHistory();
            ExportSocialScores();
            ExportStatistics();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.O)) {
            OpenInFileSystem();
        }
    }
    
    private void Tick(int step) {
        if (step == 1) {
            ExportTeams();
            ExportSocialScores();
        }
        
        if (SimulationSettings.FrequentExport && step % SimulationSettings.ExportInterval == 0) {
            ExportSocialScores();
        }
    }

    private void OpenInFileSystem() {
        Application.OpenURL("file://" + _pathToData);
    }

    private void SetupExport() {
        string dateTime = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
        _pathToData = Application.persistentDataPath + separator + "data" + separator + dateTime;
        
        Directory.CreateDirectory(_pathToData);
    }
    
    private void WriteAgentEventHistory(Agent agent, string pathToTeam) {
        string pathAgent = pathToTeam + separator + agent.name;
        Directory.CreateDirectory(pathAgent);

        string eventHistoryFileName = "event_history.txt";
            
        string eventString = agent.GetEventHistoryString();
        StreamWriter streamWriter = new StreamWriter(pathAgent + separator + eventHistoryFileName);
        streamWriter.Write(eventString);
        streamWriter.Close();
    }

    private void ExportTeams() {
        string team1NamesFilePath = _pathToData + separator + "team1.json";
        string team2NamesFilePath = _pathToData + separator + "team2.json";
        
        (AgentController team1, AgentController team2) = _environment.getAgentController();
        List<Agent> team1Agents = team1.GetAgents();
        List<Agent> team2Agents = team2.GetAgents();

        string team1String = "{\n";
        for (int i = 0; i < team1Agents.Count; i++) {
            team1String += "\t\"" + team1Agents[i].name + "\" : " + team1Agents[i].GetAgentDescriptorObjectJson();
            team1String += i != team1Agents.Count - 1 ? ",\n" : "\n}";
        }
        
        string team2String = "{\n";
        for (int i = 0; i < team2Agents.Count; i++) {
            team2String += "\t\"" + team2Agents[i].name + "\" : " + team2Agents[i].GetAgentDescriptorObjectJson();
            team2String += i != team2Agents.Count - 1 ? ",\n" : "\n}";
        }
        
        StreamWriter streamWriter = new StreamWriter(team1NamesFilePath);
        streamWriter.Write(team1String);
        streamWriter.Close();
        
        streamWriter = new StreamWriter(team2NamesFilePath);
        streamWriter.Write(team2String);
        streamWriter.Close();
    }

    private void ExportEventHistory() {
        string pathToData = _pathToData + separator + TimeManager.current.GetCurrentTimeStep();
        
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
            WriteAgentEventHistory(agent, pathToTeam1);
        }

        // Save team2
        foreach (Agent agent in team2Agents) {
            WriteAgentEventHistory(agent, pathToTeam2);
        }
    }

    private void WriteAgentSocialScores(Agent agent, string pathToTeam) {
        string pathAgent = pathToTeam + separator + agent.name;
        Directory.CreateDirectory(pathAgent);

        string socialMemoryJsonName = "social_memory.json";
            
        string socialMemoryJson = agent.GetAgentSocialMemoryJson();
        
        StreamWriter streamWriter = new StreamWriter(pathAgent + separator + socialMemoryJsonName);
        streamWriter.Write(socialMemoryJson);
        streamWriter.Close();
    }

    private void ExportSocialScores() {
        string pathToData = _pathToData + separator + TimeManager.current.GetCurrentTimeStep();
        
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
            WriteAgentSocialScores(agent, pathToTeam1);
        }

        // Save team2
        foreach (Agent agent in team2Agents) {
            WriteAgentSocialScores(agent, pathToTeam2);
        }
    }

    private void ExportStatistics() {
        string pathToData = _pathToData + separator + TimeManager.current.GetCurrentTimeStep();
        
        Directory.CreateDirectory(pathToData);

        string statistics = Statistics._current.GetStatisticsJson();
        
        StreamWriter streamWriter = new StreamWriter(pathToData + separator + "statistics.json");
        streamWriter.Write(statistics);
        streamWriter.Close();
    }
}
