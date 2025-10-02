using System;
using UnityEngine;

[System.Serializable] public class SpriteClip { public Sprite[] frames; public float fps = 10; public bool loop = true; }

public class SpriteAnimatorLite : MonoBehaviour, ICharacterComponent
{
  [SerializeField] SpriteRenderer sr;
  SpriteClip current;
  float t; int idx; bool playingOnce; System.Action onComplete;

  public Type[] Requirements => new Type[] { };

  public Type[] Provides => new Type[] { typeof(SpriteAnimatorLite) };

  public void Init(CharacterComponents components)
  {

  }

  public void Play(SpriteClip clip) { current = clip; t = 0; idx = 0; playingOnce = false; onComplete = null; sr.sprite = current.frames[0]; }
  public void PlayOnce(SpriteClip clip, System.Action onComplete) { current = clip; t = 0; idx = 0; playingOnce = true; this.onComplete = onComplete; sr.sprite = current.frames[0]; }

  public void PostInit(CharacterComponents components)
  {
  }

  public void Tick(float dt)
  {
    if (current == null) return;
    t += dt * current.fps;
    int next = (int)t;
    if (next == idx) return;
    idx = next;
    if (idx >= current.frames.Length)
    {
      if (playingOnce) { var cb = onComplete; onComplete = null; current = null; cb?.Invoke(); return; }
      idx %= current.frames.Length;
      t %= current.frames.Length;
    }
    sr.sprite = current.frames[idx];
  }
}
