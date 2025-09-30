using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimatorView : MonoBehaviour
{
  [SerializeField] CharacterEventHub eventHub;
  Animator animator;

  protected readonly int SpeedHash = Animator.StringToHash("Speed");
  protected readonly int AttackHash = Animator.StringToHash("Attack");
  protected readonly int HurtHash = Animator.StringToHash("Hurt");
  protected readonly int DieHash = Animator.StringToHash("Die");


  private void Awake()
  {
    if(eventHub == null)
      Debug.LogError($"[CharacterAnimatorView] {name}: Missing EventHub reference.", this);

    animator = GetComponent<Animator>();
  }

  private void OnEnable()
  {
    eventHub.OnAttack += OnAttack;
    eventHub.OnHurt += OnHurt;
    eventHub.OnDie += OnDie;
  }


  private void OnDisable()
  {
    if (eventHub == null)
      Debug.LogError($"[CharacterAnimatorView] {name}: Missing EventHub reference.", this);

    eventHub.OnAttack -= OnAttack;
    eventHub.OnHurt -= OnHurt;
    eventHub.OnDie -= OnDie;
  }

  void OnAttack()
  {
    animator.SetTrigger(AttackHash);
  }

  void OnHurt()
  {
    animator.SetTrigger(HurtHash);
  }

  void OnDie()
  {
    animator.SetTrigger(DieHash);
  }

}
