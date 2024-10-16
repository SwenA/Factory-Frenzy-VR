using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement; //3

public class MissingScriptEditor : MonoBehaviour
{
    [MenuItem("LIT/Missing/Scene > Select GameObjects With Missing Scripts")]
    static void SelectGameObjectsWithMissingScript()
    {
        //Get the current scene and all top-level GameObjects in the scene hierarchy
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = currentScene.GetRootGameObjects();

        List<GameObject> objectsWithDeadLinks = new List<GameObject>();
        int res = 0;
        foreach (GameObject g in rootObjects)
        {
            res += SelectGameObjectsWithMissingScript(g, ref objectsWithDeadLinks, true);
        }

        if(res>0)
        {
            Debug.LogError($"Found {objectsWithDeadLinks.Count} with missing script and GameObject are selected");
            Selection.objects = objectsWithDeadLinks.ToArray();
        }
        else
        {
            Debug.Log($"No missing script found");
        }
    }

    static int SelectGameObjectsWithMissingScript(GameObject g, ref List<GameObject> selectionList, bool recursive = true, string indentStr = "")
    {
        
        //Get all components on the GameObject, then loop through them 
        Component[] components = g.GetComponents<Component>();
        //Debug.Log($"{indentStr}{g.name} - {components.Length} components remove:{removeMissingComponent}");
        int res = 0;
        for (int i = components.Length-1; i >=0; i--)
        {
            Component currentComponent = components[i];
            //If the component is null, that means it's a missing script!
            if (currentComponent == null)
            {
                //Add the sinner to our naughty-list
                Selection.activeGameObject = g;
                selectionList.Add(g);
                res += 1;
            }
        }
        //GameObjectUtility.GetMonoBehavioursWithMissingScriptCount()
        for (int i = 0; i < g.transform.childCount; i++)
        {
            res += SelectGameObjectsWithMissingScript(g.transform.GetChild(i).gameObject, ref selectionList, recursive, indentStr + " ");
        }
        return res;
    }


    [MenuItem("LIT/Missing/GameObject > Remove Missing Scripts Recursively")]
    private static void FindAndRemoveMissingInSelected()
    {
        if(EditorUtility.DisplayDialog("Remove MissingScript", "Are you sure you want to remove Missing Script of selected objects ?\n This Action can't be undone", "Yes, remove missing script", "Cancel"))
        {
            FindAndRemoveMissingInSelected(true);
        }
        
    }

    private static void FindAndRemoveMissingInSelected(bool removeMonoBehavioursWithMissingScript = false)
    {
        var deepSelection = EditorUtility.CollectDeepHierarchy(Selection.gameObjects);
        int compCount = 0;
        int goCount = 0;
        foreach (var o in deepSelection)
        {
            if (o is GameObject go)
            {
                int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                if (count > 0)
                {
                    // Edit: use undo record object, since undo destroy wont work with missing
                    Undo.RegisterCompleteObjectUndo(go, "Remove missing scripts");
                    if (removeMonoBehavioursWithMissingScript)
                    {
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                    }
                    compCount += count;
                    goCount++;
                }
            }
        }
        Debug.Log($"Found and removed {compCount} missing scripts from {goCount} GameObjects");
    }
}