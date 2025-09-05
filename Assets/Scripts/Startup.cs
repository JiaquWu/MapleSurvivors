using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
  [SerializeField] string persistentScene = "Persistent";
  [SerializeField] string menuScene = "MainMenu";
  [SerializeField] string selfScene = "Startup";

  IEnumerator Start()
  {
    var loadPersistent = SceneManager.LoadSceneAsync(persistentScene, LoadSceneMode.Additive);
    while (!loadPersistent.isDone) yield return null;

    var loadMenu = SceneManager.LoadSceneAsync(menuScene, LoadSceneMode.Additive);
    while (!loadMenu.isDone) yield return null;


    var self = SceneManager.GetActiveScene();
    var menu = SceneManager.GetSceneByName(menuScene);
    if (menu.IsValid())
      SceneManager.SetActiveScene(menu);

    if(self.name == selfScene && self.isLoaded)
      yield return SceneManager.UnloadSceneAsync(self);
  }
}