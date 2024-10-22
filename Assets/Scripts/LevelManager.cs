using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;


public struct LevelObject
{
    public string prefabName;
    public float[] position;
    public float[] rotation;
    public float[] scale;
}

public class LevelManager : MonoBehaviour
{
    public GameObject[] levelObjectsPrefab;

    public void saveLevel(string levelName)
    {
        List<LevelObject> levelObjects = new List<LevelObject>();

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("LevelObject"))
        {
            LevelObject levelObject = new LevelObject();
            levelObject.prefabName = obj.name.Split('(')[0].Trim();
            levelObject.position = new float[] { obj.transform.position.x, obj.transform.position.y, obj.transform.position.z };
            levelObject.rotation = new float[] { obj.transform.rotation.x, obj.transform.rotation.y, obj.transform.rotation.z, obj.transform.rotation.w };
            levelObject.scale = new float[] { obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z };

            levelObjects.Add(levelObject);
        }

        string path = Path.Combine(Application.dataPath, "levels", levelName + ".json");
        string json = JsonConvert.SerializeObject(levelObjects, Formatting.Indented);

        using (StreamWriter file = new StreamWriter(path))
        {
            file.Write(json);
        }

        Debug.Log("Level saved to: " + path);
    }

    public void loadLevel(string levelName)
    {
        string path = Path.Combine(Application.dataPath, "levels", levelName + ".json");
        string json = File.ReadAllText(path);

        List<LevelObject> levelObjects = JsonConvert.DeserializeObject<List<LevelObject>>(json);

        foreach (LevelObject levelObject in levelObjects)
        {
            // check if level object name is in the list of prefab
            foreach(GameObject prefab in levelObjectsPrefab)
            {
                if (prefab.name == levelObject.prefabName)
                {
                    GameObject obj = Instantiate(prefab, 
                                                    new Vector3(levelObject.position[0], levelObject.position[1], levelObject.position[2]), 
                                                    new Quaternion(levelObject.rotation[0], levelObject.rotation[1], levelObject.rotation[2], levelObject.rotation[3]));
                    obj.transform.localScale = new Vector3(levelObject.scale[0], levelObject.scale[1], levelObject.scale[2]);
                    break;
                }
            }
        }

        Debug.Log("Level loaded from: " + path);
    }

    void Start()
    {
        if (File.Exists(Path.Combine(Application.dataPath, "levels", PlayerPrefs.GetString("mapName") + ".json")))
        {
            loadLevel(PlayerPrefs.GetString("mapName"));
        }
        else 
        {
            saveLevel(PlayerPrefs.GetString("mapName"));
        }
    }
}
