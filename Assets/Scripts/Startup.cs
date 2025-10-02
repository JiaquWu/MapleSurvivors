using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
  //[SerializeField] string persistentScene = "Persistent";
  //[SerializeField] string menuScene = "MainMenu";
  //[SerializeField] string selfScene = "Startup";

  IEnumerator Start()
  {
    var loadPersistent = SceneManager.LoadSceneAsync(SceneNames.Persistent, LoadSceneMode.Additive);
    while (!loadPersistent.isDone) yield return null;

    var loadMenu = SceneManager.LoadSceneAsync(SceneNames.MainMenu, LoadSceneMode.Additive);
    while (!loadMenu.isDone) yield return null;


    var self = SceneManager.GetActiveScene();
    var menu = SceneManager.GetSceneByName(SceneNames.MainMenu);
    if (menu.IsValid())
      SceneManager.SetActiveScene(menu);

    if(self.name == SceneNames.Startup && self.isLoaded)
      yield return SceneManager.UnloadSceneAsync(self);
  }
}