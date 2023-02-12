using UnityEngine;
using UnityEngine.SceneManagement;

//	TODO: This probably needs to be either renamed or changed in some other way.
public class SceneLoader : MonoBehaviour
{
	private void LoadScene(string sceneName, bool additive = false)
	{
		var operation = SceneManager.LoadSceneAsync(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
		operation.completed += SceneLoadCompleted;
	}

	private void SceneLoadCompleted(AsyncOperation obj)
	{
		Debug.Log($"Scene loading completed: {obj.isDone}");
	}

	public void OnLaunchClicked()
	{
		LoadScene(CommonTypes.Scenes.Level1);
	}
}
