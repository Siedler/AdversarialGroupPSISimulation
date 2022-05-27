using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationSettings {
    public static readonly int AgentViewDistance = 4;

    public static readonly double FoodPleasure = 0.4f;

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
