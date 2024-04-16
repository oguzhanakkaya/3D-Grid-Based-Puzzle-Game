namespace Code.Editor
{
    public interface IEditorWindow
    {
        void Init(FTFEditorWindow ftfEditor);
        void OnEnable();
        void OnDisable();
        void OnGUI();
    }
}