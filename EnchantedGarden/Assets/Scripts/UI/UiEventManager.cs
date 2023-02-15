using UnityEngine;

//	Handle UI Events
public class UiEventManager : MonoBehaviour
{
	public void OnClick_LaunchGame()
	{
		SceneLoader.LoadScene(CommonTypes.Scenes.Level1);
	}

	public void OnClick_RestartGame()
	{
		GameManager.Instance.RestartGame();
	}
}
