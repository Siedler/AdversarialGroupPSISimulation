public class SocialEvent {
	private readonly int _timeStep;
	
	private readonly Agent _agent1;
	private readonly Agent _agent2;

	private readonly bool _sameTeam;

	private readonly SocialEventType _eventType;

	public SocialEvent(Agent agent1, Agent agent2, int timeStep, SocialEventType eventType) {
		_timeStep = timeStep;
		
		_agent1 = agent1;
		_agent2 = agent2;

		_sameTeam = agent1.GetTeam() == agent2.GetTeam();

		_eventType = eventType;
	}

	private string GetEventTypeString() {
		switch (_eventType) {
			case SocialEventType.ExchangeLocationInformation:
				return "\"LocationInformationExchange\"";
			case SocialEventType.ExchangeSocialInformation:
				return "\"SocialInformationExchange\"";
			case SocialEventType.GiveFood:
				return "\"GiveFood\"";
			case SocialEventType.Heal:
				return "\"Heal\"";
		}

		return "\"Unknown\"";
	}

	public string GetSocialEventJson() {
		return "{"
		       + "\"time_step\" : " + _timeStep + ","
		       + "\"agent1\" : \"" + _agent1.name + "\","
		       + "\"agent2\" : \"" + _agent2.name + "\","
		       + "\"same_team\" : " + (_sameTeam ? "true" : "false") + ","
		       + "\"event_type\" : " + GetEventTypeString() + "}";
	}
}