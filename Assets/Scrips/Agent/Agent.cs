using System;
using System.Collections.Generic;
using Priority_Queue;
using Scrips.Agent;
using Scrips.Agent.Memory;
using Scrips.Agent.Personality;
using Scrips.Helper.Math;
using UnityEngine;

public class Agent : MonoBehaviour {
    
    // Sprite management
    public Sprite[] teamSprites = new Sprite[2];
    private Transform _spriteObject;
    private SpriteRenderer _spriteRenderer;

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
    private double _health;
    private int _foodCount;

    private Queue<RequestInformation> _incomingRequests;

    private List<ActionPlan> _actionPlans;
    private Dictionary<Agent, GoHeal> _healingActionPlans;
    private Dictionary<Agent, ExchangeLocationInformation> _locationMemoryExchangeActionPlans;
    private Dictionary<Agent, ExchangeSocialInformation> _socialMemoryExchangeActionPlans;
    private Dictionary<FoodCluster, Tuple<ActionPlan, ActionPlan>> _foodClusterActionPlans;
    private Dictionary<Agent, GiveFood> _giveFoodActionPlans;
    private Dictionary<Agent, Tuple<Flee, Engage>> _fightFlightActionPlans;
    private SearchForFoodToEat _searchForFoodToEat;
    private RequestHealing _requestHealing;
    
    private SimplePriorityQueue<ActionPlan> _currentMotives;
    private ActionPlan _currentActionPlan;
    
    private int _motiveCheckInterval = 5;
    private int clock = 0;
    
    // This boolean is used to manage if the agent should evaluate its motives for a special occasion such as
    // being hit by another agent...
    private bool specialMotiveSelectionEvaluationRequired = false;

    // DEBUG
    public GameObject debugObject;
    private Queue<GameObject> toDespawn = new Queue<GameObject>();

    // Needed for the spawn of a new agent. This sets the team so that the sprite can be updated onSpawn
    public void InitiateAgent(int team, AgentPersonality agentPersonality, AgentController agentController) {
        this._team = team;
        this._agentController = agentController;
            
        _environment = GameObject.Find("World").GetComponent<Environment>();

        // Get everything that is needed for rendering an agent
        _spriteObject = this.transform.GetChild(0);
        _spriteRenderer = _spriteObject.GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = teamSprites[team];

        // Get random name
        NameGenerator nameGenerator = new NameGenerator();
        this.name = nameGenerator.GetRandomName();
        
        // Set name of agent
        TextMesh nameTag = this.transform.Find("NameTag").GetComponent<TextMesh>();
        nameTag.text = this.name;

        _agentPersonality = agentPersonality;

        _eventHistoryManager = new AgentEventHistoryManager(name);
        
        // Setup Agent
        _hypothalamus = new Hypothalamus(_agentPersonality);

        _incomingRequests = new Queue<RequestInformation>();

        _locationMemory = new HippocampusLocation(this, _environment, agentPersonality);
        _socialMemory = new HippocampusSocial(agentPersonality, team);

        _health = 0;

        SetupActionPlans();
    }

    private void SetupActionPlans() {
        _currentMotives = new SimplePriorityQueue<ActionPlan>();
        _actionPlans = new List<ActionPlan>();
        _healingActionPlans = new Dictionary<Agent, GoHeal>();
        _locationMemoryExchangeActionPlans = new Dictionary<Agent, ExchangeLocationInformation>();
        _socialMemoryExchangeActionPlans = new Dictionary<Agent, ExchangeSocialInformation>();
        _foodClusterActionPlans = new Dictionary<FoodCluster, Tuple<ActionPlan, ActionPlan>>();
        _fightFlightActionPlans = new Dictionary<Agent, Tuple<Flee, Engage>>();
        _giveFoodActionPlans = new Dictionary<Agent, GiveFood>();

        _actionPlans.Add(new Explore(this, _agentPersonality, _hypothalamus, _locationMemory, _socialMemory,
            _eventHistoryManager, _environment));

        _requestHealing = new RequestHealing(this, _agentPersonality, _hypothalamus, _locationMemory, _socialMemory,
            _eventHistoryManager, _environment);
        _actionPlans.Add(_requestHealing);
        
        _actionPlans.Add(new CallForFoodToEat(this, _agentPersonality, _hypothalamus, _locationMemory, _socialMemory,
            _eventHistoryManager, _environment));
        _searchForFoodToEat = new SearchForFoodToEat(this, _agentPersonality, _hypothalamus, _locationMemory,
            _socialMemory, _eventHistoryManager, _environment);
        _actionPlans.Add(_searchForFoodToEat);

        _actionPlans.Add(new EatCloseFood(this, _agentPersonality, _hypothalamus, _locationMemory, _socialMemory,
            _eventHistoryManager, _environment));
    }
    
