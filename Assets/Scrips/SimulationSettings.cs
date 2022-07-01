using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class SimulationSettings {
    public static readonly int AgentViewDistance = 4;
    
    public static readonly int NumberOfWorldCellsToExchange = 10;

    public static readonly int MaximumStoredFoodCount = 2;

    // Agent Personality values
    public static readonly double SetValuePainAvoidanceMean = 0.8;
    public static readonly double SetValuePainAvoidanceSigma = 0.03;

    public static readonly double LeakageValuePainAvoidanceMean = -0.02;
    public static readonly double LeakageValuePainAvoidanceSigma = 0.002;
    
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
    
    // FOOD
    public static readonly double FoodEnergyIntakeValue = 0.4;
    
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
    
    // ACTION PLAN SECTION
    // Structure double[] = {painAvoidance, energyIntake, affiliation, certainty, competence}
    // EXPLORE
    public static readonly double[] ExploreOnSuccess = new double[] {
        0,
        0,
        0,
        0.3,
        0.2,
    };
    public static readonly double[] ExploreOnFailure = new double[] {
        0,
        0,
        0,
        0,
        0,
    };
    
    // Engage
    // Both are set as base values and modified based on the social score of the specific agent
    public static readonly double[] EngageOnSuccess = new double[] {
        0,
        0,
        -0.2,
        0.2,
        0.2
    };
    public static readonly double[] EngageOnFailure = new double[] {
        0,
        0,
        -0.2, 
        -0.2,
        -0.2
    };
    
    // Flee
    public static readonly double[] FleeOnSuccess = new double[] {
        0.1,
        0,
        0,
        0.2,
        0.2,
    };
    public static readonly double[] FleeOnFailure = new double[] {
        -0.1,
        0,
        0,
        -0.2,
        -0.2
    };
    
    // Food Related
    public static readonly double[] FoodRelatedOnSuccess = new[] {
        0,
        SimulationSettings.FoodEnergyIntakeValue,
        0,
        0.2,
        0.2
    };
    public static readonly double[] FoodRelatedOnFailure = new double[] {
        0,
        0,
        0,
        -0.2,
        -0.2
    };
    
    // Call For Food
    // The affiliation signal for this action plan is regulated through the "receive food" method inside the
    // agent script
    public static readonly double[] CallForFoodOnSuccess = new double[] {
        0,
        FoodEnergyIntakeValue,
        0,
        0.2,
        0.2
    };
    public static readonly double[] CallForFoodOnFailure = new double[] {
        0,
        FoodEnergyIntakeValue,
        -0.2,
        -0.2,
        -0.2
    };

    // Collect Food
    public static readonly double[] CollectFoodOnSuccess = new double[] {
        0,
        0,
        0,
        0.21,
        0.2,
    };
    public static readonly double[] CollectFoodOnFailure = new double[] {
        0,
        0,
        0,
        -0.2,
        -0.2
    };
    
    // Exchange Information
    public static readonly double[] ExchangeInformationOnSuccess = new double[] {
        0,
        0,
        0.3,
        0.1,
        0.2,
    };
    public static readonly double[] ExchangeInformationOnFailure = new double[] {
        0,
        0,
        -0.2,
        -0.15,
        -0.2
    };
    
    // Give Food
    public static readonly double[] GiveFoodOnSuccess = new double[] {
        0,
        0,
        0.2,
        0.2,
        0.2,
    };
    public static readonly double[] GiveFoodOnFailure = new double[] {
        0,
        0,
        -0.2,
        -0.2,
        -0.2,
    };
    
    // Go Heal
    public static readonly double[] GoHealOnSuccess = new double[] {
        0,
        0,
        0.2,
        0.2,
        0.2,
    };
    public static readonly double[] GoHealOnFailure = new double[] {
        0,
        0,
        -0.2,
        -0.2,
        -0.2
    };
    
    // Request Healing
    // The painAvoidance feeling and the affiliation feeling is satisfied through the action of the other agent
    public static readonly double[] RequestHealingOnSuccess = new double[] {
        0,
        0,
        0,
        0.2,
        0.2,
    };
    public static readonly double[] RequestHealingOnFailure = new double[] {
        0,
        0,
        -0.2,
        -0.2,
        -0.2,
    };
    
    // Self Heal
    public static readonly double[] SelfHealingOnSuccess = new double[] {
        0,
        0,
        0,
        0.2,
        0.2
    };
    public static readonly double[] SelfHealingOnFailure = new double[] {
        0,
        0,
        0,
        -0.2,
        -0.2
    };
}
