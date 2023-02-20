using System;

public interface ICombinable
{
	bool CanBeCombined { get; }
	float CombinationThreshold { get; }
	event EventHandler<float> CombineProgress;
	bool Combining();
	void OnCombine();
}
