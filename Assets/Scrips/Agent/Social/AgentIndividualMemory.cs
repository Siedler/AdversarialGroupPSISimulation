
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

	public void SetSocialScore(double value) {
		_socialScore = value;
		
		if (_socialScore > 1) _socialScore = 1;
		else if (_socialScore < -1) _socialScore = -1;
	}

	public double GetSocialScore() {
		return _socialScore;
	}

	// This method handles that forget rate for social associations
	// Input:
	// socialForgetRatePositiveNegative: double[2] social forget rate [forget_p, forget_n] where 0 <= forget_x <= 1
	public void Forget(double[] socialForgetRatePositiveNegative) {
		_socialScore *= socialForgetRatePositiveNegative[_socialScore >= 0 ? 0 : 1];
	}
}