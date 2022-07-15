public class EngageEvent {
	private readonly int _timeStep;
	
	private readonly Agent _attackingAgent;
	private readonly Agent _attackedAgent;

	private readonly bool _intraTeam;

	public EngageEvent(Agent attackingAgent, Agent attackedAgent, int timeStep) {
		_attackingAgent = attackingAgent;
		_attackedAgent = attackedAgent;

		_timeStep = timeStep;

		_intraTeam = _attackingAgent.GetTeam() == _attackedAgent.GetTeam();
	}

	public string GetEngageEventObjectJson() {
		return "{"
		       + "\"time_step\" : " + _timeStep + ","
		       + "\"attacking_agent\" : \"" + _attackingAgent.name + "\","
		       + "\"attacked_agent\" : \"" + _attackedAgent.name + "\","
		       + "\"intra_group_attack\" : " + (_intraTeam ? "true" : "false") + "}";
	}
}