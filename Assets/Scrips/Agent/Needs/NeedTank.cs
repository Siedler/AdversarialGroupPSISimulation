

public class NeedTank {
	private double _currentValue;
	private double _setValue;
	private double _delta;

	private double _leakage;

	public NeedTank(double currentValue, double setValue, double leakage) {
		_currentValue = currentValue;
		_setValue = setValue;
		_leakage = leakage;

		_delta = 0;
	}

	public void UpdateTankValue(double value) {
		_currentValue += value;
		
		_delta += value;

		if (_currentValue > 1) _currentValue = 1;
		else if (_currentValue < 0) _currentValue = 0;
	}

	public void Tick() {
		_delta = 0;
		UpdateTankValue(-_leakage);		
	}

	public double GetCurrentValue() {
		return _currentValue;
	}

	public double GetSetValue() {
		return _setValue;
	}

	public double GetDelta() {
		return _delta;
	}

	/***
	 * Return the difference that has to be bridged. It is negative if the set value is below the current value
	 */
	public double GetDifference() {
		return _setValue - _currentValue;
	}

	/***
	 * In German: Bedarfs-Indikator (BedIn)
	 */
	public double GetNeedIndicator() {
		return -GetDifference();
	}
}