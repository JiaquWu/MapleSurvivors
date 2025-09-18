using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterInstaller : MonoBehaviour
{
  [Tooltip("把实现了 ICharacterComponent 的组件拖进来；或留空用自动收集")]
  [SerializeField] private List<MonoBehaviour> componentRefs;

  public CharacterComponents CharacterComponents {  get; private set; }

  private void Awake()
  {
    // 1) 收集模块
    var modules = (componentRefs != null && componentRefs.Count > 0)
      ? componentRefs.OfType<ICharacterComponent>().ToList()
      : GetComponentsInChildren<MonoBehaviour>(true).OfType<ICharacterComponent>().ToList();

    CharacterComponents = new CharacterComponents();

    // 2) 注册“提供的契约”（先建索引，便于 Init 阶段按 Type 取）
    foreach (var component in modules)
    {
      var provides = component.Provides;
      if(provides != null && provides.Length > 0)
      {
        foreach (var provide in provides)
          CharacterComponents.Register(provide, component);
      }
      else
      {
        // 可选：自动推断（白名单）——避免把所有接口都注册
        // 例如：如果 m 实现了 IAttributesQuery / IBrain / EventHub / Motor2D / WeaponManager 就注册这些
      }
    }

    // 3) 校验依赖是否齐全（运行前就把错打出来）
    foreach (var component in modules)
    {
      var requirements = component.Requirements;
      if (requirements == null)
        continue;
      foreach (var requirement in requirements)
      {
        if(!CharacterComponents.Contains(requirement))
          Debug.LogError($"[Installer] {name}: {component.GetType().Name} requires {requirement.Name} but it's missing.", this);
      }
    }

    // 4) 两阶段初始化（先 Init，再 PostInit）
    foreach (var m in modules) m.Init(CharacterComponents);
    foreach (var m in modules) m.PostInit(CharacterComponents);
  }
}
