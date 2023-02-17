public interface ICombinable
{
	bool CanBeCombined { get; }
	bool Combining();
	void OnCombine();
}
