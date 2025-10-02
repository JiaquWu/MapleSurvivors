using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneNames
{
  public const string Startup = "Startup";
  public const string MainMenu = "MainMenu";
  public const string Persistent = "Persistent";
  public const string InGame = "InGame";
  // ...
}

public enum GameState { Boot, MainMenu, Loading, Playing, WaveClear, LevelClear, GameOver }

public class GameManager : SingletonManager<GameManager>
{
  [SerializeField] int defaultSeed = 1;

  //[SerializeField] LevelEventBus levelBus;
  //[SerializeField] PlayerSpawner playerSpawner;
  //[SerializeField] LevelManager levelManager;
  //[SerializeField] SceneLoader sceneLoader;   // 你已有异步加载器

  GameState state;
  PlayerHandle player;

  protected override void AwakeEnd()
  {
    base.AwakeEnd();
    SceneManager.sceneLoaded += SceneManager_sceneLoaded;
  }

  private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
  {
    if(arg0 == SceneManager.GetSceneByName(SceneNames.InGame))
    {
      GameSignals.Publish(new NewGameStarted(defaultSeed));
    }
  }

  void Start() => ToMainMenu();

  void ToMainMenu() { state = GameState.MainMenu; /* 显示主菜单 UI */ }

  //public async void StartGame(LevelConfig cfg)
  //{
  //  state = GameState.Loading;
  //  await sceneLoader.LoadLevelAdditive(cfg.sceneName);
  //  levelBus.RaiseLevelLoaded(cfg.index);

  //  // 生成玩家（或多人）
  //  player = playerSpawner.Spawn(cfg.playerSpawn);
  //  HookPlayer(player); // 订玩家事件（比如死亡 → GameOver）

  //  // 开始波次
  //  //levelManager.StartWave(cfg.firstWave);
  //  state = GameState.Playing;

  //  // 订 Level 事件推进流程
  //  levelBus.OnWaveCleared += OnWaveCleared;
  //  levelBus.OnLevelCompleted += OnLevelCompleted;
  //}

  void OnWaveCleared()
  {
    //if (/* 最后一波 */) levelBus.RaiseLevelCompleted();
    //else levelManager.StartWave(/* next */);
  }

  void OnLevelCompleted()
  {
    state = GameState.LevelClear;
    // 展示结算/掉落/进入下一关按钮…
  }

  void HookPlayer(PlayerHandle h)
  {
    h.Hub.OnDie += () => { state = GameState.GameOver; /* 打开失败面板 */ };
  }
}



