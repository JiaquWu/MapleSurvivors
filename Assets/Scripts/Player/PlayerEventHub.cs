using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEventHub: CharacterEventHub
{
  public event Action<int> OnLevelUp;

  public void RaiseLevelUp(int newLevel) => OnLevelUp?.Invoke(newLevel);
}
