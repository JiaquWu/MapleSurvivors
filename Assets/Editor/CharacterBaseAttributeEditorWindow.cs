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
            if (GUILayout.Button("��ѡ���ʲ���ȡ", GUILayout.Height(24)))
            {
                TryLoadFromSelection();
            }

            _includeZero = GUILayout.Toggle(_includeZero, "������� 0 ����", "Button", GUILayout.Height(24));
        }

        EditorGUILayout.Space();

        // ��ֵ��
        EditorGUILayout.LabelField("�������ԣ�StatId �� Value��", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("��дÿ����ֵ���ɵ������ѡ���ʲ���ȡ�����ٱ༭�����ʲ���", MessageType.Info);

        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        EditorGUI.indentLevel++;
        foreach (StatId id in Enum.GetValues(typeof(StatId)))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(id.ToString(), GUILayout.Width(180));
                _values[id] = EditorGUILayout.FloatField(_values[id]);
                if (GUILayout.Button("����", GUILayout.Width(60)))
                    _values[id] = 0f;
            }
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndScrollView();

        GUILayout.FlexibleSpace();
        EditorGUILayout.Space();

        // ������
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("�½��ʲ�...", GUILayout.Height(30)))
            {
                CreateNewAsset();
            }

            using (new EditorGUI.DisabledScope(!(Selection.activeObject is CharacterBaseAttribute)))
            {
                if (GUILayout.Button("���µ�ǰѡ���ʲ�", GUILayout.Height(30)))
                {
                    var asset = Selection.activeObject as CharacterBaseAttribute;
                    if (asset != null) UpdateAsset(asset);
                }
            }
        }

        EditorGUILayout.Space();
    }

    // ���� ����ʵ�� ���� //

    private void TryLoadFromSelection()
    {
        var asset = Selection.activeObject as CharacterBaseAttribute;
        if (asset == null)
        {
            EditorUtility.DisplayDialog("��ʾ", "��ѡ��һ�� CharacterBaseAttribute �ʲ���", "�õ�");
            return;
        }

        // �� SerializedObject ��ȡ˽���ֶ� baseStats
        var so = new SerializedObject(asset);
        var listProp = so.FindProperty("baseStats");
        if (listProp == null || !listProp.isArray)
        {
            EditorUtility.DisplayDialog("����", "δ�ҵ� baseStats �б�", "�õ�");
            return;
        }

        // ��ղ�������� UI ֵ
        foreach (var key in new List<StatId>(_values.Keys))
            _values[key] = 0f;

        for (int i = 0; i < listProp.arraySize; i++)
        {
            var elem = listProp.GetArrayElementAtIndex(i);
            var idProp = elem.FindPropertyRelative("Id");
            var valProp = elem.FindPropertyRelative("Value");

            var id = (StatId)idProp.enumValueIndex;  // enum ���������
            var val = valProp.floatValue;

            if (_values.ContainsKey(id)) _values[id] = val;
            else _values.Add(id, val);
        }

        Repaint();
        ShowNotification(new GUIContent("�Ѵ�ѡ���ʲ���ȡ"));
    }

    private void CreateNewAsset()
    {
        var path = EditorUtility.SaveFilePanelInProject(
            "���� CharacterBaseAttribute",
            "CharacterBaseAttribute",
            "asset",
            "ѡ�񱣴�λ��");

        if (string.IsNullOrEmpty(path)) return;

        var asset = ScriptableObject.CreateInstance<CharacterBaseAttribute>();
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        UpdateAsset(asset);
        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);
        ShowNotification(new GUIContent("�Ѵ�����д��"));
    }

    private void UpdateAsset(CharacterBaseAttribute asset)
    {
        var so = new SerializedObject(asset);
        var listProp = so.FindProperty("baseStats");
        if (listProp == null || !listProp.isArray)
        {
            EditorUtility.DisplayDialog("����", "δ�ҵ� baseStats �б�", "�õ�");
            return;
        }

        // �����
        listProp.ClearArray();

        // д�룺ֻд���㣨����ѡ�С�������� 0 �����
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

        ShowNotification(new GUIContent("�ʲ��Ѹ���"));
    }
}
#endif