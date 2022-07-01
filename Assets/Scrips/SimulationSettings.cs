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
    
    // MEMORY SECTION
    // Defining the radius of which the memory of the agent is affected if a new experience was made
    public static readonly int MemoryWorldCellNeedSatisfactionAssociationRadius = 4;
    
    // Defining the values of how much each agent forgets values of the location memory.
    // The values are split for positive and negative associations (negative values are forgotten quicker than positive
    // ones).
    // The values should be 0 < x <= 1 where 1 would mean that the agent would never forget anything and 0 would mean
    // that the agent would forget in one time step everything he/she knew.
    public static readonly double LocationMemoryPainAvoidancePositiveForgetRate = 0.95;
    public static readonly double LocationMemoryPainAvoidanceNegativeForgetRate = 0.9;
    
    public static readonly double LocationMemoryEnergyPositiveForgetRate = 0.95;
    public static readonly double LocationMemoryEnergyNegativeForgetRate = 0.9;

    public static readonly double LocationMemoryAffiliationPositiveForgetRate = 0.95;
    public static readonly double LocationMemoryAffiliationNegativeForgetRate = 0.9;

    public static readonly double LocationMemoryCertaintyPositiveForgetRate = 0.95;
    public static readonly double LocationMemoryCertaintyNegativeForgetRate = 0.9;

    public static readonly double LocationMemoryCompetencePositiveForgetRate = 0.95;
    public static readonly double LocationMemoryCompetenceNegativeForgetRate = 0.9;
    
    // Simple datastructure to hold the forget values
    // The first value will define if the values are positive or negative and the second value corresponds to the
    // need in question
    public static readonly double[][] LocationMemoryForgetRates = new double[][] {
        new double[] {
            SimulationSettings.LocationMemoryPainAvoidancePositiveForgetRate,
            SimulationSettings.LocationMemoryEnergyPositiveForgetRate,
            SimulationSettings.LocationMemoryAffiliationPositiveForgetRate,
            SimulationSettings.LocationMemoryCertaintyPositiveForgetRate,
            SimulationSettings.LocationMemoryCompetencePositiveForgetRate
        },
        new double[] {
            SimulationSettings.LocationMemoryPainAvoidanceNegativeForgetRate,
            SimulationSettings.LocationMemoryEnergyNegativeForgetRate,
            SimulationSettings.LocationMemoryAffiliationNegativeForgetRate,
            SimulationSettings.LocationMemoryCertaintyNegativeForgetRate,
            SimulationSettings.LocationMemoryCompetenceNegativeForgetRate
        }
    };

    // SOCIAL MEMORY SECTION
    // The forget rate is given 0 < x <= 1 where 1 means no forgetting and 0 means quick forgetting
    public static readonly double SocialMemoryPositiveForgetRate = 0.95;
    public static readonly double SocialMemoryNegativeForgetRate = 0.9;

    public static readonly double[] SocialMemoryForgetRate = new double[] {
        SocialMemoryPositiveForgetRate,
        SocialMemoryNegativeForgetRate
    };

    // A factor of how much the agent takes in the information about the previously unknown agent
    // Situation: An agent receives social information about another agent he/she does not know yet. The value of how
    // much the agent is influenced by the previous agent received information is controlled by this factor.
    // 0 < x <= 1
    public static readonly double SocialMemoryReceiveNewUnknownAgentSoftenFactor = 0.8;

    // A factor regulating how much the newly shared information about a known agent is taken into account.
    // Example: I have a friend that I like by 0.8. Another friends tells me they like them only 0.5. I'll get influenced
    // by a small factor by this saying. This factor regulated this here. So the new value would be:
    // (0.5 * SocialMemoryReceiveNewKnownAgentAlphaFactor) + (0.8 * (1 - SocialMemoryReceiveNewKnownAgentAlphaFactor))
    // 0 <= x <= 1
    public static readonly double SocialMemoryReceiveNewKnownAgentAlphaFactor = 0.3;
    
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
