using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
{
	public static T Instance { get; protected set; }

	protected virtual void Awake()
	{
		if (Instance == null && Instance != this)
		{
			Instance = (T)this;
		}
		else
		{
			Destroy(this);
		}
	}
}
