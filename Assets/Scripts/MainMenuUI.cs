using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
  [SerializeField] Button gameStartButton;

  private void Awake()
  {
    gameStartButton.onClick.AddListener(OnGameStartButtonClicked);

  }

  void OnGameStartButtonClicked()
  {
    var op = SceneManager.UnloadSceneAsync(SceneNames.MainMenu);
    op.completed += (asyncOp) => SceneManager.LoadSceneAsync(SceneNames.InGame, LoadSceneMode.Additive);
  }
}
