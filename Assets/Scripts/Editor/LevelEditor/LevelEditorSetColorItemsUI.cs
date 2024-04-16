using System;
using System.Collections.Generic;
using Code.LevelEditor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class LevelEditorSetColorItemsUI
{
    public static LevelEditorSetColorItemsUI Instance;
    public static GameObject previewItem;

    public static bool levelItemsGroup;

    private readonly LevelEditorWindow _levelEditorWindow;

    private FTFEditorWindow _ftfEditor;

    public LevelEditorSetColorItemsUI(LevelEditorWindow levelEditorWindow, FTFEditorWindow ftfEditor)
    {
        _ftfEditor = ftfEditor;
        _levelEditorWindow = levelEditorWindow;
        Instance = this;
    }

    public static void Init()
    {
        if (previewItem != null)
            Object.DestroyImmediate(previewItem.gameObject);
        previewItem = null;
        LevelEditorWindow.activePaintLevelEditorItem = null;
    }

    public void DrawBasePaintUI()
    {
        levelItemsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(levelItemsGroup, "Set Color");
        if (levelItemsGroup)
        {
            EditorGUI.indentLevel++;
            DrawLevelItems();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }
  

    private void DrawLevelItems()
    {
       /* var itemWidth = 100; // Mathf.FloorToInt((_levelEditorWindow.position.width - 40) / itemPerRow);
        var itemPerRow = Math.Max(1, Mathf.FloorToInt((_ftfEditor.position.width - 40) / itemWidth));
        var c = 0;*/
        EditorGUILayout.BeginVertical();

      //  EditorGUILayout.PropertyField(ColorSpecs, new GUIContent("Vector Object"));
        /* foreach (var k in _levelEditorWindow.itemPaintToggles.Keys)
         {
             if (c % itemPerRow == 0)
             {
                 if (c > 0)
                 {
                     EditorGUILayout.EndHorizontal();
                     EditorGUILayout.Space();
                 }

                 EditorGUILayout.BeginHorizontal();
             }

             DrawLevelItemToggle(k, itemWidth);
             c++;
         }

         if (c > 0) EditorGUILayout.EndHorizontal();*/
        EditorGUILayout.EndVertical();
    }

    private void DrawLevelItemToggle(string k, int width)
    {
        var p = _levelEditorWindow.itemPaintToggles[k];
        EditorGUILayout.BeginVertical();
        var g = new GUIContent(p.preview);
        g.tooltip = p.name;
        GUILayout.Label(p.name, GUILayout.MaxWidth(width));

        var active = LevelEditorWindow.activePaintLevelEditorItem != null && LevelEditorWindow.activePaintLevelEditorItem == p.prefab;

        var isToggleDown =
            GUILayout.Toggle(active, g, GUI.skin.button, GUILayout.Width(width), GUILayout.Height(width));

        if (isToggleDown && !active)
        {
            LevelEditorWindow.activePaintLevelEditorItem = p.prefab;
            CreatePreviewItem();
        }

        EditorGUILayout.EndVertical();
    }

    public static void DisablePreviewItem()
    {
        if (previewItem != null)
            previewItem.gameObject.SetActive(false);
    }

    private void CreatePreviewItem()
    {
        if (previewItem != null) Object.DestroyImmediate(previewItem.gameObject);
        if (LevelEditorWindow.activePaintLevelEditorItem != null)
        {
            var i = Object.Instantiate(LevelEditorWindow.activePaintLevelEditorItem.gameObject,
                _levelEditorWindow.tempArea.transform);
            previewItem = i;
            var colliders = i.GetComponentsInChildren<Collider>();
            for (var j = 0; j < colliders.Length; j++) colliders[j].enabled = false;
            i.SetActive(false);
        }
    }
}