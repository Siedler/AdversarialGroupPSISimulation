
public class AgentIndividualMemory {
	private double _socialScore;

	public AgentIndividualMemory(double socialScore) {
		this._socialScore = socialScore;
	}
	
	public void UpdateSocialScore(double delta) {
		_socialScore += delta;

		if (_socialScore > 1) _socialScore = 1;
		else if (_socialScore < -1) _socialScore = -1;
	}

	public double GetSocialScore() {
		return _socialScore;
	}

	public void Tick() {
		_socialScore *= SimulationSettings.SocialMemoryForgetRate[_socialScore >= 0 ? 0 : 1];
	}
}