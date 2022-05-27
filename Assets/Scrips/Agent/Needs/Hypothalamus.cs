
public class Hypothalamus {

	private NeedTank _painAvoidance;
	private NeedTank _energy;
	private NeedTank _affiliation;
	private NeedTank _certainty;
	private NeedTank _competence;
	
	public Hypothalamus() {
		_painAvoidance = new NeedTank(1, 0.8, 0);
		_energy = new NeedTank(0.8, 0.8, 0.02);
		_affiliation = new NeedTank(1, 0.7, 0.02);
		_certainty = new NeedTank(0.1, 0.5, 0.02);
		_competence = new NeedTank(1, 1, 0);
	}

	public void Tick() {
		// This value is positive if the competence should shrink!!!
		// i.e. if the agent is not able to handle the situation / fill his need-tanks then the competence shrinks
		double competenceIndicator = CalculateCompetenceIndicatorChange();
		_competence.UpdateTankValue(-competenceIndicator);
		
		_painAvoidance.Tick();
		_energy.Tick();
		_affiliation.Tick();
		_certainty.Tick();
		
		_competence.Tick();
	}

	public void InfluencePainAvoidance(double value) {
		_painAvoidance.UpdateTankValue(value);
	}
	
	public void InfluenceEnergy(double value) {
		_energy.UpdateTankValue(value);
	}
	
	public void InfluenceAffiliation(double value) {
		_affiliation.UpdateTankValue(value);
	}
	
	public void InfluenceCertainty(double value) {
		_certainty.UpdateTankValue(value);
	}
	
	public void InfluenceCompetence(double value) {
		_competence.UpdateTankValue(value);
	}

	public void Influence(double painAvoidanceValue = 0.0, double energyIntakeValue = 0.0,
		double affiliationValue = 0.0, double certaintyValue = 0.0, double competenceValue = 0.0) {
		_painAvoidance.UpdateTankValue(painAvoidanceValue);
		_energy.UpdateTankValue(energyIntakeValue);
		_affiliation.UpdateTankValue(affiliationValue);
		_certainty.UpdateTankValue(certaintyValue);
		_competence.UpdateTankValue(competenceValue);
	}

	public double GetCurrentPainAvoidanceValue() {
		return _painAvoidance.GetCurrentValue();
	}
	
	public double GetCurrentEnergyValue() {
		return _energy.GetCurrentValue();
	}
	
	public double GetCurrentAffiliationValue() {
		return _affiliation.GetCurrentValue();
	}
	
	public double GetCurrentCertaintyValue() {
		return _certainty.GetCurrentValue();
	}
	
	public double GetCurrentCompetenceValue() {
		return _competence.GetCurrentValue();
	}

	public double GetPainAvoidanceDifference() {
		return _painAvoidance.GetDifference();
	}
	
	public double GetEnergyDifference() {
		return _energy.GetDifference();
	}
	
	public double GetAffiliationDifference() {
		return _affiliation.GetDifference();
	}
	
	public double GetCertaintyDifference() {
		return _certainty.GetDifference();
	}
	
	public double GetCompetenceDifference() {
		return _competence.GetDifference();
	}

	private double CalculateCompetenceIndicatorChange() {
		double painAvoidanceChange = _painAvoidance.GetDelta() * SimulationSettings.CompetenceIndicatorWeightPainAvoidance;
		double energyIntakeChange = _energy.GetDelta() * SimulationSettings.CompetenceIndicatorWeightEnergy;
		double affiliationChange = _affiliation.GetDelta() * SimulationSettings.CompetenceIndicatorWeightAffiliation;
		double certaintyChange = _certainty.GetDelta() * SimulationSettings.CompetenceIndicatorWeightCertainty;

		return painAvoidanceChange + energyIntakeChange + affiliationChange + certaintyChange;
	}
}