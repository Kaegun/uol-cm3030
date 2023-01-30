using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
{
	public static T Instance { get; private set; }

	protected void Awake()
	{
		if (Instance == null && Instance != this)
		{
			Instance = (T)this;
		}
		else
		{
			//	This shouldn't be possible
			Destroy(this);
		}
	}
}
