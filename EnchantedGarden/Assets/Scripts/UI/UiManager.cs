using UnityEngine;

//	TODO: This probably needs to be either renamed or changed in some other way.
public class UiManager : MonoBehaviour
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