    private void GenerateSocialActionPlans(Agent newlyMetAgent) {
        // Add engagement and healing to the agents behavior
        Engage engage = new Engage(this, _agentPersonality, _hypothalamus, _locationMemory, _socialMemory,
            _eventHistoryManager, _environment, newlyMetAgent);
        _actionPlans.Add(engage);

        Flee flee = new Flee(this, _agentPersonality, _hypothalamus, _locationMemory, _socialMemory,
            _eventHistoryManager, _environment, newlyMetAgent);
        _actionPlans.Add(flee);
        _fightFlightActionPlans.Add(newlyMetAgent, new Tuple<Flee, Engage>(flee, engage));

        GoHeal goHeal = new GoHeal(this, _agentPersonality, _hypothalamus, _locationMemory, _socialMemory,
            _eventHistoryManager, _environment, newlyMetAgent);
        _actionPlans.Add(goHeal);
        _healingActionPlans.Add(newlyMetAgent, goHeal);

        GiveFood giveFood = new GiveFood(this, _agentPersonality, _hypothalamus, _locationMemory, _socialMemory,
            _eventHistoryManager, _environment, newlyMetAgent);
        _actionPlans.Add(giveFood);
        _giveFoodActionPlans.Add(newlyMetAgent, giveFood);
        
        ExchangeLocationInformation exchangeLocationInformation = new ExchangeLocationInformation(this,
            _agentPersonality, _hypothalamus, _locationMemory, _socialMemory, _eventHistoryManager, _environment,
            newlyMetAgent);
        _actionPlans.Add(exchangeLocationInformation);
        _locationMemoryExchangeActionPlans.Add(newlyMetAgent, exchangeLocationInformation);

        ExchangeSocialInformation exchangeSocialInformation = new ExchangeSocialInformation(this, _agentPersonality,
            _hypothalamus, _locationMemory, _socialMemory, _eventHistoryManager, _environment, newlyMetAgent);
        _actionPlans.Add(exchangeSocialInformation);
        _socialMemoryExchangeActionPlans.Add(newlyMetAgent, exchangeSocialInformation);
    }

    public void AddNewFoodCluster(FoodCluster foodCluster) {
        ActionPlan eatFoodClusterActionPlan = new EatFoodClusterActionPlan(this, _agentPersonality, _hypothalamus, _locationMemory, _socialMemory, _eventHistoryManager, _environment, foodCluster, _searchForFoodToEat);
        ActionPlan collectFoodClusterActionPlan = new CollectFoodClusterActionPlan(this, _agentPersonality,
            _hypothalamus, _locationMemory, _socialMemory, _eventHistoryManager, _environment, foodCluster);
        _foodClusterActionPlans.Add(foodCluster, new Tuple<ActionPlan, ActionPlan>(eatFoodClusterActionPlan, collectFoodClusterActionPlan));
        _actionPlans.Add(eatFoodClusterActionPlan);
    }

    public void RemoveFoodCluster(FoodCluster foodCluster) {
        Tuple<ActionPlan, ActionPlan> foodClusterActionPlan = _foodClusterActionPlans[foodCluster];
        _actionPlans.Remove(foodClusterActionPlan.Item1);
        _actionPlans.Remove(foodClusterActionPlan.Item2);
        _foodClusterActionPlans.Remove(foodCluster);
        
        if(_currentMotives.Contains(foodClusterActionPlan.Item1)) _currentMotives.Remove(foodClusterActionPlan.Item1);
        if(_currentMotives.Contains(foodClusterActionPlan.Item2)) _currentMotives.Remove(foodClusterActionPlan.Item2);

        if (_currentActionPlan == foodClusterActionPlan.Item1 || _currentActionPlan == foodClusterActionPlan.Item2)
            _currentActionPlan = null;
    }

