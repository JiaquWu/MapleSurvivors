using UnityEngine;

public class TestPossessOnce : MonoBehaviour
{
  [SerializeField] private PlayerCharacterController player;

  void Start()
  {
    if (!player) { Debug.LogError("Assign PlayerController in inspector."); return; }

    var router = player.GetComponent<PlayerRouter>();
    // 正式流程里由导演做；测试场景我们直接授予一次控制权
    player.Possess(router);                 // 订阅输入
    (router as IInputController).Switch(ActionType.Player); // 切到Player Map
  }
}