using System;
using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using Scrips.Agent;
using Scrips.Agent.Personality;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Agent : MonoBehaviour {
    
    // Sprite management
    public Sprite[] teamSprites = new Sprite[2];
    private Transform _spriteObject;
    private SpriteRenderer _spriteRenderer;

    private SimpleCamera _simpleCamera;

    private int _team;
    public Direction agentDirection = Direction.E;

    // Environment Information
    private Environment _environment;
    private EnvironmentWorldCell _currentEnvironmentWorldCell;

    private AgentController _agentController;
    
    // Agent Personality
    private AgentPersonality _agentPersonality;

    // Agent Memory
    private HippocampusLocation _locationMemory;
    private HippocampusSocial _socialMemory;
    
    // Need System
    private Hypothalamus _hypothalamus;

    private AgentEventHistoryManager _eventHistoryManager;

    // Internal State
    [Range(0,100)]
    private int _health;
    private int _foodCount;

    private Queue<RequestInformation> _incomingRequests;

    private List<ActionPlan> _actionPlans;
    private SimplePriorityQueue<ActionPlan> _currentMotives;
    private ActionPlan _currentActionPlan;
    
    private int _motiveCheckInterval = 5;
    private int clock = 0;

    // Needed for the spawn of a new agent. This sets the team so that the sprite can be updated onSpawn
    public void InitiateAgent(int team, AgentPersonality agentPersonality, AgentController agentController) {
        this._team = team;
        this._agentController = agentController;
            
        _environment = GameObject.Find("World").GetComponent<Environment>();

        // Get everything that is needed for rendering an agent
        _spriteObject = this.transform.GetChild(0);
        _spriteRenderer = _spriteObject.GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = teamSprites[team];

        _simpleCamera = Camera.main.GetComponent<SimpleCamera>();
        
        // Get random name
        NameGenerator nameGenerator = new NameGenerator();
        this.name = nameGenerator.GetRandomName();
        
        // Set name of agent
        TextMesh nameTag = this.transform.Find("NameTag").GetComponent<TextMesh>();
        nameTag.text = this.name;

        _agentPersonality = agentPersonality;

        _eventHistoryManager = new AgentEventHistoryManager();
        
        // Setup Agent
        _hypothalamus = new Hypothalamus(_agentPersonality);

        _incomingRequests = new Queue<RequestInformation>();

        _locationMemory = new HippocampusLocation(agentPersonality);
        _socialMemory = new HippocampusSocial(agentPersonality);

        SetupActionPlans();
    }

    private void SetupActionPlans() {
        _currentMotives = new SimplePriorityQueue<ActionPlan>();
        _actionPlans = new List<ActionPlan>();
        
        _actionPlans.Add(new Explore(this, _hypothalamus, _locationMemory, _socialMemory, _eventHistoryManager, _environment));
        _actionPlans.Add(new EatCloseFood(this, _hypothalamus, _locationMemory, _socialMemory, _eventHistoryManager, _environment));
    }
    
    private void GenerateSocialActionPlans(Agent newlyMetAgent) {
        // Add engagement and healing to the agents behavior
        _actionPlans.Add(new Engage(this, _hypothalamus, _locationMemory, _socialMemory, _eventHistoryManager, _environment, newlyMetAgent));
        _actionPlans.Add(new GoHeal(this, _hypothalamus, _locationMemory, _socialMemory, _eventHistoryManager, _environment, newlyMetAgent));
        _actionPlans.Add(new ExchangeSocialInformation(this, _hypothalamus, _locationMemory, _socialMemory, _eventHistoryManager, _environment, newlyMetAgent));
    }

    public void Spawn(EnvironmentWorldCell spawnCell, Direction startDirection) {
        SetOrientation(startDirection);
        SetCurrentWorldCell(spawnCell);

        this.agentDirection = startDirection;

        this._health = 100;
        this._foodCount = 0;
        gameObject.SetActive(true);

        UpdateSpriteOrientation();
        
        AgentEventManager.current.AgentSpawned(this);
        
        Debug.Log(name + " Spawned");
    }

    public void Despawn() {
        _currentActionPlan = null;
        _incomingRequests = new Queue<RequestInformation>();

        gameObject.SetActive(false);
        _currentEnvironmentWorldCell.Unoccupy();

        this.transform.position = Vector3.zero;

        _agentController.RegisterToRespawn(this.gameObject);

        AgentEventManager.current.AgentDespawned(this);
        
        Debug.Log(name + " Despawned");
    }

    // Internal State regulation functions
    public void RegisterIncomingRequest(RequestInformation requestInformation) {
        _incomingRequests.Enqueue(requestInformation);
    }
    
    public void TakeDamage(int damage, Agent attackingAgent) {
        _health -= damage;

        // TODO variable on damage and on how inflicted damage (better influence)
        Experience(-0.1, 0, -0.1, -0.3, -0.3);
        _socialMemory.SocialInfluence(attackingAgent, -0.5);
        
        if (_health <= 0) {
            _health = 0;
            Despawn();
        }
    }

    public void Heal(int amount, Agent healingAgent = null) {
        if (amount <= 0) throw new ArgumentException("The amount to heal is not positive!");
        
        _health += amount;

        if(healingAgent == null) return;
        
        // Another agent healed me!
        // Give social credit
        // TODO implement social credit!
        _socialMemory.SocialInfluence(healingAgent, 0.1f);
    }

    public int GetTeam() {
        return _team;
    }
    
    public bool IsAlive() {
        return _health > 0;
    }
    
    public int GetHealth() {
        return _health;
    }

    public void CollectFood() {
        _foodCount++;
    }
    
    public void EatFoodFromStorage() {
        if (_foodCount == 0) throw new InvalidOperationException("Tried to eat food even though the agent has no food!");

        _foodCount--;
        
        // IMPORTANT: THE INFLUENCE OF THE FOOD INTAKE IS DONE IN THE ACTION PLAN!
    }

    public bool HasFood() {
        return _foodCount >= 0;
    }

    public void ReceiveAgentIndividualMemory(Agent correspondingAgent, double socialScore) {
        if (_socialMemory.KnowsAgent(correspondingAgent)) {
            _socialMemory.ReceiveSocialInfluence(correspondingAgent, socialScore, _agentPersonality.GetValue("SocialMemoryReceiveNewKnownAgentAlphaFactor"));
            return;
        }

        double initialSocialScore =
            socialScore * _agentPersonality.GetValue("SocialMemoryReceiveNewKnownAgentAlphaFactor");
        
        // Agent is not know:
        _socialMemory.AddNewlyMetAgent(correspondingAgent, initialSocialScore);
    }

    // Experience something, i.e. influence the need satisfaction values
    public void Experience(double painAvoidance, double energyIntake, double affiliation, double certainty, double competence) {
        // Influence the hypothalamus, i.e. the needs
        _hypothalamus.Influence(
            painAvoidance,
            energyIntake,
            affiliation,
            certainty,
            competence);
        
        // Associate the experience with current location
        _locationMemory.UpdateNeedSatisfactionAssociations(
            _currentEnvironmentWorldCell.cellCoordinates,
            painAvoidance,
            energyIntake,
            affiliation,
            certainty,
            competence);
    }

    public AgentIndividualMemory GetIndividualMemory(Agent agent) {
        return _socialMemory.GetIndividualMemory(agent);
    }

    private List<EnvironmentWorldCell> SenseEnvironment() {
        List<EnvironmentWorldCell> fieldOfView = _environment.SenseWorld(_currentEnvironmentWorldCell, agentDirection);
        List<Agent> agentInFieldOfView = new List<Agent>();
        
        // Add 
        foreach (EnvironmentWorldCell environmentWorldCell in fieldOfView) {
            if(environmentWorldCell == null) continue; // World cell not existent / visible
            
            Vector3Int worldCellCoordinate = environmentWorldCell.cellCoordinates;

            // World Cell has not been seen yet; add to memory
            if(_locationMemory.KnowsLocation(worldCellCoordinate)) continue;
            
            // If the world cell has not been seen -> Add world cell to memory
            AgentMemoryWorldCell agentMemoryWorldCell = new AgentMemoryWorldCell(
                environmentWorldCell.cellCoordinates,
                environmentWorldCell.worldCoordinates, environmentWorldCell.GetWorldCellType());
            _locationMemory.AddNewWorldCellToMemory(worldCellCoordinate, agentMemoryWorldCell);
        }

        // Link up neighbours
        for (int i = 0; i < fieldOfView.Count; i++) {
            if(fieldOfView[i] == null) continue; // World cell not existent / visible

            AgentMemoryWorldCell agentMemoryWorldCell = _locationMemory.GetAgentMemoryWorldCell(fieldOfView[i].cellCoordinates);

            if(agentMemoryWorldCell.IsExplored()) continue;

            if (i < 6*((SimulationSettings.AgentViewDistance)*(SimulationSettings.AgentViewDistance-1)/2)) agentMemoryWorldCell.Explore();
            
            Vector3Int[] neighbouringCells =
                HexagonGridUtility.GetCoordinatesOfNeighbouringCells(agentMemoryWorldCell.cellCoordinates);

            for (int j = 0; j < 6; j++) {
                if(!_locationMemory.KnowsLocation(neighbouringCells[j])) continue;
                
                agentMemoryWorldCell.AddNeighbour(_locationMemory.GetAgentMemoryWorldCell(neighbouringCells[j]), j);
            }
        }

        return fieldOfView;
    }

    private List<Agent> SenseCloseAgents(List<EnvironmentWorldCell> fieldOfView) {
        List<Agent> agentsInFieldOfView = new List<Agent>();

        foreach (EnvironmentWorldCell environmentWorldCell in fieldOfView) {
            if(environmentWorldCell == null) continue;
            
            // If the world cell contains an agent
            if (!environmentWorldCell.IsOccupied()) continue;

            Agent agentOnWorldCell = environmentWorldCell.GetAgent();
            agentsInFieldOfView.Add(agentOnWorldCell);
            
            // Check if the Agent already knows the 
            if (_socialMemory.KnowsAgent(agentOnWorldCell)) continue;

            // If the agent is seen for the first time
            // -> Assign social score depending on team
            // -> Add to social memory
            // -> Generate new Action Plans associated with the agent
            double initialSocialScore = agentOnWorldCell._team == _team ? Random.Range(0, 1) : Random.Range(-1, 0);
            _socialMemory.AddNewlyMetAgent(agentOnWorldCell, initialSocialScore);
            GenerateSocialActionPlans(agentOnWorldCell);
        }

        return agentsInFieldOfView;
    }

    private double GetCertaintyInfluenceOfCloseUpAgents(List<Agent> agentsInFieldOfView) {
        // TODO Add more complex certainty update method
        double certaintyLevel = 0;
        
        foreach (Agent agent in agentsInFieldOfView) {
            certaintyLevel += _socialMemory.GetIndividualMemory(agent).GetSocialScore();
        }

        return certaintyLevel * 0.1f;
    }
    private ActionPlan GetStrongestMotive(
        EnvironmentWorldCell currentEnvironmentWorldCell,
        List<EnvironmentWorldCell> agentsFieldOfView,
        List<Agent> nearbyAgents) {
        double[] indicator = new double[] {
            _hypothalamus.GetPainAvoidanceDifference(),
            _hypothalamus.GetEnergyDifference(),
            _hypothalamus.GetAffiliationDifference(),
            _hypothalamus.GetCertaintyDifference(),
            _hypothalamus.GetCompetenceDifference(),
        };
        double[] multiplier = new double[] {
            SimulationSettings.PainAvoidanceMultiplier,
            SimulationSettings.EnergyMultiplier,
            SimulationSettings.AffiliationMultiplier,
            SimulationSettings.CertaintyMultiplier,
            SimulationSettings.CompetenceMultiplier,
        };

        // Check if action plans should be added to the queue of current motives
        foreach (ActionPlan actionPlan in _actionPlans) {
            // If the action plan is already part of the list of current motives check if it should still be part
            if (_currentMotives.Contains(actionPlan)) {
                if(actionPlan.CanBeExecuted(currentEnvironmentWorldCell, agentsFieldOfView, nearbyAgents)) continue;
                
                _currentMotives.Remove(actionPlan);
                continue;
            }
            
            // Otherwise add the action plan *if* it can be pursiuted
            if(!actionPlan.CanBeExecuted(currentEnvironmentWorldCell, agentsFieldOfView, nearbyAgents)) continue;
            _currentMotives.Enqueue(actionPlan, Single.MaxValue);
        }

        double selectionThreshold = _agentPersonality.GetValue("SelectionThreshold");
        
        foreach (ActionPlan actionPlan in _currentMotives) {
            double[] expectedSatisfaction = actionPlan.GetExpectedSatisfaction();

            // Set the start motivation strength to 0 if the action that is being reviewed is the current action
            // and to the negative selectionThreshold (so subtract it from the final sum) if its another action plan.
            // That way the current action plan "protects" itself from the new action.
            double newMotiveStrength = (_currentActionPlan == null || actionPlan == _currentActionPlan) ? 0 : -selectionThreshold;
            // Create the weighted sum of the need-indicators
            for (int i = 0; i < indicator.Length; i++) {
                newMotiveStrength += indicator[i] * multiplier[i] * expectedSatisfaction[i];
            }

            // As the priority queue works using a min approach we negate the motive strength. That way we keep the 
            // motive with the highest motive strength at the top of the queue
            _currentMotives.UpdatePriority(actionPlan, (float) -newMotiveStrength);
        }

        return _currentMotives.First;
    }
    
    // Execute actions of one time-step
    public void Tick() {
        if (!IsAlive()) return;

        clock++;

        _hypothalamus.Tick();
        _locationMemory.Tick();
        _socialMemory.Tick();
        
        // Sense the world and process it
        List<EnvironmentWorldCell> fieldOfView = SenseEnvironment();
        List<Agent> agentsInFieldOfView = SenseCloseAgents(fieldOfView);
        
        // TODO handle incoming requests
        _incomingRequests.Clear();

        if (clock >= _motiveCheckInterval || _currentActionPlan == null) {
            _eventHistoryManager.AddHistoryEvent("Agent " + name + ": reevaluating the current motive!");
            
            clock = 0;

            // The agents certainty is effected by the types of agents that are around him. If the agent is close to
            // agents he likes then the certainty level should increase (i.e. they feel safe). If the agent is around
            // enemies then the certainty level should drop.
            double certaintyInfluenceOfCloseAgents = GetCertaintyInfluenceOfCloseUpAgents(agentsInFieldOfView);
            _hypothalamus.InfluenceCertainty(certaintyInfluenceOfCloseAgents);

            ActionPlan strongestActionPlan = GetStrongestMotive(_currentEnvironmentWorldCell, fieldOfView, agentsInFieldOfView);

            if (_currentActionPlan != strongestActionPlan) {
                strongestActionPlan.InitiateActionPlan();

                _currentActionPlan = strongestActionPlan;
                
                _eventHistoryManager.AddHistoryEvent("Agent " + name + ": Changed my current action plan!");
            }
        }

        ActionResult actionResult = _currentActionPlan.Execute(_currentEnvironmentWorldCell, fieldOfView, agentsInFieldOfView);
        if (actionResult != ActionResult.InProgress) {
            _currentActionPlan = null;

            if (actionResult == ActionResult.Success) _currentMotives.Dequeue();
        }
    }

    public void SetCurrentWorldCell(EnvironmentWorldCell environmentWorldCell) {
        this._currentEnvironmentWorldCell = environmentWorldCell;
        if(environmentWorldCell != null) environmentWorldCell.Occupy(this);
    }

    public void ChangeCurrentWorldCell(EnvironmentWorldCell newEnvironmentWorldCell, Direction direction = Direction.E) {
        if (!_environment.DoesCellWithCoordinateExist(newEnvironmentWorldCell.cellCoordinates) || newEnvironmentWorldCell.IsOccupied()) {
            throw new InvalidOperationException("Couldn't change the world cell as the cell does not exist, or the cell is already occupied");
        }
        
        transform.position = newEnvironmentWorldCell.worldCoordinates;
        agentDirection = direction;
        
        _currentEnvironmentWorldCell.Unoccupy();
        newEnvironmentWorldCell.Occupy(this);
        _currentEnvironmentWorldCell = newEnvironmentWorldCell;
        
        UpdateSpriteOrientation();
    }

    public EnvironmentWorldCell GetCurrentWorldCell() {
        return _currentEnvironmentWorldCell;
    }
    
    public void UpdateSpriteOrientation() {
        // Flip the agent if the direction they are facing the left
        _spriteRenderer.flipY = agentDirection is Direction.NW or Direction.W or Direction.SW;

        int zRotation = 60 * (int)agentDirection;
        //this.transform.rotation = new Quaternion(0,0,zRotation,0);

        Vector3 temp = _spriteObject.rotation.eulerAngles;
        temp.z = zRotation;
        _spriteObject.rotation = Quaternion.Euler(temp);
    }
    
    // This is needed for orientation changes not caused by the internal behaviour of an agent. (e.g. after spawn)
    public void SetOrientation(Direction direction) {
        this.agentDirection = direction;

        if (_spriteRenderer != null) {
            UpdateSpriteOrientation();
        }
    }
    
    // UNITY METHODS
    private void OnMouseDown() {
        AgentEventManager.current.SelectAgent(this);
        Debug.Log("Agent " + name + " was clicked!!!!!");
    }
}
