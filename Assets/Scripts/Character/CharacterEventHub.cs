using System;
using UnityEngine;
using UnityEngine.Events;

public class CharacterEventHub : MonoBehaviour, ICharacterComponent
{
  public event Action OnAttack;
  public event Action OnHurt;
  public event Action OnDie;

  public Type[] Requirements => new Type[] { }; 

    public Type[] Provides => new Type[] { typeof(CharacterEventHub) };

    public void Init(CharacterComponents components)
    {
        //throw new NotImplementedException();
    }

    public void PostInit(CharacterComponents components)
    {
        //throw new NotImplementedException();
    }

  public void RaiseAttack() => OnAttack?.Invoke();
  public void RaiseHurt() => OnHurt?.Invoke();
  public void RaiseDie() => OnDie?.Invoke();
}
