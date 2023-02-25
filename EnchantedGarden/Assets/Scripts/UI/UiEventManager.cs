using UnityEngine;

//	Handle UI Events
public class UiEventManager : MonoBehaviour
{
	public void OnClick_LaunchGame()
	{
		SceneLoader.LoadScene(CommonTypes.Scenes.Level0);
	}

	public void OnClick_RestartGame()
	{
		GameManager.Instance.RestartGame();
	}

	public void OnClick_ContinueGame()
	{
		GameManager.Instance.ContinueGame();
	}

	public void OnClick_VictorySceneContinue()
    {
		GameManager.Instance.LoadNextLevel();
	}
}
