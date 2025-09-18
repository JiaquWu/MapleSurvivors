using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ICharacterComponent
{
  public System.Type[] Requirements { get;}

  // 我对外提供哪些“契约”？（可选；为空时可让 Installer 自动推断）
  System.Type[] Provides { get; }

  public void Init(CharacterComponents components);

  public void PostInit(CharacterComponents components);
}
