using UnityEngine;

public interface ISettable<T>
{
	void SetMaximum(T value);
	void SetValue(T value);

	Transform Transform { get; }
}
