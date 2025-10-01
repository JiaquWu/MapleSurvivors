using System;
using UnityEngine;

public class EnemyInstance : Poolable, ICharacterComponent
{
  AttributesRuntime attributesRuntime;
  EnemyBrain brain;
  CharacterMotor2D motor;
  CharacterEventHub eventHub;

  public Type[] Requirements => 
    new Type[] { typeof(AttributesRuntime), 
      typeof(EnemyBrain), 
      typeof(CharacterMotor2D),  
      typeof(CharacterEventHub)
    };

  public Type[] Provides => new Type[] { };

  public void Init(CharacterComponents components)
  {
    attributesRuntime = components.Require<AttributesRuntime>();
    brain = components.Require<EnemyBrain>();
    motor = components.Require<CharacterMotor2D>();
    eventHub = components.Require<CharacterEventHub>();
  }

  public void PostInit(CharacterComponents components)
  {

  }

  public void Init(EnemySpec enemySpec)
  {


















  }
}
