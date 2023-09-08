using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class EnemyWindow : EditorWindow
{
    [MenuItem("Window/UI Toolkit/EnemyWindow")]
    public static void ShowExample()
    {
        EnemyWindow wnd = GetWindow<EnemyWindow>();
        wnd.titleContent = new GUIContent("EnemyWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/EnemyWindow.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        labelFromUXML.name = "EnemyWindow";
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/EnemyWindow.uss");
        root.styleSheets.Add(styleSheet);
        
    }
}