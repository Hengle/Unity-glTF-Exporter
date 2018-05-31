#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

/// <summary>
/// 将 Unity 的场景或者资源导出为 glTF 2.0 标准格式
/// </summary>
public class ExporterGLTF20 : EditorWindow {

    const int SPACE_SIZE = 5;
    // Fields limits
    const int NAME_LIMIT = 48;
    const int DESC_LIMIT = 1024;
    const int TAGS_LIMIT = 50;
    const int PASSWORD_LIMIT = 64;

    GameObject exporterGo;
    SceneToGlTFWiz exporter;

    Texture2D mBanner;
    string status = "";
    Vector2 descSize = new Vector2(512, 64);
    GUIStyle exporterTextArea;

    private bool opt_exportAnimation = true;
    private string param_name = "";
    private string param_description = "";
    private string param_tags = "";

    void Awake() {
        exporterGo = new GameObject("Exporter");
        exporter = exporterGo.AddComponent<SceneToGlTFWiz>();
        //FIXME: Make sure that object is deleted;
        exporterGo.hideFlags = HideFlags.HideAndDontSave;
    }

    void OnEnable() {
        mBanner = Resources.Load<Texture2D>("ExporterBanner");
        this.minSize = new Vector2(512, 320);
    }

    void OnSelectionChange() {
        updateExporterStatus();
        Repaint();
    }

    void OnGUI() {

        if (exporterTextArea == null) {
            exporterTextArea = new GUIStyle(GUI.skin.textArea);
            exporterTextArea.fixedWidth = descSize.x;
            exporterTextArea.fixedHeight = descSize.y;
        }

        // Model settings
        GUILayout.Label("Model properties", EditorStyles.boldLabel);

        // Model name
        GUILayout.Label("Name");
        param_name = EditorGUILayout.TextField(param_name);
        GUILayout.Label("(" + param_name.Length + "/" + NAME_LIMIT + ")", EditorStyles.centeredGreyMiniLabel);
        EditorStyles.textField.wordWrap = true;
        GUILayout.Space(SPACE_SIZE);

        GUILayout.Label("Description");
        param_description = EditorGUILayout.TextArea(param_description, exporterTextArea);
        GUILayout.Label("(" + param_description.Length + " / 1024)", EditorStyles.centeredGreyMiniLabel);
        GUILayout.Space(SPACE_SIZE);

        GUILayout.Label("Tags (separated by spaces)");
        param_tags = EditorGUILayout.TextField(param_tags);
        GUILayout.Label("'unity' and 'unity3D' added automatically (" + param_tags.Length + "/50)", EditorStyles.centeredGreyMiniLabel);
        GUILayout.Space(SPACE_SIZE);

        GUILayout.Label("Options", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        opt_exportAnimation = EditorGUILayout.Toggle("Export animation (beta)", opt_exportAnimation);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(SPACE_SIZE);

        bool enable = updateExporterStatus();

        //if (enable)
        //    GUI.color = Color.blue;
        //else
        //    GUI.color = Color.grey;

        GUI.enabled = enable;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(status, GUILayout.Width(220), GUILayout.Height(32))) {
            if (!enable) {
                EditorUtility.DisplayDialog("Error", status, "Ok");
            }
            else {

                //exporter.ExportCoroutine(exportPath, null, true, true, opt_exportAnimation, true);

            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Banner
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(mBanner);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private bool updateExporterStatus() {
        status = "";

        if (param_name.Length > NAME_LIMIT) {
            status = "Model name is too long";
            return false;
        }


        if (param_name.Length == 0) {
            status = "Please give a name to your model";
            return false;
        }


        if (param_description.Length > DESC_LIMIT) {
            status = "Model description is too long";
            return false;
        }


        if (param_tags.Length > TAGS_LIMIT) {
            status = "Model tags are too long";
            return false;
        }


        int nbSelectedObjects = Selection.GetTransforms(SelectionMode.Deep).Length;
        if (nbSelectedObjects == 0) {
            status = "No object selected to export";
            return false;
        }

        status = "Export " + nbSelectedObjects + " object" + (nbSelectedObjects != 1 ? "s" : "");
        return true;
    }

    [MenuItem("Tools/Export to glTF 2.0")]
    static void Init() {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX // edit: added Platform Dependent Compilation - win or osx standalone
        ExporterGLTF20 window = (ExporterGLTF20)EditorWindow.GetWindow(typeof(ExporterGLTF20));
        window.titleContent.text = "glTF 2.0 Exporter";
        window.Show();
#else // and error dialog if not standalone
		EditorUtility.DisplayDialog("Error", "Your build target must be set to standalone", "Okay");
#endif
    }
}

#endif