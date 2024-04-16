using System;
using Code.LevelEditor;
using UnityEditor;
using UnityEngine;

public class LevelEditorSetTileUI
{
    private static bool roadButtonsGroup;
    private readonly LevelEditorWindow _levelEditorWindow;
    private FTFEditorWindow _ftfEditor;

    private Vector2 tileSize;

    public LevelEditorSetTileUI(LevelEditorWindow levelEditorWindow, FTFEditorWindow ftfEditor)
    {
        _levelEditorWindow = levelEditorWindow;
        _ftfEditor = ftfEditor;
    }

    public void DrawSetTileUI()
    {
        roadButtonsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(roadButtonsGroup, "Set Tile Size");

        if (roadButtonsGroup)
        {
            EditorGUI.indentLevel++;
            DrawSetTileInside();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    public void DrawSetTileInside()
    {
        var itemWidth = 100; // Mathf.FloorToInt((_levelEditorWindow.position.width - 40) / itemPerRow);
        var itemPerRow = Math.Max(1, Mathf.FloorToInt((_ftfEditor.position.width - 40) / itemWidth));
        var c = 0;
        EditorGUILayout.BeginVertical();

        tileSize = EditorGUILayout.Vector2Field("Tile :", tileSize);
        if (GUILayout.Button("Create Tile Map"))
        {
            // tileSize = Selection.activeTransform.position;

            //      GameObject.FindObjectOfType<LevelEditorManager>().width = (int)tileSize.x;
            //      GameObject.FindObjectOfType<LevelEditorManager>().height = (int)tileSize.y;
          //  Debug.LogError((int)tileSize.x+" "+(int)tileSize.y);
            GameObject.FindObjectOfType<LevelEditorManager>().CreateTileMap((int)tileSize.x, (int)tileSize.y);
          //
        }
        if (GUILayout.Button("Camera Centeralize"))
        {
            float xPos = (0f + GridManager.GetNode(GridManager.gridWidth - 1, 0).worldPosition.x) / 2f;

            Camera.main.transform.position = new Vector3(xPos, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }

        /* foreach (var k in _levelEditorWindow.roadButtons.Keys)
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

             DrawRoadButton(k, itemWidth);

             c++;
         }

         if (c > 0) EditorGUILayout.EndHorizontal();*/
        EditorGUILayout.EndVertical();
    }

    private void DrawRoadButton(string k, int width)
    {
        var p = _levelEditorWindow.roadButtons[k];
        var g = new GUIContent(p.preview);
        g.tooltip = p.name;
        EditorGUILayout.BeginVertical();
        GUILayout.Label(p.name);
        if (GUILayout.Button(g, GUILayout.Width(width), GUILayout.Height(width))) p.callback(p);
        EditorGUILayout.EndVertical();
    }
}