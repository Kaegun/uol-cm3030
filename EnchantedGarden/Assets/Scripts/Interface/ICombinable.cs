public interface ICombinable
{
	bool CanBeCombined { get; }
	void Combining();
	void OnCombine();
}
