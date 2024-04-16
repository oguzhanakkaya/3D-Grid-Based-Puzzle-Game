using Code.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.LevelEditor
{
    public class LevelEditorWindow : IEditorWindow
    {
        private FTFEditorWindow ftfEditor;
        public const string LEVELS_PATH = "Assets/Resources/Data/Levels/";


        public const int GUIAreaWidth = 300;


        public static string DATA_PATH = "Prefabs/InGame/";

        public LevelData[] levelDatas;
        private LevelEditorSetColorItemsUI _levelEditorPaintItemsUI;

        public Dictionary<string, LevelItemButton> itemPaintToggles = new Dictionary<string, LevelItemButton>();
        public Dictionary<string, LevelItemButton> roadButtons = new Dictionary<string, LevelItemButton>();

        private LevelEditorSetTileUI _levelEditorRoadsUI;

        //public Dictionary<string, GameObject> itemPaintPrefabs = new Dictionary<string, GameObject>();
        //public Dictionary<string, LevelItemButton> itemPaintToggles = new();
        //public Dictionary<string, LevelItemButton> itemPaintTogglesFiltered = new();

        //internal List<ILevelEditorPaintable> levelItemPrefabs = new();

        private string[] patternNames;

        private Vector2 scrollPosition = Vector2.zero;

        private int selectedPatternIndex;

        public static GameObject activePaintLevelEditorItem;

        public static GameObject levelArea => GameObject.Find("Tiles");
        public GameObject tempArea => GameObject.Find("TempArea");

        private string currentPatternName
        {
            get => EditorPrefs.GetString("LevelEditorCurrentPatternName", "");

            set => EditorPrefs.SetString("LevelEditorCurrentPatternName", value);
        }

        public void OnEnable()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        public void OnDisable()
        {
            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
            if (IsLevelEditorScene())
                ClearPattern();
        }

        public void OnGUI()
        {
            if (!IsLevelEditorScene())
                return;
            else if (_levelEditorPaintItemsUI == null)
                Init(ftfEditor);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            if (GUILayout.Button("Reload Editor"))
            {
                Init(ftfEditor);
                LevelEditorSceneGUI.Init();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();


            DrawLevelControls();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            _levelEditorRoadsUI.DrawSetTileUI();
           
            EditorGUILayout.Space();

            //LevelEditorHelpersUI.DrawHelpers();
            //EditorGUILayout.Space();

            //LevelEditorTestingDataUI.DrawTestingPrefs();
            //EditorGUILayout.Space();
          //  _levelEditorPaintItemsUI.DrawBasePaintUI();

            EditorGUILayout.EndScrollView();
        }

        private void PlayModeStateChanged(PlayModeStateChange p)
        {
            if (!IsLevelEditorScene())
                return;


            if (p == PlayModeStateChange.ExitingEditMode)
            {
                // save test pattern data
                //SavePattern("test", EDITOR_TEMP_PATH);
                ClearPattern();
            }
            else if (p == PlayModeStateChange.EnteredEditMode)
            {
                //LoadPattern("test", EDITOR_TEMP_PATH, true);
            }
        }

        public void Init(FTFEditorWindow ftfEditor)
        {
            this.ftfEditor = ftfEditor;
            if (!IsLevelEditorScene())
                return;

            _levelEditorPaintItemsUI = new LevelEditorSetColorItemsUI(this, ftfEditor);
            _levelEditorRoadsUI = new LevelEditorSetTileUI(this, ftfEditor);

            //LevelEditorPaintItemsUI.Init();
            ClearTempArea();
            //LoadLevelItems();
            LoadPatterns();
            LoadButtons();

            Selection.activeGameObject = levelArea;
         //   SceneView.FrameLastActiveSceneView();
            //LevelEditorTestingDataUI.Init();
            //LevelEditorHelpersUI.Init(this);
        }

        private void LoadButtons()
        {
            roadButtons.Clear();
          //  LoadItemButtonFromPath(itemPaintToggles, null, "Assets/" + DATA_PATH, btn => { }, x => x.GetComponent<TransformDataCapture>());
            LoadItemButtonFromPath(roadButtons, null, "Assets/" + DATA_PATH + "Floors/", btn =>
            {
               // AddFloor(btn.prefab);
            });
        }
        private void CreateTile()
        {

        }

        private void DrawLevelControls()
        {
            EditorGUILayout.Space();
            selectedPatternIndex = EditorGUILayout.Popup(selectedPatternIndex, patternNames);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Level Save Name");
            currentPatternName = EditorGUILayout.TextField(currentPatternName);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load"))
                if (selectedPatternIndex > 0)
                {
                    LoadPattern(patternNames[selectedPatternIndex]);
                    selectedPatternIndex = 0;
                }

            if (GUILayout.Button("Save"))
            {
                if (currentPatternName.Trim().Length > 0)
                {
                    SavePattern(currentPatternName.Trim());
                   // LoadPatterns();
                }
                else
                {
                    if (currentPatternName.Trim().Length == 0)
                        EditorUtility.DisplayDialog("Can't Save", "Please enter a name", "Ok");
                    //else if (CurrentPlatform == null)
                    //    EditorUtility.DisplayDialog("Can't Save", "Please select platform", "Ok");
                    //else if (CurrentBasket == null)
                    //    EditorUtility.DisplayDialog("Can't Save", "Please select basket", "Ok");
                    //else if (ActiveBackgroundPrefab == null)
                    //    EditorUtility.DisplayDialog("Can't Save", "Please select background", "Ok");
                }
            }

            if (GUILayout.Button("Clear")) ClearPattern();

            EditorGUILayout.EndHorizontal();
        }

        public void SavePattern(string name, string path = LEVELS_PATH)
        {

            LevelData asset = null;

            var levelManager = GameObject.FindObjectOfType<LevelManager>();

            var levelDatas = LoadAllLevelData<LevelData>(path);
            for (var i = 0; i < levelDatas.Length; i++)
                if (levelDatas[i].name.StartsWith(name))
                {
                    asset = levelDatas[i];
                    break;
                }

            var newObject = false;
            if (asset == null)
            {
                newObject = true;
                asset = ScriptableObject.CreateInstance<LevelData>();
            }


            asset.graphWidth = GridManager.gridWidth;
            asset.graphHeight = GridManager.gridHeight;


            asset.cameraPos = Camera.main.transform.position;
            asset.cameraAngle = Camera.main.transform.localEulerAngles;
            asset.cameraSize = Camera.main.orthographicSize;

            asset.Obstacles.Clear();
            //asset.passangers.Clear();
            asset.Items.Clear();


            foreach (Transform item in levelArea.transform)
            {
                if (item.GetComponent<Obstacle>()!=null)
                {
                    asset.Obstacles.Add(new ObstacleSpecs(new Vector2(item.GetComponent<Obstacle>().x, item.GetComponent<Obstacle>().y),item.GetComponent<Obstacle>().groundObstacle));
                }
                else if (item.GetComponent<Bus>() != null)
                {
                    Bus bus = item.GetComponent<Bus>();
                    if (bus is ShortBus)
                    {
                        asset.Items.Add(new ItemSpecs(bus.busController.currentNode.Position,bus.numberOfPassanger,bus.direction,BusType.Short,bus.color,bus.isStaticBus));
                    }
                    else if (item.GetComponent<Bus>() is LongBus)
                    {
                        asset.Items.Add(new ItemSpecs(bus.busController.currentNode.Position, bus.numberOfPassanger, bus.direction, BusType.Long,bus.color, bus.isStaticBus));
                    }
                }
            }

            if (newObject)
            {
                AssetDatabase.CreateAsset(asset, path + GetName(name) + ".asset");
            }
            else
            {
                var assetPath = AssetDatabase.GetAssetPath(asset.GetInstanceID());
                AssetDatabase.RenameAsset(assetPath, GetName(name));
            }
            EditorUtility.SetDirty(asset);

            Selection.activeObject = asset;

            AssetDatabase.SaveAssets();

        }

        public void LoadPattern(string name, string path = LEVELS_PATH)
        {
            if (levelArea.transform.childCount > 0)
                if (!EditorUtility.DisplayDialog("Warning",
                        "Loading a pattern will overwrite the unsaved work. Are you sure to do this?",
                        "yes, clear and load", "no, I'm not finished yet."))
                    return;
            ClearPattern();
            var data = LoadPatternData<LevelData>(name, path);
            if (data == null)
            {
                Debug.LogError("couldn't load the pattern data for " + name);
                return;
            }
            else
            {
                GameObject.FindObjectOfType<LevelEditorManager>().LoadData(data);
            }
           // 

//            Undo.RegisterCreatedObjectUndo(levelArea, "load pattern");

            EditorUtility.SetDirty(levelArea);
            currentPatternName = name;
        }

        public T LoadPatternData<T>(string name, string path) where T : ScriptableObject
        {
            T asset = null;
            var guids = AssetDatabase.FindAssets(name, new[] { path });
            if (guids.Length > 0)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
         
            return asset;
        }

        public static T[] LoadAllLevelData<T>(string path) where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets("", new[] { path });
            var assets = new List<T>();
            if (guids.Length > 0)
                for (var i = 0; i < guids.Length; i++)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                    T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    if (asset != null)
                    {
                        assets.Add(asset);
                    }
                }

            return assets.ToArray();
        }

        public static GameObject AddItem(GameObject prefab, Vector3 placePoint, Quaternion rotation)
        {
            var item = GameObject.Instantiate(prefab, levelArea.transform);
            item.transform.rotation = rotation;
            item.transform.position = placePoint;

            return item;
        }

        public void ClearPattern()
        {
            
            if (!EditorApplication.isPlaying)
            {
                Undo.RegisterFullObjectHierarchyUndo(levelArea, "Clear Pattern");
                for (var i = levelArea.transform.childCount - 1; i >= 0; i--)
                    GameObject.DestroyImmediate(levelArea.transform.GetChild(i).gameObject);
                EditorUtility.SetDirty(levelArea);
                currentPatternName = "";
                GameObject.FindObjectOfType<LevelEditorManager>().ClearAllItem();

            }
           

        }

        public void ClearTempArea()
        {
          //  for (var i = tempArea.transform.childCount - 1; i >= 0; i--)
          //      GameObject.DestroyImmediate(tempArea.transform.GetChild(i).gameObject);
        }


        private string GetName(string n)
        {
            return n;
        }

        private void LoadPatterns()
        {
            levelDatas = LoadAllLevelData<LevelData>(LEVELS_PATH);
            patternNames = levelDatas.Select(x => x.name).Prepend("None").ToArray();
        }

        /*
        public void AddFloor(GameObject prefab)
        {
            Road piecePrefab = prefab.GetComponent<Road>();
            LevelEnd levelEnd = prefab.GetComponent<LevelEnd>();
            if (piecePrefab || levelEnd)
            {
                var floorPieces = levelArea.GetComponentsInChildren<DataCapturePrefabInfo>().Select(y => y.GetComponent<Road>()).ToArray();
                Transform endPoint;
                if (floorPieces.Length > 0)
                {
                    List<Road> roads = new List<Road>(floorPieces);

                    Road road = floorPieces.OrderBy(x => Mathf.Abs(x.transform.position.magnitude)).First();
                    roads.Remove(road);
                    while (roads.Count > 0)
                    {
                        road = floorPieces.OrderBy(x => Vector3.Distance(road.EndPoint.position, x.StartPoint.position)).First();
                        roads.Remove(road);
                    }

                    endPoint = road.EndPoint;
                }
                else
                {
                    endPoint = levelArea.transform;
                }

                var instance = GameObject.Instantiate(prefab, levelArea.transform);
                Undo.RegisterCreatedObjectUndo(instance, "Create Floor Piece");

                if (piecePrefab)
                {
                    instance.GetComponent<Road>().MergePiece(endPoint);
                }
                else
                {
                    instance.transform.MergePiece(endPoint);
                }

            }
        }
        */

        //private List<ILevelEditorPaintable> LoadLevelItemPrefabsAt(string resourcePath)
        //{
        //    var paintables = new List<ILevelEditorPaintable>();
        //    var assets = AssetDatabase.FindAssets("", new[] { resourcePath });
        //    foreach (var guid in assets)
        //    {
        //        var path = AssetDatabase.GUIDToAssetPath(guid);
        //        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        //        if (prefab == null)
        //            continue;
        //        if(prefab.GetComponent<ILevelEditorPaintable>() == null)
        //        {
        //            Debug.LogError("Level editor paintable not found on " + prefab.name + " at " + path);
        //        }
        //        else {
        //            paintables.Add(prefab.GetComponent<ILevelEditorPaintable>());
        //        }
        //    }

        //    return paintables;
        //}

        //public void LoadLevelItems()
        //{
        //    levelItemPrefabs.AddRange(LoadLevelItemPrefabsAt("Assets/Prefabs/" + RUNNER_DATA_PATH + "Floors"));
        //    levelItemPrefabs.AddRange(LoadLevelItemPrefabsAt("Assets/Prefabs/" + RUNNER_DATA_PATH + "Obstacles"));
        //    levelItemPrefabs.AddRange(LoadLevelItemPrefabsAt("Assets/Prefabs/" + RUNNER_DATA_PATH + "Collectables"));
        //    levelItemPrefabs.AddRange(LoadLevelItemPrefabsAt("Assets/Prefabs/" + RUNNER_DATA_PATH + "Triggers"));
        //    levelItemPrefabs.AddRange(LoadLevelItemPrefabsAt("Assets/Prefabs/" + RUNNER_DATA_PATH + "Eggs"));
        //    levelItemPrefabs.AddRange(LoadLevelItemPrefabsAt("Assets/Prefabs/" + RUNNER_DATA_PATH + "Others"));
        //    //levelItemPrefabs.AddRange(LoadLevelItemPrefabsAt("Runner/Environment"));

        //    var presetNames = AssetDatabase.FindAssets("", new[] { "Assets/Prefabs/Runner/Presets/" });
        //    foreach (var guid in presetNames)
        //    {
        //        var path = AssetDatabase.GUIDToAssetPath(guid);
        //        var preset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        //        if (preset)
        //        {
        //            var i = preset.GetComponent<ILevelEditorPaintable>();
        //            if (i != null)
        //                levelItemPrefabs.Add(i);
        //        }
        //    }
        //}


        private Texture GetPrefabPreview(GameObject prefab, string path)
        {
            var tex2D = new Texture2D(96, 96);
            try
            {
                Texture2D texture2D = AssetPreview.GetAssetPreview(prefab);
                if(texture2D != null)
                    tex2D = texture2D;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return tex2D;
        }

        private void LoadItemButtonFromPath(Dictionary<string, LevelItemButton> saveDict,
            Dictionary<string, GameObject> prefabDict, string resourcePath, Action<LevelItemButton> callback, Predicate<GameObject> condition = null)
        {
            var assets = AssetDatabase.FindAssets("", new[] { resourcePath });

            foreach (var guid in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                    continue;
                if (condition != null && !condition(prefab))
                    continue;

                //var item = prefab.GetComponent<LevelEditorItem>();
                var key = prefab.name;
                var preview = GetPrefabPreview(prefab, path);
                // preview.Resize(64, (int)(64 * ((float)preview.width / preview.height)));
                if (prefabDict != null) prefabDict[key] = prefab;

                saveDict[key] = new LevelItemButton
                {
                    name = key,
                    preview = preview,
                    prefab = prefab,
                    //prefabID = item.PrefabID,
                    callback = callback
                };
            }
        }

        private LevelItemButton LoadItemButtonFromPath(Dictionary<string, GameObject> prefabDict, string resourcePath, Action<LevelItemButton> callback, Predicate<GameObject> condition = null)
        {
            var assets = AssetDatabase.FindAssets("", new[] { resourcePath });

            foreach (var guid in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                    continue;
                if (condition != null && !condition(prefab))
                    continue;

                //var item = prefab.GetComponent<LevelEditorItem>();
                var key = prefab.name;
                var preview = GetPrefabPreview(prefab, path);
                // preview.Resize(64, (int)(64 * ((float)preview.width / preview.height)));
                if (prefabDict != null) prefabDict[key] = prefab;

                return new LevelItemButton
                {
                    name = key,
                    preview = preview,
                    prefab = prefab,
                    //prefabID = item.PrefabID,
                    callback = callback
                };
            }

            return null;
        }

        //[MenuItem("Tools/Help")]
        //public static void Help()
        //{
        //    LevelData[] levels = LoadAllLevelData<LevelData>(LEVELS_PATH);
        //    for (int i = 0; i < levels.Length; i++)
        //    {
        //        bool hasKey = false;
        //        for (int j = 0; j < levels[i].BasketObjects.Count; j++)
        //        {
        //            if (levels[i].BasketObjects[j].hasKey)
        //            {
        //                hasKey = true;
        //                break;
        //            }
        //        }
        //        if (hasKey)
        //        {
        //            bool lockedExist = false;
        //            for (int j = 0; j < levels[i].BoxDatas.Count; j++)
        //            {
        //                if (levels[i].BoxDatas[j].lockState == LockState.FindKey)
        //                {
        //                    lockedExist = true;
        //                    break;
        //                }
        //            }

        //            if (!lockedExist)
        //            {
        //                Debug.Log(levels[i].name);
        //            }
        //        }
        //    }
        //}

        public static bool IsLevelEditorScene()
        {
         //   return false;
            return PrefabStageUtility.GetCurrentPrefabStage() == null /* &&
           //        SceneManager.GetActiveScene().name == "LevelEditor"*/;
        }
    }

    public class LevelItemButton
    {
        public Action<LevelItemButton> callback;
        public string name;
        public GameObject prefab;
        public Texture preview;

        public bool selected;
    }
}