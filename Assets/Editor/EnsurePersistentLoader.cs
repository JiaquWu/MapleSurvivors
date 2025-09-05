#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnsurePersistentLoader : MonoBehaviour
{
  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
  static void EnsurePersistent()
  {
    if (SceneManager.GetSceneByName("Persistent").isLoaded) return;

    var active = SceneManager.GetActiveScene();
    if (active.name == "Startup") return;

    SceneManager.LoadScene("Persistent", LoadSceneMode.Additive);
  }
}
#endif