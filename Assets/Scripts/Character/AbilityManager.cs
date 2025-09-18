using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour, ICharacterComponent
{
  public Type[] Requirements { get => throw new NotImplementedException(); }

  public Type[] Provides  { get => new Type[] { typeof(AbilityManager) }; }


  public void Init(CharacterComponents components)
  {
    throw new NotImplementedException();
  }

  public void PostInit(CharacterComponents components)
  {
    throw new NotImplementedException();
  }
}