    public IEnumerable<FoodCluster> GetFoodClusters() {
        return _foodClusterActionPlans.Keys;
    }

    public void Spawn(EnvironmentWorldCell spawnCell, Direction startDirection) {
        SetOrientation(startDirection);
        SetCurrentWorldCell(spawnCell);

        // THIS IS JUST A BUG FIX!
        // IF AN AGENT JUST SPAWNED AND ANOTHER AGENT INTERACTS WITH HIM/HER THEN THE AGENT MIGHT NOT BE ABLE TO
        // ASSOCIATE THE EXPERIENCE WITH THE LOCATION
        List<EnvironmentWorldCell> fieldOfView = SenseEnvironment();
        SenseCloseAgents(fieldOfView);

        agentDirection = startDirection;

        _health = 100;
        _foodCount = 0;
        gameObject.SetActive(true);

        UpdateSpriteOrientation();
        
        AgentEventManager.current.AgentSpawned(this);
        
        _hypothalamus.ResetNeedTanks();

        _eventHistoryManager.AddHistoryEvent("Spawned!");
    }

    public void Despawn() {
        _currentActionPlan = null;
        _incomingRequests = new Queue<RequestInformation>();

        gameObject.SetActive(false);
        _currentEnvironmentWorldCell.Unoccupy();

        this.transform.position = Vector3.zero;

        _agentController.RegisterToRespawn(this.gameObject);

        AgentEventManager.current.AgentDespawned(this);
        
        _eventHistoryManager.AddHistoryEvent("Despawned!");
    }

    // Internal State regulation functions
    public void RegisterIncomingRequest(RequestInformation requestInformation) {
        _incomingRequests.Enqueue(requestInformation);
    }
    
    public void TakeDamage(double damage, Agent attackingAgent) {
        _health -= damage;

        double painAvoidanceSignal = -(damage / 100);

        if (attackingAgent != null) {
            Experience(painAvoidanceSignal, 0, -0.1, -0.05, -0.07);
            _socialMemory.SocialInfluence(attackingAgent, -0.1);
            
            // Register hit for both engage and flee operation
            _fightFlightActionPlans[attackingAgent].Item1.RegisterHit();
            _fightFlightActionPlans[attackingAgent].Item2.RegisterHit();

            specialMotiveSelectionEvaluationRequired = true;

            _eventHistoryManager.AddHistoryEvent("Got hit by " + attackingAgent.name + " with " + damage +
                                                 " points of damage.");

            if (SimulationSettings.Debug) {
                GameObject g = Instantiate(debugObject, _currentEnvironmentWorldCell.worldCoordinates,
                    Quaternion.identity);
                toDespawn.Enqueue(g);
            }
        } else {
            Experience(painAvoidanceSignal, 0, 0,  0, 0);
            _eventHistoryManager.AddHistoryEvent("Got " + damage + " damage!");
        }

        if (_health <= 0) {
            _health = 0;
            Despawn();
        }
    }

    public void Heal(double amount, Agent healingAgent = null) {
        if (amount <= 0) throw new ArgumentException("The amount to heal is not positive!");

        _health += amount;

        // Cap health at 100
        if (_health > 100) _health = 100;

        // if the agent healed itself
        if (healingAgent == null) {
            return;
        }
        
        // Another agent healed me!
        // Give social credit
        // TODO implement social credit!
        double socialEffect = 0.1;
        _socialMemory.SocialInfluence(healingAgent, socialEffect);

        // If the healing resulted out of a request => Handle the pain avoidance signal inside the action plan
        // otherwise handle it here
        double painAvoidanceEffect = _currentActionPlan == _requestHealing ? 0 : amount / 100;
        
        // Add the experience from healing
        Experience(painAvoidanceEffect, 0, socialEffect, 0, 0);
        
        _eventHistoryManager.AddHistoryEvent("Got healed by " + healingAgent.name + " with " +
                                                             amount + " and credited " + socialEffect +
                                                             " social score points.");
    }

    public int GetTeam() {
        return _team;
    }
    
    public bool IsAlive() {
        return _health > 0;
    }
    
    public double GetHealth() {
        return _health;
    }

