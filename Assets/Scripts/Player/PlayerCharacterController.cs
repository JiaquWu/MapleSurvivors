using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerCharacterController : MonoBehaviour, ICharacterComponent
{
  [SerializeField] float moveSpeed = 0.5f;

  bool isPossessed = false;
  CharacterMotor2D motor;
  Vector2 moveInput;

    public Type[] Requirements => new Type[] { typeof(CharacterMotor2D) };

    public Type[] Provides => new Type[] { };



  public void Init(CharacterComponents components)
  {
    //throw new NotImplementedException();
    motor = components.Require<CharacterMotor2D>();
  }

    public void PostInit(CharacterComponents components)
    {
        //throw new NotImplementedException();
    }


  public void Possess(IInputReader inputReader)
  {
    if (isPossessed) return;
    isPossessed = true;

    inputReader.Player.Move.performed += ctx => HandleMove(ctx);
    inputReader.Player.Move.canceled += ctx => HandleMove(ctx);
  }

  public void Unpossess(IInputReader inputReader)
  {
    if (!isPossessed) return;
    isPossessed = false;

    inputReader.Player.Move.performed -= ctx => HandleMove(ctx);
    inputReader.Player.Move.canceled -= ctx => HandleMove(ctx);
  }

  private void HandleMove(CallbackContext callbackContext)
  {
    moveInput = callbackContext.ReadValue<Vector2>();
    if (moveInput.sqrMagnitude > 1f) moveInput = moveInput.normalized; // 斜向不加速
  }

  void FixedUpdate()
  {
    if (!isPossessed) return;
    Vector2 delta = moveInput * moveSpeed * Time.fixedDeltaTime;
    motor.Move(delta);
  }
}
