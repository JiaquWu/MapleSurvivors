#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterBaseAttributeEditorWindow : EditorWindow
{
    // UI state
    private readonly Dictionary<StatId, float> _values = new Dictionary<StatId, float>();
    private Vector2 _scroll;
    private bool _includeZero = false;

    [MenuItem("Tools/Stats/Character Base Attribute Editor")]
    public static void Open()
    {
        var win = GetWindow<CharacterBaseAttributeEditorWindow>("Character Base Attribute");
        win.minSize = new Vector2(420, 520);
        win.InitIfNeeded();
        win.Show();
    }

    private void OnEnable() => InitIfNeeded();

    private void InitIfNeeded()
    {
        if (_values.Count > 0) return;
        foreach (StatId id in Enum.GetValues(typeof(StatId)))
        {
            if (!_values.ContainsKey(id)) _values.Add(id, 0f);
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("从选中资产读取", GUILayout.Height(24)))
            {
                TryLoadFromSelection();
            }

            _includeZero = GUILayout.Toggle(_includeZero, "保存包含 0 的项", "Button", GUILayout.Height(24));
        }

        EditorGUILayout.Space();

        // 数值表
        EditorGUILayout.LabelField("基础属性（StatId → Value）", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("填写每项数值。可点击“从选中资产读取”快速编辑已有资产。", MessageType.Info);

        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        EditorGUI.indentLevel++;
        foreach (StatId id in Enum.GetValues(typeof(StatId)))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(id.ToString(), GUILayout.Width(180));
                _values[id] = EditorGUILayout.FloatField(_values[id]);
                if (GUILayout.Button("重置", GUILayout.Width(60)))
                    _values[id] = 0f;
            }
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndScrollView();

        GUILayout.FlexibleSpace();
        EditorGUILayout.Space();

        // 操作区
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("新建资产...", GUILayout.Height(30)))
            {
                CreateNewAsset();
            }

            using (new EditorGUI.DisabledScope(!(Selection.activeObject is CharacterBaseAttribute)))
            {
                if (GUILayout.Button("更新当前选中资产", GUILayout.Height(30)))
                {
                    var asset = Selection.activeObject as CharacterBaseAttribute;
                    if (asset != null) UpdateAsset(asset);
                }
            }
        }

        EditorGUILayout.Space();
    }

    // —— 功能实现 —— //

    private void TryLoadFromSelection()
    {
        var asset = Selection.activeObject as CharacterBaseAttribute;
        if (asset == null)
        {
            EditorUtility.DisplayDialog("提示", "请选择一个 CharacterBaseAttribute 资产。", "好的");
            return;
        }

        // 用 SerializedObject 读取私有字段 baseStats
        var so = new SerializedObject(asset);
        var listProp = so.FindProperty("baseStats");
        if (listProp == null || !listProp.isArray)
        {
            EditorUtility.DisplayDialog("错误", "未找到 baseStats 列表。", "好的");
            return;
        }

        // 清空并重新填充 UI 值
        foreach (var key in new List<StatId>(_values.Keys))
            _values[key] = 0f;

        for (int i = 0; i < listProp.arraySize; i++)
        {
            var elem = listProp.GetArrayElementAtIndex(i);
            var idProp = elem.FindPropertyRelative("Id");
            var valProp = elem.FindPropertyRelative("Value");

            var id = (StatId)idProp.enumValueIndex;  // enum 存的是索引
            var val = valProp.floatValue;

            if (_values.ContainsKey(id)) _values[id] = val;
            else _values.Add(id, val);
        }

        Repaint();
        ShowNotification(new GUIContent("已从选中资产读取"));
    }

    private void CreateNewAsset()
    {
        var path = EditorUtility.SaveFilePanelInProject(
            "保存 CharacterBaseAttribute",
            "CharacterBaseAttribute",
            "asset",
            "选择保存位置");

        if (string.IsNullOrEmpty(path)) return;

        var asset = ScriptableObject.CreateInstance<CharacterBaseAttribute>();
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        UpdateAsset(asset);
        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);
        ShowNotification(new GUIContent("已创建并写入"));
    }

    private void UpdateAsset(CharacterBaseAttribute asset)
    {
        var so = new SerializedObject(asset);
        var listProp = so.FindProperty("baseStats");
        if (listProp == null || !listProp.isArray)
        {
            EditorUtility.DisplayDialog("错误", "未找到 baseStats 列表。", "好的");
            return;
        }

        // 先清空
        listProp.ClearArray();

        // 写入：只写非零（除非选中“保存包含 0 的项”）
        foreach (var kv in _values)
        {
            if (!_includeZero && Mathf.Approximately(kv.Value, 0f)) continue;

            int newIndex = listProp.arraySize;
            listProp.InsertArrayElementAtIndex(newIndex);
            var elem = listProp.GetArrayElementAtIndex(newIndex);

            var idProp = elem.FindPropertyRelative("Id");
            var valProp = elem.FindPropertyRelative("Value");

            idProp.enumValueIndex = (int)kv.Key;
            valProp.floatValue = kv.Value;
        }

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();

        ShowNotification(new GUIContent("资产已更新"));
    }
}
#endif