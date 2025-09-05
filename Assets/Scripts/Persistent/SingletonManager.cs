// Singleton.cs
using UnityEngine;

public abstract class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
  // 全局访问点
  public static T Instance
  {
    get
    {
      if (_instance) return _instance;

      // 场景中找（避免创建隐藏对象，保持可控）
      _instance = FindFirstObjectByType<T>(FindObjectsInactive.Exclude);
      if (_instance) return _instance;

      Debug.LogError($"[Singleton<{typeof(T).Name}>] Instance not found in scene.");
      return null;
    }
  }
  protected static T _instance;

  //[Header("Singleton")]
  //[Tooltip("是否跨场景常驻")]
  //[SerializeField] private bool _dontDestroyOnLoad = false;

  //[Tooltip("当出现重复实例时的处理方式")]
  //[SerializeField] private DuplicatePolicy _duplicatePolicy = DuplicatePolicy.DestroyNewAndLog;

  //public enum DuplicatePolicy
  //{
  //  KeepExistingSilently,     // 保留已存在的，不提示
  //  KeepExistingAndLog,       // 保留已存在的，输出警告
  //  DestroyNewAndLog,         // 销毁新对象，输出错误
  //  DestroyExistingAndKeepNew // 销毁旧对象，保留新对象（少用）
  //}

  // 子类可在 AwakeEnd 做初始化（确保单例就绪后）
  protected virtual void AwakeEnd() { }

  protected virtual void Awake()
  {
    if (_instance == null)
    {
      _instance = this as T;

      //if (_dontDestroyOnLoad)
      //  DontDestroyOnLoad(gameObject);

      AwakeEnd();
    }
    //else if (_instance != this)
    //{
    //  HandleDuplicate();
    //}
  }

  //void HandleDuplicate()
  //{
  //  switch (_duplicatePolicy)
  //  {
  //    case DuplicatePolicy.KeepExistingSilently:
  //      gameObject.SetActive(false);
  //      Destroy(gameObject);
  //      break;

  //    case DuplicatePolicy.KeepExistingAndLog:
  //      Debug.LogWarning($"[Singleton<{typeof(T).Name}>] Duplicate detected. Keeping existing, destroying new.", this);
  //      Destroy(gameObject);
  //      break;

  //    case DuplicatePolicy.DestroyNewAndLog:
  //      Debug.LogError($"[Singleton<{typeof(T).Name}>] Duplicate detected. Destroying new.", this);
  //      Destroy(gameObject);
  //      break;

  //    case DuplicatePolicy.DestroyExistingAndKeepNew:
  //      Debug.LogWarning($"[Singleton<{typeof(T).Name}>] Replacing existing instance.", this);
  //      var old = _instance as MonoBehaviour;
  //      if (old) Destroy(old.gameObject);
  //      _instance = this as T;
  //      AwakeEnd();
  //      break;
  //  }
  //}

  protected virtual void OnDestroy()
  {
    // 仅当自己是当前实例时清空
    if (_instance == this) _instance = null;
  }
}
