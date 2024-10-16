using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Animations;


//MenuItem("GameObject/Distribute Object")
public class DistributeGameObjectEditor : EditorWindow
{

    [MenuItem("GameObject/Distribute")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DistributeGameObjectEditor window = (DistributeGameObjectEditor)EditorWindow.GetWindow<DistributeGameObjectEditor>();
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Distribute:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("X"))
        {
            distribute(Axis.X);
        }

        if (GUILayout.Button("Y"))
        {
            distribute(Axis.Y);
        }

        if (GUILayout.Button("Z"))
        {
            distribute(Axis.Z);
        }

        if (GUILayout.Button("XY"))
        {
            distribute(Axis.X);
            distribute(Axis.Y);
        }

        if (GUILayout.Button("XZ"))
        {
            distribute(Axis.X);
            distribute(Axis.Z);
        }

        if (GUILayout.Button("YZ"))
        {
            distribute(Axis.Y);
            distribute(Axis.Z);
        }
        if (GUILayout.Button("XYZ"))
        {
            distribute(Axis.X);
            distribute(Axis.Y);
            distribute(Axis.Z);
        }

        GUILayout.EndHorizontal();

        GUILayout.Label("Center:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("X"))
        {
            center(Axis.X);
        }

        if (GUILayout.Button("Y"))
        {
            center(Axis.Y);
        }

        if (GUILayout.Button("Z"))
        {
            center(Axis.Z);
        }

        if (GUILayout.Button("XY"))
        {
            center(Axis.X);
            center(Axis.Y);
        }

        if (GUILayout.Button("XZ"))
        {
            center(Axis.X);
            center(Axis.Z);
        }

        if (GUILayout.Button("YZ"))
        {
            center(Axis.Y);
            center(Axis.Z);
        }
        if (GUILayout.Button("XYZ"))
        {
            center(Axis.X);
            center(Axis.Y);
            center(Axis.Z);
        }

        GUILayout.EndHorizontal();


        GUILayout.Label("Drop Object:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Bottom"))
        {
            dropObject("Bottom");
        }

        if (GUILayout.Button("Center"))
        {
            dropObject("Center");
        }

        if (GUILayout.Button("Origin"))
        {
            dropObject("Origin");
        }

        

        GUILayout.EndHorizontal();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Transform GetMinPosition(Transform[] sel, Axis axis)
    {
        if (axis == Axis.X) return sel.Aggregate((curMin, p) => (curMin == null || p.position.x < curMin.position.x) ? p : curMin);
        if (axis == Axis.Y) return sel.Aggregate((curMin, p) => (curMin == null || p.position.y < curMin.position.y) ? p : curMin);
        if (axis == Axis.Z) return sel.Aggregate((curMin, p) => (curMin == null || p.position.z < curMin.position.z) ? p : curMin);
        return null;
    }

    Transform GetMaxPosition(Transform[] sel, Axis axis)
    {
        if (axis == Axis.X) return sel.Aggregate((curMin, p) => (curMin == null || p.position.x > curMin.position.x) ? p : curMin);
        if (axis == Axis.Y) return sel.Aggregate((curMin, p) => (curMin == null || p.position.y > curMin.position.y) ? p : curMin);
        if (axis == Axis.Z) return sel.Aggregate((curMin, p) => (curMin == null || p.position.z > curMin.position.z) ? p : curMin);
        return null;
    }


    Transform[] sortTransformArrayOnAxis(Transform[] sel, Axis axis)
    {
        switch (axis)
        {
            case Axis.X:
                return Selection.transforms.OrderBy((t) => t.position.x).ToArray();
            case Axis.Y:
                return Selection.transforms.OrderBy((t) => t.position.y).ToArray();
            case Axis.Z:
                return Selection.transforms.OrderBy((t) => t.position.z).ToArray();
            case Axis.None:
            default:
                return sel;
        }
    }

    void distribute(Axis axis)
    {
        if (axis == Axis.None) return;
        if(Selection.transforms.Length<=2)
        {
            Debug.Log($"Select at least 3 objects");
            return;
        }
        Transform[] selectionSorted = sortTransformArrayOnAxis(Selection.transforms, axis);

        var minTransform = GetMinPosition(Selection.transforms, axis);
        var maxTransform = GetMaxPosition(Selection.transforms, axis);
        Vector3 deltaObj = (maxTransform.position - minTransform.position) / (selectionSorted.Length - 1);
        
        Debug.Log($"Distribute on {axis} on {selectionSorted.Length} Transforms\n" +
            $"  min max {minTransform.position} - {maxTransform.position}\n\n" +
            $" delta: {deltaObj}");
        
        for (int i = 0; i < selectionSorted.Length; i++)
        {
            var t = selectionSorted[i];
            Vector3 direction = Vector3.zero;
            Vector3 p = t.transform.position;

            switch (axis)
            {
                case Axis.X:
                    p.x = minTransform.position.x + i * deltaObj.x;
                    break;
                case Axis.Y:
                    p.y = minTransform.position.y + i * deltaObj.y;
                    break;
                case Axis.Z:
                    p.z = minTransform.position.z + i * deltaObj.z;
                    break;
            }
            Undo.RecordObject(t.transform, "distribute");
            t.transform.position = p;
        }
    }

