using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEventHub : MonoBehaviour, ICharacterComponent
{
    public UnityEvent OnPlayerLevelUp = new UnityEvent();

    public Type[] Requirements => new Type[] { }; 

    public Type[] Provides => new Type[] { typeof(PlayerEventHub) };

    public void Init(CharacterComponents components)
    {
        //throw new NotImplementedException();
    }

    public void PostInit(CharacterComponents components)
    {
        //throw new NotImplementedException();
    }
}
