using System;
using System.Linq;
using UnityEngine;

public class PlayerLevelManager : MonoBehaviour, ICharacterComponent
{
  [SerializeField] PassiveSpec passiveSpec;

  private PlayerEventHub eventHub;
  private IStatModifierSink statModifierSink;
  private int currentLevel = 1;

  public Type[] Requirements => new Type[] { typeof(PlayerEventHub), typeof(IStatModifierSink) };

  public Type[] Provides => new Type[] { };

  [ContextMenu("Level Up Test")]
  private void LevelUpTest()
  {
    eventHub.RaiseLevelUp(1);
  }

  public void Init(CharacterComponents components)
  {
    eventHub = components.Require<PlayerEventHub>();
    statModifierSink = components.Require<IStatModifierSink>();
    eventHub.OnLevelUp += OnPlayerLevelUp;
  }

  private void OnDisable()
  {
    eventHub.OnLevelUp -= OnPlayerLevelUp;
  }
  public void PostInit(CharacterComponents components)
  {

  }

  void OnPlayerLevelUp(int level)
  {
    if (passiveSpec == null) return;
    var mods = passiveSpec.GetLevelMods(++currentLevel, name);
    if (mods.Count() > 0)
      Debug.Log(mods.ElementAt(0).Stat);
    statModifierSink.AddOrUpdate(name, mods);
  }
}