    /// <summary>
    /// center the object on the axis => put the axis value to the same as the first selected
    /// </summary>
    /// <param name="axis"></param>
    void center(Axis axis)
    {
        if (axis == Axis.None) return;
        if (Selection.transforms.Length <= 1)
        {
            Debug.Log($"Select at least 2 objects");
            return;
        }

        Transform firstSelected = Selection.transforms[0];

        Transform t;
        for (int i = 1; i < Selection.transforms.Length; i++)
        {
            t = Selection.transforms[i];
            Undo.RecordObject(t.transform, "center");
            switch (axis)
            {
                case Axis.X:
                    t.position = new Vector3(firstSelected.position.x, t.position.y, t.position.z);
                    break;
                case Axis.Y:
                    t.position = new Vector3(t.position.x, firstSelected.position.y, t.position.z);
                    break;
                case Axis.Z:
                    t.position = new Vector3(t.position.x, t.position.y, firstSelected.position.z);
                    break;
                case Axis.None:
                default:
                    break;
            }
        }
    }

    void dropObject(string method)
    {
        const int ignoreRaycastLayer = 2;
        // drop multi-selected objects using the right method
        for (int i = 0; i < Selection.transforms.Length; i++)
        {
            // get the game object
            GameObject go = Selection.transforms[i].gameObject;

            // get the bounds
            Bounds bounds = go.GetComponent<Renderer>().bounds;
            RaycastHit hit;
            float yOffset;

            // override layer so it doesn't hit itself
            int savedLayer = go.layer;
            Undo.RecordObject(go, "layer");
            go.layer = ignoreRaycastLayer; // ignore raycast
                          // see if this ray hit something
            if (Physics.Raycast(go.transform.position, -Vector3.up, out hit))
            {
                // determine how the y will need to be adjusted
                switch (method)
                {
                    case "Bottom":
                        yOffset = go.transform.position.y - bounds.min.y;
                        break;
                    case "Center":
                        yOffset = bounds.center.y - go.transform.position.y;
                        break;
                    default:
                    case "Origin":
                        yOffset = 0.0f;
                        break;

                }
                Undo.RecordObject(go.transform, "position");
                go.transform.position = hit.point+ yOffset * Vector3.up;
            }
            // restore layer
            go.layer = savedLayer;
        }
    }
        /*
        class DropObjectEditor extends EditorWindow
        {
            // add menu item
            @MenuItem ("Window/Drop Object")

        static function Init()
        {
            // Get existing open window or if none, make a new one:
            var window : DropObjectEditor = EditorWindow.GetWindow(DropObjectEditor);
            window.Show();
        }

        function OnGUI()
        {
            GUILayout.Label("Drop Using:", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Bottom"))
            {
                DropObjects("Bottom");
            }

            if (GUILayout.Button("Origin"))
            {
                DropObjects("Origin");
            }

            if (GUILayout.Button("Center"))
            {
                DropObjects("Center");
            }

            GUILayout.EndHorizontal();
        }

        function DropObjects(method : String)
        {
            // drop multi-selected objects using the right method
            for (var i : int = 0; i < Selection.transforms.length; i++)
            {
                // get the game object
                var go : GameObject = Selection.transforms[i].gameObject;

                // don't think I need to check, but just to be sure...
                if (!go)
                {
                    continue;
                }

                // get the bounds
                var bounds : Bounds = go.renderer.bounds;
                var hit : RaycastHit;
                var yOffset : float;

                // override layer so it doesn't hit itself
                var savedLayer : int = go.layer;
                go.layer = 2; // ignore raycast
                // see if this ray hit something
                if (Physics.Raycast(go.transform.position, -Vector3.up, hit))
                {
                    // determine how the y will need to be adjusted
                    switch (method)
                    {
                        case "Bottom":
                            yOffset = go.transform.position.y - bounds.min.y;
                            break;
                        case "Origin":
                            yOffset = 0.0;
                            break;
                        case "Center":
                            yOffset = bounds.center.y - go.transform.position.y;
                            break;
                    }
                    go.transform.position = hit.point;
                    go.transform.position.y += yOffset;
                }
                // restore layer
                go.layer = savedLayer;
            }
        }*/
    }
