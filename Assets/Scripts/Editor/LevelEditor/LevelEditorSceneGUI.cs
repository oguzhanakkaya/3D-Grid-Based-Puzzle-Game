using Code.LevelEditor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

[InitializeOnLoad]
public class LevelEditorSceneGUI : Editor
{
    private const int CLEARTILE = 0;
    private const int OBSTACLE = 1;
    private const int SHORTBUS = 2;
    private const int LONGBUS = 3;
    private const int NORMALMODE = 4;
    private const int GROUNDOBSTACLE = 5;
    public const int toolbarHeight = 45;
    private static bool canPaint;
    private static readonly string[] buttons = { "ClearTile", "Obstacle", "ShortBus", "LongBus","NormalMode","GroundObst" };
    private static GameObject currentObject;
    private static GameObject lastObject;

    private static int roadLayer = 1 << 8;

    static LevelEditorSceneGUI()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
        EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;
        EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
    }

    public static int SelectedTool
    {
        get => EditorPrefs.GetInt("LevelEditorSelectedTool", 0);
        set
        {
            if (value == SelectedTool) return;
            EditorPrefs.SetInt("LevelEditorSelectedTool", value);


            switch (value)
            {
                case CLEARTILE:
                    Tools.hidden = true;
                    break;
                case OBSTACLE:
                    Tools.hidden = true;
                    break;
                case SHORTBUS:
                    Tools.hidden = true;
                  //  LevelEditorPaintItemsUI.levelItemsGroup = true;
                    break;
                case LONGBUS:
                    Tools.hidden = true;
                    break;
                case GROUNDOBSTACLE:
                    Tools.hidden = true;
                    break;
                case NORMALMODE:
                    Tools.hidden = false;
                    break;
            }
        }
    }


    private static void OnSceneGUI(SceneView sceneView)
    {
        if (IsValid())
        {
            Tools.hidden = SelectedTool != NORMALMODE;
        }
        else
        {
            Tools.hidden = false;
            Init();
            return;
        }

        if (Application.isPlaying)
            return;

        DrawToolsMenu(sceneView.position);
        DrawHandle(sceneView);
        HandleMouseInput();
        HandleKeyboardInput();
        // sceneView.Repaint();


    }

    private static void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (IsValid())
        {
            Tools.hidden = SelectedTool != CLEARTILE;
            Init();
        }
        else
        {
            Tools.hidden = false;
        }
    }

    public static void Init()
    {
        currentObject = null;
        lastObject = null;
        canPaint = false;
    }


    private static void DrawHandle(SceneView sceneView)
    {
        LevelEditorSetColorItemsUI.DisablePreviewItem();
        DrawSelectHandle(sceneView);
    }


    private static void DrawPaintHandle(SceneView sceneView)
    {
        var pos = Event.current.mousePosition;
        var ray = HandleUtility.GUIPointToWorldRay(pos);
        if (Physics.Raycast(ray, out var hit, 1000f, roadLayer))
        {
            canPaint = true;
            sceneView.Repaint();
        }
        else
        {
            canPaint = false;
            LevelEditorSetColorItemsUI.DisablePreviewItem();
        }
    }

    private static void DrawSelectHandle(SceneView sceneView)
    {
        var defaultMatrix = Handles.matrix;
        var pos = Event.current.mousePosition;
        var ray = HandleUtility.GUIPointToWorldRay(pos);
        var mask = ~roadLayer;

        if (Event.current.control)
            mask = roadLayer;

        if (Physics.Raycast(ray, out var hit, 1000f, mask))
        {
            //Handles.color = Color.red;
            //var rotationMatrix = Matrix4x4.TRS(hit.transform.position, hit.transform.rotation,
            //    hit.transform.lossyScale);
            //Handles.matrix = rotationMatrix;
            //Handles.DrawWireCube(Vector3.zero, Vector3.one);
            currentObject = hit.transform.gameObject;
            sceneView.Repaint();
        }
        else
        {
            currentObject = null;
        }
    }

    private static void HandleKeyboardInput()
    {
        var controlID = GUIUtility.GetControlID(FocusType.Keyboard);
        if (Event.current.type == EventType.KeyDown)
        {
            if (SelectedTool == OBSTACLE)
            {
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    currentObject = null;
                    Selection.activeGameObject = null;
                }
            }


            if (Event.current.keyCode == KeyCode.Alpha1)
            {
                SelectedTool = CLEARTILE;
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.Alpha2)
            {
                SelectedTool = OBSTACLE;
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.Alpha3)
            {
                SelectedTool = SHORTBUS;
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.Alpha4)
            {
                SelectedTool = LONGBUS;
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.Alpha5)
            {
                SelectedTool = NORMALMODE;
                Event.current.Use();
            }
        }
        else if (Event.current.type == EventType.KeyUp)
        {
            if (Event.current.keyCode == KeyCode.Alpha1 ||
                Event.current.keyCode == KeyCode.Alpha2 ||
                Event.current.keyCode == KeyCode.Alpha3 ||
                Event.current.keyCode == KeyCode.Alpha4)
                Event.current.Use();
        }

        HandleUtility.AddDefaultControl(controlID);
    }

    private static void HandleMouseInput()
    {

        var cId = GUIUtility.GetControlID(FocusType.Passive);
        if (Event.current.type == EventType.MouseDown &&
            Event.current.button == 0)
        {
            if (SelectedTool == SHORTBUS)
            {

                lastObject = currentObject;
                if (currentObject.GetComponent<Tile>())
                {
                    GameObject.FindObjectOfType<LevelEditorManager>().CreateShortBus(currentObject.GetComponent<Tile>().x,
                        currentObject.GetComponent<Tile>().y);
                }
            }
            else if (SelectedTool == OBSTACLE)
            {
                lastObject = currentObject;

                if (currentObject.GetComponent<Tile>())
                {
                    Selection.activeGameObject = currentObject;


                    GameObject.FindObjectOfType<LevelEditorManager>().CreateObstacle(currentObject.GetComponent<Tile>().x,
                        currentObject.GetComponent<Tile>().y);
                }

            }
            else if (SelectedTool == LONGBUS)
            {
                if (currentObject.GetComponent<Tile>())
                {
                    GameObject.FindObjectOfType<LevelEditorManager>().CreateLongBus(currentObject.GetComponent<Tile>().x,
                         currentObject.GetComponent<Tile>().y);
                    // Undo.RegisterFullObjectHierarchyUndo(currentObject, "Delete level item");
                    // DestroyImmediate(currentObject);
                }
            }
            else if (SelectedTool == GROUNDOBSTACLE)
            {
                if (currentObject.GetComponent<Tile>())
                {
                    GameObject.FindObjectOfType<LevelEditorManager>().CreateGroundObstacle(currentObject.GetComponent<Tile>().x,
                         currentObject.GetComponent<Tile>().y);
                    // Undo.RegisterFullObjectHierarchyUndo(currentObject, "Delete level item");
                    // DestroyImmediate(currentObject);
                }
            }
            else if (SelectedTool == CLEARTILE)
            {
               
                if (currentObject.GetComponent<Bus>())
                {
                    var node = currentObject.GetComponent<Bus>().GetCurrentNode();

                    if (node.tileType==TileType.Bus)
                    {
                        if (node.currentBus is ShortBus)
                        {
                            DestroyImmediate(node.currentBus.gameObject);
                            node.currentBus = null;
                            node.tileType = TileType.Empty;
                        }
                        else
                        {
                            var nextNode = GridManager.GetNextNodeForLongBus(node,node.currentBus.direction);

                            DestroyImmediate(node.currentBus.gameObject);
                            node.currentBus = null;
                            node.tileType = TileType.Empty;

                            nextNode.currentBus = null;
                            nextNode.tileType = TileType.Empty;
                        }
                    }
                }
                else if (currentObject.GetComponent<Obstacle>())
                {
                    var node = GridManager.GetNode(currentObject.GetComponent<Obstacle>().x, currentObject.GetComponent<Obstacle>().y);

                    DestroyImmediate(currentObject.gameObject);
                    node.currentBus = null;
                    node.tileType = TileType.Empty;

                }
            }
            else
            {
                lastObject = currentObject;
            }

        }
        HandleUtility.AddDefaultControl(cId);
    }

    private static void DrawToolsMenu(Rect position)
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(0, position.height - toolbarHeight, position.width, 20), EditorStyles.toolbar);
        SelectedTool =
            GUILayout.SelectionGrid(SelectedTool, buttons, 6, EditorStyles.toolbarButton, GUILayout.Width(325));
        GUILayout.EndArea();

        Handles.EndGUI();
    }


    public static bool IsValid()
    {
        return SceneManager.GetActiveScene().name == "GameScene";
    }

}