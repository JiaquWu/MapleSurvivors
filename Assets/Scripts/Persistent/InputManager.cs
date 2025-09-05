using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


public interface IInputReader
{
  //ActionType Current { get; }
  public PlayerActions Player { get; }
  public UIActions UI { get; }
}

// 控接口：只给授权者
internal interface IInputController
{
  int Push(ActionType type); // 返回 token
  void Pop(int token);        // 用 token 弹回
  void Switch(ActionType type);
}

[RequireComponent(typeof(PlayerInput))]
public class InputManager : SingletonManager<InputManager>, IInputReader, IInputController
{
  PlayerInput playerInput;

  public PlayerActions Player { get; private set; }
  public UIActions UI { get; private set; }

  private readonly Stack<ActionType> actionMapStack = new Stack<ActionType>();
  //public ActionType Current => actionMapStack.Count > 0 ? actionMapStack.Peek() : ActionType.Disable;

  public string playerActionMapName = "Player";
  public string uiActionMapName = "UI";

  string activeMapName;

  // — 对外公开：只读入口 —
  public static IInputReader Reader => Instance; // 所有人都拿这个

  // — 仅授权者可拿到控接口（做成 internal / 只在导演初始化时获取）—
  internal static IInputController Controller => Instance;

  protected override void AwakeEnd()
  {
    base.AwakeEnd();
    if (!TryGetComponent(out playerInput))
      Debug.LogError("no playerInput component found on InputManager");

    var asset = playerInput.actions;

    Player = new PlayerActions(asset.FindActionMap(playerActionMapName, true));
    UI = new UIActions(asset.FindActionMap(uiActionMapName, true));
  }

  struct Ctx { public ActionType type; public int token; }

  readonly Stack<Ctx> _stack = new Stack<Ctx>();
  int _nextToken = 1;


  // 初始化时压入一个基准（例如 Player 或 Disable）
  // 比如在 Awake 末尾：_stack.Push(new Ctx{ type = ActionType.Player, token = 0 });

  int IInputController.Push(ActionType t)
  {
    // 生成 token，入栈，应用栈顶
    int token = _nextToken++;
    _stack.Push(new Ctx { type = t, token = token });

    // 复用你已有的切换逻辑
    ((IInputController)this).Switch(t);
    return token;
  }

  void IInputController.Pop(int token)
  {
    if (_stack.Count == 0) return;

    // 只允许弹出栈顶，并且 token 必须匹配，避免别人乱弹
    var top = _stack.Peek();
    if (top.token != token)
    {
      Debug.LogWarning($"[InputManager] Pop ignored: token mismatch (top:{top.token}, got:{token})");
      return;
    }

    // 弹出当前
    _stack.Pop();

    // 如果还有上一个上下文，就应用它；没有就 Disable
    var nextType = _stack.Count > 0 ? _stack.Peek().type : ActionType.Disable;
    ((IInputController)this).Switch(nextType);
  }

  void IInputController.Switch(ActionType type)
  {
    switch (type)
    {
      case ActionType.Player:
        EnableOnly(playerActionMapName);
        //if (lockCursorInPlayer) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        break;

      case ActionType.UI:
        EnableOnly(uiActionMapName);
        //Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
        break;

      case ActionType.Disable:
        DisableCurrent();
        activeMapName = null;
        //Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
        // 如果用 PlayerInput 的消息回调，这句会让它没有“当前 Map”
        playerInput.SwitchCurrentActionMap(null);
        break;
    }
  }

  // —— 只禁用“上一个”并启用“下一个” —— //
  void EnableOnly(string nextMapName)
  {
    if (activeMapName == nextMapName) return;

    var asset = playerInput.actions;

    // 关掉当前
    if (!string.IsNullOrEmpty(activeMapName))
    {
      var prev = asset.FindActionMap(activeMapName, false);
      if (prev != null && prev.enabled) prev.Disable();
    }

    // 打开下一个
    var next = asset.FindActionMap(nextMapName, true);
    if (!next.enabled) next.Enable();

    // 告诉 PlayerInput “当前 Map”是谁（用于 PlayerInput 的回调/控制方案）
    playerInput.SwitchCurrentActionMap(nextMapName);

    activeMapName = nextMapName;
  }

  void DisableCurrent()
  {
    if (string.IsNullOrEmpty(activeMapName)) return;
    var asset = playerInput.actions;
    var prev = asset.FindActionMap(activeMapName, false);
    if (prev != null && prev.enabled) prev.Disable();
  }

  
}

public enum ActionType
{
  Disable,
  Player,
  UI
}


public class PlayerActions
{
  public InputAction
    Move,
    Look,
    Attack,
    Interact,
    Crouch,
    Jump,
    Previous,
    Next,
    Sprint;

  public PlayerActions(InputActionMap map)
  {
    Move = map.FindAction("Move", true);
    Look = map.FindAction("Look", true);
    Attack = map.FindAction("Attack", true);
    Interact = map.FindAction("Interact", true);
    Crouch = map.FindAction("Crouch", true);
    Jump = map.FindAction("Jump", true);
    Previous = map.FindAction("Previous", true);
    Next = map.FindAction("Next", true);
    Sprint = map.FindAction("Sprint", true);

  }

}

public class UIActions
{
  public InputAction 
    Navigate,
    Submit,
    Cancel,
    Point,
    Click,
    ScrollWheel,
    MiddleClick,
    RightClick,
    TrackedDevicePosition,
    TrackedDeviceOrientation;

  public UIActions(InputActionMap map)
  {
    Navigate = map.FindAction("Navigate", true);
    Submit = map.FindAction("Submit", true);
    Cancel = map.FindAction("Cancel", true);
    Point = map.FindAction("Point", true);
    Click = map.FindAction("Click", true);
    ScrollWheel = map.FindAction("ScrollWheel", true);
    MiddleClick = map.FindAction("MiddleClick", true);
    RightClick = map.FindAction("RightClick", true);
    TrackedDevicePosition = map.FindAction("TrackedDevicePosition", true);
    TrackedDeviceOrientation = map.FindAction("TrackedDeviceOrientation", true);
  }
}