    public void CollectFood() {
        _foodCount++;
    }

    public void ReceiveFood(Agent receiveFromAgent) {
        _foodCount++;
        
        _eventHistoryManager.AddHistoryEvent("Received food from " + receiveFromAgent.name);
        
        // Manage affiliation experience
        double affiliationScore = _socialMemory.GetSocialScore(receiveFromAgent) > 0 ? 0.2 : 0.15;
        Experience(0, 0, affiliationScore, 0, 0);
        _socialMemory.SocialInfluence(receiveFromAgent, 0.1);
    }
    
    public void ConsumeFoodFromStorage() {
        if (_foodCount == 0) throw new InvalidOperationException("Tried to eat food even though the agent has no food!");

        _foodCount--;
        
        // IMPORTANT: THE INFLUENCE OF THE FOOD INTAKE IS DONE IN THE ACTION PLAN!
    }

    public bool HasFood() {
        return _foodCount > 0;
    }

    public int GetFoodCount() {
        return _foodCount;
    }

    public void ReceiveLocationMemory(Dictionary<Vector3Int, double[]> needSatisfactionAssociations, Agent receiveFromAgent) {
        foreach ((Vector3Int coordinates, double[] associations) in needSatisfactionAssociations) {
            if(!_locationMemory.KnowsLocation(coordinates)) continue;
            
            _locationMemory.ReceiveLocationInformation(coordinates, associations);
        }

        if (!_socialMemory.KnowsAgent(receiveFromAgent))
            AddNewAgentToSocialScore(receiveFromAgent);
        
        _socialMemory.SocialInfluence(receiveFromAgent, 0.1);
        _eventHistoryManager.AddHistoryEvent("Got new location information!");
    }
    
    public void ReceiveAgentIndividualMemory(Agent correspondingAgent, double socialScore, Agent receiveFromAgent) {
        if (!_socialMemory.KnowsAgent(receiveFromAgent))
            AddNewAgentToSocialScore(receiveFromAgent);
        
        _socialMemory.SocialInfluence(receiveFromAgent, 0.1);
        
        if (_socialMemory.KnowsAgent(correspondingAgent)) {
            _socialMemory.ReceiveSocialInfluence(correspondingAgent, socialScore, _agentPersonality.GetValue("SocialMemoryReceiveNewKnownAgentAlphaFactor"));
            
            _eventHistoryManager.AddHistoryEvent("Got new information about " +
                                                 correspondingAgent.name + " (social score " + socialScore + ").");
            return;
        }

        double initialSocialScore =
            socialScore * _agentPersonality.GetValue("SocialMemoryReceiveNewKnownAgentAlphaFactor");

        _eventHistoryManager.AddHistoryEvent("Met new agent with social score " + socialScore +
                                             ". Setting own initial social score to " + initialSocialScore + ".");
        
        // Agent is not know:
        AddNewAgentToSocialScore(correspondingAgent, initialSocialScore);
    }

