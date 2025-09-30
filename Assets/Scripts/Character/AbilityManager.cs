using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour, ICharacterComponent
{
  public Type[] Requirements { get => new Type[] { }; }

  public Type[] Provides  { get => new Type[] { typeof(AbilityManager) }; }


  public void Init(CharacterComponents components)
  {

  }

  public void PostInit(CharacterComponents components)
  {

  }
}
