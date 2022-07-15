public class EngageEvent {

	private Agent _attackingAgent { get; }
	private Agent _attackedAgent { get; }

	private bool _intraTeam { get; }

	private int _timeStep { get; }
	
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
		       + "\"intra_group_attack\" : " + _intraTeam + "}";
	}
}