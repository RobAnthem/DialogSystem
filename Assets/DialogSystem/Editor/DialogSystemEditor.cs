using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class DialogSystemEditor : EditorWindow
{
    public enum EditorTypes
    {
        None,
        Dialogs,
        Vars,
        LipsyncCreator,
        EntityCreator
    }
    public EditorTypes currentEditor;
    public Vector2 mainScroll, subScroll;
    public List<DialogGraph> graphs;
    private DialogVars.DialogVar currentVar = null;
    private string dialogObjName;
    private bool showNewDialog;
    [MenuItem("Tools/Game Creator")]
    public static void StartWindow()
    {
        DialogSystemEditor g = GetWindow<DialogSystemEditor>();
        g.minSize = new Vector2(600, 500);
        g.graphs = new List<DialogGraph>(Resources.LoadAll<DialogGraph>(""));

    }
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        foreach (EditorTypes eT in System.Enum.GetValues(typeof(EditorTypes)))
        {
            if (GUILayout.Button(eT.ToString(), EditorStyles.toolbarButton))
            {
                currentEditor = eT;
                if (currentEditor == EditorTypes.LipsyncCreator)
                {
                    currentEditor = EditorTypes.None;
                    LipsyncCreator.OpenLipsyncCreator();
                }
                else if (currentEditor == EditorTypes.EntityCreator)
                {
                    currentEditor = EditorTypes.None;
                    DialogTriggerCreator.OpenDialogCreator();
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        if (currentEditor != EditorTypes.None)
        {
            if (currentEditor == EditorTypes.Dialogs)
            {
                EditorGUILayout.BeginVertical();
                RenderCreateDialog();
                RenderDialogList();
                EditorGUILayout.EndVertical();
            }
            else if (currentEditor == EditorTypes.Vars)
            {

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                RenderCreateVars();
                RenderVarsList();
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                RenderVarEdit();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    public void RenderCreateDialog()
    {
        EditorGUILayout.LabelField("File Name");
        dialogObjName = EditorGUILayout.TextField(dialogObjName);
        if (GUILayout.Button("Create New"))
        {
            if (!string.IsNullOrEmpty(dialogObjName))
            {
                DialogObject dObj = DialogObject.CreateInstance<DialogObject>();
                dObj.name = dialogObjName;
                DialogGraph graph = DialogGraph.CreateInstance<DialogGraph>();
                graph.dialogObject = dObj;
                AssetDatabase.CreateAsset(graph, "Assets/DialogSystem/Editor/Resources/" + dialogObjName + ".asset");
                AssetDatabase.CreateAsset(dObj, "Assets/DialogSystem/Resources/" + dialogObjName + ".asset");
                graphs.Add(graph);
                dialogObjName = "";
            }
        }
    }
    public void OpenDialog(DialogGraph dg)
    {
        XNodeEditor.NodeEditorWindow.Open(dg);
    }
    public void RenderDialogList()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        mainScroll = EditorGUILayout.BeginScrollView(mainScroll);
        foreach (DialogGraph dg in graphs)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open: " + dg.name))
            {
                XNodeEditor.NodeEditorWindow.Open(dg);
            }
            if (GUILayout.Button("Delete"))
            {
                graphs.Remove(dg);
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(dg.dialogObject));
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(dg));
                return;
            }
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
    public void RenderCreateVars()
    {
        if (GUILayout.Button("Create New"))
        {
            DialogVars.DialogVar dv = new DialogVars.DialogVar();
            dv.ID = ToolKit.GetUniqueID();
            dv.Name = "New Var";
            DialogVars.Instance.dialogVars.Add(dv.ID, dv);
            DialogVars.Instance.Save();
            return;
        }
    }
	private void OnLostFocus()
	{
        DialogVars.Instance.Save();

    }
    public void RenderVarsList()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        mainScroll = EditorGUILayout.BeginScrollView(mainScroll);
        foreach (KeyValuePair<string, DialogVars.DialogVar> dv in DialogVars.Instance.dialogVars)
        {
            EditorGUILayout.BeginHorizontal();
            if (dv.Value == currentVar)
            {
                if (GUILayout.Button("*"+dv.Value.Name+"*"))
                {
                    currentVar = null;
                }
            }
            else
            {
                if (GUILayout.Button(dv.Value.Name))
                {
                    currentVar = dv.Value;
                }
            }
            if (GUILayout.Button("X"))
            {
                DialogVars.Instance.dialogVars.Remove(dv.Key);
                return;
            }
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
    public void RenderVarEdit()
    {
        if (currentVar != null && !string.IsNullOrEmpty(currentVar.ID))
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            currentVar.Name = EditorGUILayout.TextField("Name", currentVar.Name);
            currentVar.value = EditorGUILayout.Toggle("Start Value", currentVar.value);
            EditorGUILayout.EndVertical();
        }
    }
}
