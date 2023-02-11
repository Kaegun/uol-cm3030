public interface ICombinable
{
	bool CanBeCombined { get; }

	void OnCombine();
}
