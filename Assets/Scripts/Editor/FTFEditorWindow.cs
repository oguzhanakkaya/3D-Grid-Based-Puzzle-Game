using Code.Editor;
using Code.LevelEditor;
using System;
using UnityEditor;
using UnityEngine;

public class FTFEditorWindow : EditorWindow
{
    public static FTFEditorWindow Instance;

    private LevelEditorWindow levelEditorWindow;

    private IEditorWindow activeWindow;
    private static StartWindowType startWindowType;

    private int tabIndex;
    private string[] tabNames = new string[] { "Level Editor", "New Tab", "New Tab 2" };

    public void OnEnable()
    {
        Instance = this;
        if (levelEditorWindow != null)
            levelEditorWindow.OnDisable();


        switch (startWindowType)
        {
            case StartWindowType.LevelEditor:
                InitLevelEditor();
                break;
            default:
                break;
        }

        if (activeWindow != null)
        {
            activeWindow.OnEnable();
        }
    }

    private void InitLevelEditor()
    {
        if (levelEditorWindow == null)
        {
            levelEditorWindow = new LevelEditorWindow();
            levelEditorWindow.Init(this);
        }
        activeWindow = levelEditorWindow;
        tabIndex = (int)StartWindowType.LevelEditor;
    }

    private void OnDisable()
    {
        if (activeWindow != null)
        {
            activeWindow.OnDisable();
        }
    }

    private void OnGUI()
    {
        if (activeWindow != null)
        {
            activeWindow.OnGUI();
        }

        GUILayout.FlexibleSpace();
        int newTab = GUILayout.Toolbar(tabIndex, tabNames);

        if (tabIndex != newTab)
        {
            StartWindowType windowType = (StartWindowType)newTab;
            if (windowType == StartWindowType.LevelEditor)
            {
                InitLevelEditor();
            }
        }
    }

    [MenuItem("Tools/Level Editor", priority = -1)]
    public static void CreateWindowSetLevelEditor()
    {
        startWindowType = StartWindowType.LevelEditor;
        var window = (FTFEditorWindow)GetWindow(typeof(FTFEditorWindow), false, "Editor Tools");
        window.Show();
    }

    private enum StartWindowType 
    {
        LevelEditor = 0,
    }
}
