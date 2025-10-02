using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
  [SerializeField] GameObject playerPrefab;
  int _nextId = 1;

  private void Awake()
  {
    GameSignals.Subscribe<NewGameStarted>(OnNewGameStarted);
  }

  void OnNewGameStarted(NewGameStarted newGameStarted)
  {
    Spawn(Vector3.zero);
  }


  public PlayerHandle Spawn(Vector3 pos)
  {
    var go = Instantiate(playerPrefab, pos, Quaternion.identity);
    var installer = go.GetComponent<CharacterInstaller>(); // 你已有
    var ctx = installer.CharacterComponents;

    var hub = ctx.Require<CharacterEventHub>();
    var attr = ctx.Require<IAttributesQuery>();
    var sink = ctx.Require<IStatModifierSink>();

    var router = go.GetComponent<PlayerRouter>();
    var controller = go.GetComponent<PlayerCharacterController>();
    controller.Possess(router);
    (router as IInputController).Switch(ActionType.Player); 

    var handle = new PlayerHandle(_nextId++, go.transform, hub, attr, sink);
    HandleRegistry.Instance.Register(handle);
    return handle;
  }

  public void Despawn(PlayerHandle handle)
  {
    if (handle == null) return;
    HandleRegistry.Instance.Unregister(handle);
    Destroy(handle.Root.gameObject);
  }
}
