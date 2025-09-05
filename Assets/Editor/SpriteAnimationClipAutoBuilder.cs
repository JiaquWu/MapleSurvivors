using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class SpriteAnimationClipAutoBuilder : EditorWindow
{
  [Header("Input")]
  Object spritesFolder;
  [Header("Output")]
  DefaultAsset outputFolder;
  [Header("Animator")]
  AnimatorController baseController;
  string overrideControllerName = "AOC_{FolderName}";
  [Header("Settings")]
  int fps = 12;
  bool createOverrideController = true;

  static readonly Dictionary<string, bool> loopRule = new Dictionary<string, bool>
    {
        {"idle", true}, {"move", true}, {"walk", true}, {"run", true}, {"hover", true},
        {"attack", false}, {"hit", false}, {"hurt", false}, {"die", false}, {"death", false},
        {"cast", false}, {"spawn", false}
    };


  //static readonly Dictionary<string, string> stateMap = new Dictionary<string, string>
  //  {
  //      {"idle","Idle"}, {"move","Move"}, {"walk","Move"}, {"run","Move"},
  //      {"attack","Attack"}, {"hit","Hit"}, {"hurt","Hit"}, {"die","Die"}, {"death","Die"},
  //      {"cast","Attack"}, {"spawn","Idle"}
  //  };

  private void OnGUI()
  {
    GUILayout.Label("Source Sprites Folder", EditorStyles.boldLabel);
    spritesFolder = EditorGUILayout.ObjectField("Sprites Folder", spritesFolder, typeof(DefaultAsset), false);

    GUILayout.Space(6);
    GUILayout.Label("Output", EditorStyles.boldLabel);
    outputFolder = (DefaultAsset)EditorGUILayout.ObjectField("Output Folder", outputFolder, typeof(DefaultAsset), false);

    GUILayout.Space(6);
    GUILayout.Label("Animator", EditorStyles.boldLabel);
    baseController = (AnimatorController)EditorGUILayout.ObjectField("Base Controller", baseController, typeof(AnimatorController), false);
    createOverrideController = EditorGUILayout.Toggle("Create Animator Override", createOverrideController);
    overrideControllerName = EditorGUILayout.TextField("AOC Name", overrideControllerName);

    GUILayout.Space(6);
    GUILayout.Label("Settings", EditorStyles.boldLabel);
    fps = EditorGUILayout.IntSlider("FPS", fps, 6, 30);

    GUILayout.Space(10);
    using (new EditorGUI.DisabledScope(spritesFolder == null || outputFolder == null))
    {
      if (GUILayout.Button("Build Clips")) BuildSpriteAnimationClips();
    }
  }


  [MenuItem("Tools/Sprite Animation Clip Auto Builder")]
  static void Open() => GetWindow<SpriteAnimationClipAutoBuilder>("Sprite Animation Clip Auto Builder");


  void BuildSpriteAnimationClips()
  {
    var srcPath = AssetDatabase.GetAssetPath(spritesFolder);
    var dstPath = AssetDatabase.GetAssetPath(outputFolder);

    var folderName = Path.GetFileName(srcPath.TrimEnd('/', '\\'));
    var dstSubFolder = Path.Combine(dstPath, folderName).Replace("\\", "/");

    if (!AssetDatabase.IsValidFolder(dstSubFolder))
    {
      AssetDatabase.CreateFolder(dstPath, folderName);
    }

    if (!AssetDatabase.IsValidFolder(srcPath) || !AssetDatabase.IsValidFolder(dstPath))
    {
      EditorUtility.DisplayDialog("Error", "no vaild sprites folder or output folder", "OK");
      return;
    }

    var guids = AssetDatabase.FindAssets("t:Sprite", new[] { srcPath });
    var sprites = guids.Select(g => AssetDatabase.GUIDToAssetPath(g))
                          .SelectMany(p => AssetDatabase.LoadAllAssetsAtPath(p).OfType<Sprite>())
                          .OrderBy(s => s.name, new NaturalNameComparer()) // 自然排序 idle_0, idle_1 ...
                          .ToList();

    if (sprites.Count == 0)
    {
      EditorUtility.DisplayDialog("Info", "no sprites found", "OK");
      return;
    }

    var groups = sprites.GroupBy(s => GetPrefix(s.name))
                            .Where(g => !string.IsNullOrEmpty(g.Key))
                            .ToList();

    var createdClips = new Dictionary<string, AnimationClip>();

    foreach (var g in groups)
    {
      var prefix = g.Key; // e.g., "idle" / "move" / "attack" / "die"
      var frames = g.ToList();

      // generate AnimationClip
      // 生成 AnimationClip
      var clip = new AnimationClip { frameRate = fps };

#if UNITY_5_6_OR_NEWER
      var binding = new EditorCurveBinding
      {
        type = typeof(SpriteRenderer),
        path = "",
        propertyName = "m_Sprite"
      };
#else
            var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
#endif
      var keys = new ObjectReferenceKeyframe[frames.Count];
      for (int i = 0; i < frames.Count; i++)
      {
        keys[i] = new ObjectReferenceKeyframe
        {
          time = i / (float)fps,
          value = frames[i]
        };
      }
      AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);

      var loop = GuessLoop(prefix);
      var settings = AnimationUtility.GetAnimationClipSettings(clip);
      settings.loopTime = loop;
      AnimationUtility.SetAnimationClipSettings(clip, settings);

      //save
      var fileName = $"{prefix}.anim";
      // 保存 anim 的时候用 dstSubFolder
      var savePath = Path.Combine(dstSubFolder, $"{prefix}.anim").Replace("\\", "/");
      AssetDatabase.CreateAsset(clip, AssetDatabase.GenerateUniqueAssetPath(savePath));
      createdClips[prefix] = clip;
    }

    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();

    // Generate Animator Override Controller（optional）
    if (createOverrideController && baseController != null)
    {
      //var folderName = Path.GetFileName(srcPath.TrimEnd('/'));
      var aocName = overrideControllerName.Replace("{FolderName}", folderName);
      var aoc = new AnimatorOverrideController { runtimeAnimatorController = baseController };

      var pairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();
      foreach (var kv in baseController.animationClips.Distinct())
      {
        var baseClip = kv;
        var key = baseClip.name;
        //var key = MapStateKey(baseClip.name); // 将 baseClip 名字映射到前缀：Idle/Move/Attack/Die
        if (key != null && createdClips.TryGetValue(key, out var overrideClip))
          pairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(baseClip, overrideClip));
        else
          pairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(baseClip, baseClip));
      }
      aoc.ApplyOverrides(pairs);

      var aocPath = Path.Combine(dstSubFolder, aocName + ".overrideController").Replace("\\", "/");
      AssetDatabase.CreateAsset(aoc, AssetDatabase.GenerateUniqueAssetPath(aocPath));
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }

    EditorUtility.DisplayDialog("Done", $"生成 {createdClips.Count} 个 AnimationClip 完成。", "OK");

    //var guids = AssetDatabase.FindAssets("t:Sprite", new[] {  });

    //string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Sprites" });
    //foreach (string guid in guids)
    //{
    //  string path = AssetDatabase.GUIDToAssetPath(guid);
    //  TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
    //  if (textureImporter != null && textureImporter.spriteImportMode == SpriteImportMode.Single)
    //  {
    //    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
    //    foreach (Object asset in assets)
    //    {
    //      if (asset is Sprite sprite)
    //      {
    //        string clipName = sprite.name + "_Anim";
    //        string clipPath = System.IO.Path.GetDirectoryName(path) + "/" + clipName + ".anim";
    //        AnimationClip existingClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
    //        if (existingClip == null)
    //        {
    //          AnimationClip clip = new AnimationClip();
    //          clip.frameRate = 12;
    //          EditorCurveBinding spriteBinding = new EditorCurveBinding();
    //          spriteBinding.type = typeof(SpriteRenderer);
    //          spriteBinding.path = "";
    //          spriteBinding.propertyName = "m_Sprite";
    //          ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[1];
    //          keyFrames[0] = new ObjectReferenceKeyframe();
    //          keyFrames[0].time = 0;
    //          keyFrames[0].value = sprite;
    //          AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyFrames);
    //          AssetDatabase.CreateAsset(clip, clipPath);
    //          Debug.Log("Created Animation Clip: " + clipPath);
    //        }
    //      }
    //    }
    //  }
    //}
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();
  }

  //static string MapStateKey(string baseClipName)
  //{
  //  // 将基础Animator中的占位Clip名（如 Idle/Move/Attack/Die）映射到前缀键（idle/move/attack/die）
  //  var n = baseClipName.ToLowerInvariant();
  //  foreach (var kv in stateMap)
  //    if (n.Contains(kv.Value.ToLowerInvariant()))
  //      return kv.Key; // 返回用于查找createdClips的键
  //                     // 兜底：如果基础clip名本身就是前缀
  //  return stateMap.ContainsKey(n) ? n : null;
  //}

  static bool GuessLoop(string prefix)
  {
    // 根据常见关键字判断是否循环
    foreach (var kv in loopRule)
      if (prefix.Contains(kv.Key)) return kv.Value;

    // 默认：移动/待机循环，其他不循环
    if (prefix.Contains("move") || prefix.Contains("walk") || prefix.Contains("run") || prefix.Contains("idle") || prefix.Contains("stand"))
      return true;
    return false;
  }

  static string GetPrefix(string spriteName)
  {
    // take prefix before _：idle_0 -> idle
    var idx = spriteName.IndexOf('_');
    if (idx <= 0) return null;
    return spriteName.Substring(0, idx).ToLowerInvariant();
  }

  class NaturalNameComparer : IComparer<string>, IComparer<object>, IComparer
  {
    public int Compare(string x, string y) => EditorUtility.NaturalCompare(x, y);
    public int Compare(object x, object y) => Compare(x?.ToString(), y?.ToString());
    int IComparer.Compare(object x, object y) => Compare(x, y);
  }
}
