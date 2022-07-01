using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimulationSettings {
    public static readonly int AgentViewDistance = 4;
    
    public static readonly int NumberOfWorldCellsToExchange = 10;

    public static readonly int MaximumStoredFoodCount = 2;

    // Agent Personality values
    public static readonly double SetValuePainAvoidanceMean = 0.8;
    public static readonly double SetValuePainAvoidanceSigma = 0.03;

    public static readonly double SetValueEnergyMean = 0.8;
    public static readonly double SetValueEnergySigma = 0.03;
    
    public static readonly double LeakageValueEnergyMean = 0.02;
    public static readonly double LeakageValueEnergySigma = 0.002;

    public static readonly double SetValueAffiliationMean = 0.8;
    public static readonly double SetValueAffiliationSigma = 0.03;
    
    public static readonly double LeakageValueAffiliationMean = 0.02;
    public static readonly double LeakageValueAffiliationSigma = 0.002;
    
    public static readonly double SetValueCertaintyMean = 0.8;
    public static readonly double SetValueCertaintySigma = 0.03;
    
    public static readonly double LeakageValueCertaintyMean = 0.02;
    public static readonly double LeakageValueCertaintySigma = 0.002;
    
    public static readonly double SetValueCompetenceMean = 1;
    public static readonly double SetValueCompetenceSigma = 0;
    
    public static readonly double LeakageValueCompetenceMean = 0.02;
    public static readonly double LeakageValueCompetenceSigma = 0.002;
    
    // DAMAGE AMOUNT
    public static readonly double HitMinDamage = 1;
    public static readonly double HitMaxDamage = 12;
    
    // HEAL AMOUNT
    public static readonly double HealMinAmount = 5;
    public static readonly double HealMaxAmount = 15;
    
    // SOCIAL SECTION
    
    
    // MEMORY SECTION
    // Defining the radius of which the memory of the agent is affected if a new experience was made
    public static readonly int MemoryWorldCellNeedSatisfactionAssociationRadius = 4;

    // ACTION PLAN SECTION
    public static readonly double ActionPlanRollingAverageAlpha = 0.1;

    public static readonly double ActionPlanSuccessProbabilityAlphaMean = 0.3;
    public static readonly double ActionPlanSuccessProbabilityAlphaSigma = 0.05;

    public static readonly double ActionPlanGeneralVsSpecificCompetenceWeightMean = 0.5;
    public static readonly double ActionPlanGeneralVsSpecificCompetenceWeightSigma = 0.033;
    
    // TODO Check if these can be variable
    public static readonly double PainAvoidanceMultiplier = 3;
    public static readonly double EnergyMultiplier = 2;
    public static readonly double AffiliationMultiplier = 1;
    public static readonly double CertaintyMultiplier = 1;
    public static readonly double CompetenceMultiplier = 1;
    
    // Competence indicator weights
    public static readonly double CompetenceIndicatorWeightPainAvoidance = 0.9;
    public static readonly double CompetenceIndicatorWeightEnergy = 0.8;
    public static readonly double CompetenceIndicatorWeightAffiliation = 0.7;
    public static readonly double CompetenceIndicatorWeightCertainty = 0.7;
}
