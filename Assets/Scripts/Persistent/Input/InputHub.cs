using System.Collections.Generic;
using UnityEngine;

public class InputHub:SingletonManager<InputHub>
{
  private readonly Dictionary<int, PlayerRouter> players = new();

  public void Register(PlayerRouter router)
  {
    players[router.PlayerIndex] = router;
    // Debug.Log($"Registered player {router.PlayerIndex}");
  }

  public void Unregister(PlayerRouter router)
  {
    if (players.TryGetValue(router.PlayerIndex, out var r) && r == router)
      players.Remove(router.PlayerIndex);
  }

  public IInputReader GetReader(int playerIndex)
      => players.TryGetValue(playerIndex, out var r) ? (IInputReader)r : null;

  internal IInputController GetController(int playerIndex)
      => players.TryGetValue(playerIndex, out var r) ? (IInputController)r : null;

  // 便捷转发（可选）
  public void Switch(int playerIndex, ActionType t) => GetController(playerIndex)?.Switch(t);
  public int Push(int playerIndex, ActionType t) => GetController(playerIndex)?.Push(t) ?? -1;
  public void Pop(int playerIndex, int token) => GetController(playerIndex)?.Pop(token);

  // 单人便捷
  public IInputReader PrimaryReader => GetReader(0);
  internal IInputController PrimaryController => GetController(0);
}
