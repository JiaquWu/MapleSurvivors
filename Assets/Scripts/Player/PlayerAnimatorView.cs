using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorView : MonoBehaviour
{
  static readonly int SpeedHash = Animator.StringToHash("Speed");
  static readonly int AttackHash = Animator.StringToHash("Attack");

  [SerializeField] float smoothTime = 0.06f;
  [SerializeField] SpriteRenderer spriteRenderer;
  Animator anim;
  Rigidbody2D rb;

  float speedRaw;       // 物理步计算出来的速度
  float speedDisplay;   // 渲染帧平滑后的速度
  float speedVel;

  Vector2 lastPosFixed;

  void Awake()
  {
    anim = GetComponent<Animator>();
    rb = GetComponentInParent<Rigidbody2D>();
  }

  void FixedUpdate()
  {
    var pos = rb.position;
    var dt = Time.fixedDeltaTime;
    var offset = pos - lastPosFixed;
    speedRaw = dt > 0f ? offset.magnitude / dt : 0f;

    if (spriteRenderer != null && speedRaw > 0f)
    {
      if(offset.x != 0)
        spriteRenderer.flipX = (pos - lastPosFixed).x > 0f;
    }

    lastPosFixed = pos;

    
  }

  void Update()
  {
    speedDisplay = Mathf.SmoothDamp(speedDisplay, speedRaw, ref speedVel, smoothTime);
    anim.SetFloat(SpeedHash, speedDisplay);

    
  }

  public void PlayAttack() => anim.SetTrigger(AttackHash);

}