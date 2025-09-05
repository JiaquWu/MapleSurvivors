

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RuntimeRefs/PlayerRef")]
public class PlayerReference : ScriptableObject
{
  [NonSerialized] private PlayerCharacterController _current;
  public PlayerCharacterController Current => _current;

  public event Action<PlayerCharacterController> OnChanged;

  public void Set(PlayerCharacterController player)
  {
    if (_current == player) return;
    _current = player;
    OnChanged?.Invoke(_current);
  }

  public void Clear()
  {
    if (_current == null) return;
    _current = null;
    OnChanged?.Invoke(null);
  }
}