    public void ReceivedHelpAfterCalling(Agent agentThatHelps) {
        _socialMemory.SocialInfluence(agentThatHelps, 0.1);
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

        // Add current environment world cell if not known already
        if (!_locationMemory.KnowsLocation(_currentEnvironmentWorldCell.cellCoordinates)) {
            AgentMemoryWorldCell agentMemoryWorldCell = new AgentMemoryWorldCell(
                _currentEnvironmentWorldCell.cellCoordinates,
                _currentEnvironmentWorldCell.worldCoordinates, _currentEnvironmentWorldCell.GetWorldCellType());
            _locationMemory.AddNewWorldCellToMemory(_currentEnvironmentWorldCell.cellCoordinates, agentMemoryWorldCell);
        }
        
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

    private void AddNewAgentToSocialScore(Agent newlyMetAgent, double initialSocialScore = Double.NaN) {
        if (newlyMetAgent == null)
            throw new ArgumentException("Tried to add a new agent, but the agent provided is null!");
        
        // Skip if AddNewAgentToSocialScore is called for one self 
        if(newlyMetAgent == this) return;
        
        // If the agent is seen for the first time
        // -> Assign social score depending on team
        // -> Add to social memory
        // -> Generate new Action Plans associated with the agent
        if(double.IsNaN(initialSocialScore)) 
            initialSocialScore = newlyMetAgent._team == _team ? 
                MathHelper.NextGaussian(SimulationSettings.NewlyMetAgentSameTeamMean, SimulationSettings.NewlyMetAgentSameTeamSigma, 0, 1) 
                : MathHelper.NextGaussian(SimulationSettings.NewlyMetAgentOppositeTeamMean, SimulationSettings.NewlyMetAgentOppositeTeamSigma, -1, 0);
        _socialMemory.AddNewlyMetAgent(newlyMetAgent, initialSocialScore);
        GenerateSocialActionPlans(newlyMetAgent);
    }

    private List<Agent> SenseCloseAgents(List<EnvironmentWorldCell> fieldOfView) {
        List<Agent> agentsInFieldOfView = new List<Agent>();

        foreach (EnvironmentWorldCell environmentWorldCell in fieldOfView) {
            if(environmentWorldCell == null || !environmentWorldCell.IsOccupied()) continue;

            // If the world cell contains an agent

            Agent agentOnWorldCell = environmentWorldCell.GetAgent();

            // Skip myself
            if(agentOnWorldCell == this) continue;
            
            agentsInFieldOfView.Add(agentOnWorldCell);
            
            // Check if the Agent already knows the 
            if (_socialMemory.KnowsAgent(agentOnWorldCell)) continue;

            AddNewAgentToSocialScore(agentOnWorldCell);
        }

        return agentsInFieldOfView;
    }

    private void ProcessCertaintyUpdateForAgentsInRange(
        EnvironmentWorldCell currentEnvironmentWorldCell,
        List<EnvironmentWorldCell> agentFieldOfView,
        List<Agent> nearbyAgents) {
        
        if(!SimulationSettings.CertaintyAdjustmentActivated) return;

        if(nearbyAgents.Count == 0) return;
        
        double socialScoreSum = 0;
        foreach (Agent nearbyAgent in nearbyAgents) {
            socialScoreSum += _socialMemory.GetSocialScore(nearbyAgent);
        }

        double averageSocialScore = socialScoreSum / nearbyAgents.Count;
        double certaintyInfluence = SimulationSettings.CertaintyAdjustmentParameter * averageSocialScore;
        
        Experience(0, 0, 0, certaintyInfluence, 0);
    }
    
    private void ProcessIncomingRequests() {
        while (_incomingRequests.Count > 0) {
            RequestInformation incomingRequestInformation = _incomingRequests.Dequeue();

            if (incomingRequestInformation.GetCallingAgent() == this)
                throw new InvalidOperationException("Agent got a request from him/herself");
            
            // If the agent does not know the other agents: Add them to their list
            if (!_socialMemory.KnowsAgent(incomingRequestInformation.GetCallingAgent())) {
                AddNewAgentToSocialScore(incomingRequestInformation.GetCallingAgent());
            }
            if (incomingRequestInformation.GetRegardingAgent() != null
                && incomingRequestInformation.GetRegardingAgent() != this
                && !_socialMemory.KnowsAgent(incomingRequestInformation.GetRegardingAgent())) {
                AddNewAgentToSocialScore(incomingRequestInformation.GetRegardingAgent());
            }
            
            if (incomingRequestInformation.GetRequestType() == RequestType.InformationLocation) {
                if(incomingRequestInformation.GetRegardingAgent() != null
                   && incomingRequestInformation.GetRegardingAgent() != this) continue;
                
                _locationMemoryExchangeActionPlans[incomingRequestInformation.GetCallingAgent()].RegisterRequest();
                
            } else if (incomingRequestInformation.GetRequestType() == RequestType.InformationSocial) {
                if(incomingRequestInformation.GetRegardingAgent() != null
                   && incomingRequestInformation.GetRegardingAgent() != this) continue;

                _socialMemoryExchangeActionPlans[incomingRequestInformation.GetCallingAgent()].RegisterRequest();
                
            } else if (incomingRequestInformation.GetRequestType() == RequestType.Help) {
                if (incomingRequestInformation.GetRegardingAgent() == this
                    || !_socialMemory.KnowsAgent(incomingRequestInformation.GetRegardingAgent())) continue;

                _fightFlightActionPlans[incomingRequestInformation.GetRegardingAgent()].Item2.RequestHelpToAttackThisAgent(incomingRequestInformation.GetCallingAgent());
            } else if (incomingRequestInformation.GetRequestType() == RequestType.Healing) {
                _healingActionPlans[incomingRequestInformation.GetCallingAgent()].RequestHealing();
            } else if (incomingRequestInformation.GetRequestType() == RequestType.Food) {
                _giveFoodActionPlans[incomingRequestInformation.GetCallingAgent()].RegisterRequest();
            }
        }
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
        // TODO check with Nabil if this cap is correct
        // If the need_indicator is not capped to 0, then the agent searches actively for behaviour that would affect
        // a need negatively
        double[] indicator = new double[] {
            Math.Max(0, _hypothalamus.GetPainAvoidanceDifference()),
            Math.Max(0, _hypothalamus.GetEnergyDifference()),
            Math.Max(0, _hypothalamus.GetAffiliationDifference()),
            Math.Max(0, _hypothalamus.GetCertaintyDifference()),
            Math.Max(0, _hypothalamus.GetCompetenceDifference()),
        };
        /*double[] indicator = new double[] {
            _hypothalamus.GetPainAvoidanceDifference(),
            _hypothalamus.GetEnergyDifference(),
            _hypothalamus.GetAffiliationDifference(),
            _hypothalamus.GetCertaintyDifference(),
            _hypothalamus.GetCompetenceDifference(),
        };*/
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
        double generalVsSpecificCompetenceWeight =
            _agentPersonality.GetValue("ActionPlanGeneralVsSpecificCompetenceWeight");
        
        foreach (ActionPlan actionPlan in _currentMotives) {
            double[] expectedSatisfaction = actionPlan.GetExpectedSatisfaction();

            // Set the start motivation strength to 0 if the action that is being reviewed is the current action
            // and to the negative selectionThreshold (so subtract it from the final sum) if its another action plan.
            // That way the current action plan "protects" itself from the new action.
            double newMotiveStrength = (_currentActionPlan == null || actionPlan == _currentActionPlan) ? 0 : -selectionThreshold;
            // Create the weighted sum of the need-indicators
            for (int i = 0; i < indicator.Length; i++) {
                newMotiveStrength += multiplier[i] * indicator[i] * expectedSatisfaction[i];
            }
            
            newMotiveStrength += actionPlan.GetUrgency(currentEnvironmentWorldCell, agentsFieldOfView, nearbyAgents);
            
            // Weigh in the general and specific competence.
            // As the general competence is a running average over competence signals it value is always 0 <= x <= 1
            // As the specific competence is a running average over the success probability of an action plan its also 0 <= x <= 1
            // Therefore the weighted sum is also 0 <= x <= 1
            newMotiveStrength *= (generalVsSpecificCompetenceWeight * _hypothalamus.GetCurrentCompetenceValue() +
                                  ((1 - generalVsSpecificCompetenceWeight) * actionPlan.GetSuccessProbability()));
            
            // As the priority queue works using a min approach we negate the motive strength. That way we keep the 
            // motive with the highest motive strength at the top of the queue
            _currentMotives.UpdatePriority(actionPlan, (float) -newMotiveStrength);
        }

        return _currentMotives.First;
    }
    
    // Execute actions of one time-step
    public void Tick(int timeStep) {
        _eventHistoryManager.Tick(timeStep);
        
        if (!IsAlive()) return;

        clock++;

        _hypothalamus.Tick();
        _locationMemory.Tick();
        _socialMemory.Tick();
        
        // Sense the world and process it
        List<EnvironmentWorldCell> fieldOfView = SenseEnvironment();
        List<Agent> agentsInFieldOfView = SenseCloseAgents(fieldOfView);

        // Update the certainty tank if agents are around you accordingly if you like them or not, i.e. if you feel safe
        ProcessCertaintyUpdateForAgentsInRange(_currentEnvironmentWorldCell, fieldOfView, agentsInFieldOfView);
        
        // Process all incoming signals
        ProcessIncomingRequests();

        // If it is activated it adapt the pain avoidance tank according to the health level
        if (SimulationSettings.PainAvoidanceHealthAdaptionActivated) {
            if (Math.Abs(_hypothalamus.GetCurrentPainAvoidanceValue() - (_health / 100)) > 0.01) {
                Experience(((_health/100)-_hypothalamus.GetCurrentPainAvoidanceValue())*SimulationSettings.PainAvoidanceHealthAdaptionAlpha,
                    0,
                    0,
                    0,
                    0);
            }
        }

        if (clock >= _motiveCheckInterval || _currentActionPlan == null || specialMotiveSelectionEvaluationRequired) {
            _eventHistoryManager.AddHistoryEvent("Reevaluating the current motive!");

            specialMotiveSelectionEvaluationRequired = false;
            clock = 0;

            // The agents certainty is effected by the types of agents that are around him. If the agent is close to
            // agents he likes then the certainty level should increase (i.e. they feel safe). If the agent is around
            // enemies then the certainty level should drop.
            double certaintyInfluenceOfCloseAgents = GetCertaintyInfluenceOfCloseUpAgents(agentsInFieldOfView);
            _hypothalamus.InfluenceCertainty(certaintyInfluenceOfCloseAgents);

            ActionPlan strongestActionPlan = GetStrongestMotive(_currentEnvironmentWorldCell, fieldOfView, agentsInFieldOfView);

            if (_currentActionPlan != strongestActionPlan) {
                _eventHistoryManager.AddHistoryEvent("Changed my current action plan!");
                strongestActionPlan.InitiateActionPlan();

                _currentActionPlan = strongestActionPlan;
            }
        }

        ActionResult actionResult = _currentActionPlan.Execute(_currentEnvironmentWorldCell, fieldOfView, agentsInFieldOfView);
        if (actionResult != ActionResult.InProgress) {
            _currentActionPlan = null;

            if (actionResult == ActionResult.Success) _currentMotives.Dequeue();
        }

        if (_hypothalamus.GetCurrentEnergyValue() == 0) {
            TakeDamage(1, null);
        }

        if (SimulationSettings.Debug) {
            // Despawn
            while (toDespawn.Count > 0) {
                Destroy(toDespawn.Dequeue());
            }
        }
    }
    
    public void SetCurrentWorldCell(EnvironmentWorldCell environmentWorldCell) {
        _currentEnvironmentWorldCell = environmentWorldCell;
        if (environmentWorldCell != null) {
            transform.position = environmentWorldCell.worldCoordinates;
            environmentWorldCell.Occupy(this);
        }
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

    public double[][] GetNeedTankSummary() {
        return new double[][] {
            new double[] {
                _hypothalamus.GetCurrentPainAvoidanceValue(),
                _agentPersonality.GetValue("HypothalamusPainAvoidanceSetValue"),
                _agentPersonality.GetValue("HypothalamusPainAvoidanceLeakage")
            },
            new double[] {
                _hypothalamus.GetCurrentEnergyValue(), _agentPersonality.GetValue("HypothalamusEnergySetValue"),
                _agentPersonality.GetValue("HypothalamusEnergyLeakage")
            },
            new double[] {
                _hypothalamus.GetCurrentAffiliationValue(),
                _agentPersonality.GetValue("HypothalamusAffiliationSetValue"),
                _agentPersonality.GetValue("HypothalamusAffiliationLeakage")
            },
            new double[] {
                _hypothalamus.GetCurrentCertaintyValue(), _agentPersonality.GetValue("HypothalamusCertaintySetValue"),
                _agentPersonality.GetValue("HypothalamusCertaintyLeakage")
            },
            new double[] {
                _hypothalamus.GetCurrentCompetenceValue(), _agentPersonality.GetValue("HypothalamusCompetenceSetValue"),
                _agentPersonality.GetValue("HypothalamusCompetenceLeakage")
            },
        };
    }

    public string GetEventHistoryString() {
        return _eventHistoryManager.GetListOfEventsAsString();
    }

    public string GetAgentSocialMemoryJson() {
        return _socialMemory.ToJson();
    }

    public string GetAgentDescriptorObjectJson() {
        return "{"
               + "\"Hypothalamus\" : " + _hypothalamus.GetHypothalamusDescriptorJson() + "}";
    }
    
    
    // UNITY METHODS
    private void OnMouseDown() {
        AgentEventManager.current.SelectAgent(this);
        Debug.Log("Agent " + name + " was clicked!!!!!");
    }
}
