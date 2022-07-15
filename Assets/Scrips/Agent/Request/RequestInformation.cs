
public class RequestInformation {
	private readonly RequestType _requestType;
	
	private readonly Agent _callingAgent;
	private readonly  Agent _regardingAgent;

	private readonly  EnvironmentWorldCell _callingLocation;

	public RequestInformation(RequestType requestType, Agent callingAgent, Agent regardingAgent,
		EnvironmentWorldCell callingLocation) {
		_requestType = requestType;

		_callingAgent = callingAgent;
		_regardingAgent = regardingAgent;

		_callingLocation = callingLocation;
	}

	public RequestType GetRequestType() {
		return _requestType;
	}
	
	public Agent GetCallingAgent() {
		return _callingAgent;
	}

	public Agent GetRegardingAgent() {
		return _regardingAgent;
	}

	public EnvironmentWorldCell GetCallingLocation() {
		return _callingLocation;
	}

}