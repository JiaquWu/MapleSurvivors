using System;
using UnityEngine;

public enum EnemyState { Chase, Attack, Hurt, Dead }
public class EnemyBrain : MonoBehaviour, ICharacterComponent
{
  public EnemySpec spec;
  CharacterMotor2D motor;
  SpriteAnimatorLite anim;
  EnemyState state;


  public Type[] Requirements  => new Type[] { typeof(SpriteAnimatorLite) };

  public Type[] Provides => new Type[] { typeof(EnemyBrain) };

  public void Init(CharacterComponents components)
  {
    anim = components.Require<SpriteAnimatorLite>();
  }

  public void PostInit(CharacterComponents components)
  {

  }

  public void Tick(float dt, in PlayerSnapshot[] players)
  {
    if (state == EnemyState.Dead)
    {
      EnemyScheduler.Instance.Unregister(this, anim);

      return;
    }


    // 简单 LOD：太远就低频/睡眠（由 Scheduler 控制）
    // 1) 选最近玩家(每若干帧)
    //  var target = SelectNearest(players, ref targetPlayerId);

    //  switch (state)
    //  {
    //    case EnemyState.Chase:
    //      // 追
    //      Vector2 dir = (target.Pos - (Vector2)transform.position).normalized;
    //      motor.Move(dir, spec.MoveSpeed, dt);

    //      cd -= dt;
    //      if (cd <= 0f && CanAttack(target))
    //      {
    //        state = EnemyState.Attack;
    //        cd = spec.AttackCooldown;
    //        anim.PlayOnce(spec.AttackClip, onComplete: () => {
    //          PerformAttack(target);
    //          anim.Play(spec.MoveClip);
    //          state = EnemyState.Chase;
    //        });
    //      }
    //      break;

    //    case EnemyState.Hurt:
    //      // 短暂硬直/闪白，由 OnHurt 设置计时器
    //      // 计时器结束会回到 Chase
    //      break;
    //  }
  }
  //void OnHurt(DamageInfo dmg)
  //{
  //  if (state == EnemyState.Dead) return;
  //  anim.PlayOnce(spec.HurtClip, onComplete: () => anim.Play(spec.MoveClip));
  //  // 可加短暂硬直：state = EnemyState.Hurt; hurtTimer = spec.HurtDuration;
  //}
  //void OnDie()
  //{
  //  if (state == EnemyState.Dead) return;
  //  state = EnemyState.Dead;
  //  motor.enabled = false;
  //  anim.PlayOnce(spec.DeathClip, onComplete: () => Despawn());
  //}

  //void PerformAttack(in PlayerSnapshot target)
  //{
  //  switch (spec.AttackType)
  //  {
  //    case AttackType.Touch: /* 近身伤害在碰撞系统做 */ break;
  //    case AttackType.ShootRadial: ShootBulletsRadial(spec); break;
  //    case AttackType.ShootAtPlayer: ShootTowards(target.Pos); break;
  //      // …按需扩展
  //  }
  //}
}
