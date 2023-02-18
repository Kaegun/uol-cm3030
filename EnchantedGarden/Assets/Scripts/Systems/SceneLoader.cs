using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public static void LoadScene(string sceneName, bool additive = false)
	{
		var operation = SceneManager.LoadSceneAsync(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
		operation.completed += SceneLoadCompleted;
	}

	public static void UnloadScene(string sceneName)
	{
		var operation = SceneManager.UnloadSceneAsync(sceneName);
		operation.completed += SceneUnloadCompleted;
	}

	private static void SceneLoadCompleted(AsyncOperation obj)
	{
		Debug.Log($"Scene loading completed: {obj.isDone}");
	}

	private static void SceneUnloadCompleted(AsyncOperation obj)
	{
		Debug.Log($"Scene unloading completed: {obj.isDone}");
	}
}
