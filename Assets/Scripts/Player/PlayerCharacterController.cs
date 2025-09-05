using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerCharacterController : MonoBehaviour
{
  bool isPossessed = false;

  public void Possess(IInputReader inputReader)
  {
    if (isPossessed) return;
    isPossessed = true;

    inputReader.Player.Move.performed += ctx => HandleMove(ctx);
  }

  public void Unpossess(IInputReader inputReader)
  {
    if (!isPossessed) return;
    isPossessed = false;

    inputReader.Player.Move.performed -= ctx => HandleMove(ctx);
  }

  private void HandleMove(CallbackContext callbackContext)
  {

  }
}
