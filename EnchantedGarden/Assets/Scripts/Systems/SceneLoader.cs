using UnityEngine;
using UnityEngine.SceneManagement;

//	TODO: Implement as scene loader helper // move to Common?
public class SceneLoader : MonoBehaviour
{
	public static void LoadScene(string sceneName, bool additive = false)
	{
		var operation = SceneManager.LoadSceneAsync(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
		operation.completed += SceneLoadCompleted;

		//if (!additive)
		//	SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
	}

	private static void SceneLoadCompleted(AsyncOperation obj)
	{
		Debug.Log($"Scene loading completed: {obj.isDone}");
	}
}
