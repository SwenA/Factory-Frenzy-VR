using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CopyPathEditor : MonoBehaviour
{
    [MenuItem("GameObject/Copy Path")]
    private static void INTERVALES_CopyPath()
    {
        var go = Selection.activeGameObject;

        if (go == null)
        {
            return;
        }

        var path = go.name;

        while (go.transform.parent != null)
        {
            go = go.transform.parent.gameObject;
            path = string.Format("{0}/{1}", go.name, path);
        }

        EditorGUIUtility.systemCopyBuffer = path;
    }

    [MenuItem("GameObject/Copy Path", true)]
    private static bool CopyPathValidation()
    {
        // We can only copy the path in case 1 object is selected
        return Selection.gameObjects.Length == 1;
    }
}
