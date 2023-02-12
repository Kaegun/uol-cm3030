using UnityEngine;
using UnityEngine.SceneManagement;

public class UiSceneLoader : MonoBehaviour
{
	private void UISceneLoadComplete(AsyncOperation obj)
	{
		//	Do nothing right now
		Debug.Log($"Scene loading completed: {obj.isDone}");
	}

	// Start is called before the first frame update
	void Start()
	{
		//	Load UI Scene
		var operation = SceneManager.LoadSceneAsync(CommonTypes.Scenes.UI, LoadSceneMode.Additive);
		operation.completed += UISceneLoadComplete;
	}
}
