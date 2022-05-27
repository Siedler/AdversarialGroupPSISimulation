
using System;

public abstract class ActionPlanFoodRelated : ActionPlan{
	protected ActionPlanFoodRelated(Agent agent, Hypothalamus hypothalamus, HippocampusLocation locationMemory, HippocampusSocial socialMemory, Environment environment) : base(agent, hypothalamus, locationMemory, socialMemory, environment) { }
	
	protected ActionResult CollectFood(EnvironmentWorldCell currentEnvironmentWorldCell) {
		if (!currentEnvironmentWorldCell.ContainsFood()) throw new InvalidOperationException("Tried to collect food even though the world cell has no food");

		agent.CollectFood();
		currentEnvironmentWorldCell.ConsumeFood();
		
		OnSuccess(0, 0, 0, 0, 0.5);
		return ActionResult.Success;
	}

	protected ActionResult CollectAndEatFood(EnvironmentWorldCell currentEnvironmentWorldCell) {
		if (!currentEnvironmentWorldCell.ContainsFood()) throw new InvalidOperationException("Tried to collect food even though the world cell has no food");

		currentEnvironmentWorldCell.ConsumeFood();

		OnSuccess(0, 0.1, 0, 0, 0.5);
		return ActionResult.Success;
	}
}