using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;


public struct SerializableVector3
{
  public float x;
  public float y;
  public float z;

  public SerializableVector3(float x, float y, float z)
  {
    this.x = x;
    this.y = y;
    this.z = z;
  }

  public static implicit operator Vector3(SerializableVector3 sVector)
  {
    return new Vector3(sVector.x, sVector.y, sVector.z);
  }

  public static implicit operator SerializableVector3(Vector3 vector)
  {
    return new SerializableVector3(vector.x, vector.y, vector.z);
  }
}

public struct SerializableQuaternion
{
  public float x;
  public float y;
  public float z;
  public float w;

  public SerializableQuaternion(float x, float y, float z, float w)
  {
    this.x = x;
    this.y = y;
    this.z = z;
    this.w = w;
  }

  public static implicit operator Quaternion(SerializableQuaternion sQuaternion)
  {
    return new Quaternion(sQuaternion.x, sQuaternion.y, sQuaternion.z, sQuaternion.w);
  }

  public static implicit operator SerializableQuaternion(Quaternion quaternion)
  {
    return new SerializableQuaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
  }
}

public struct LevelObject
{
  public string prefabName;
  public SerializableVector3 position;
  public SerializableQuaternion rotation;
  public SerializableVector3 scale;
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
            levelObject.position = obj.transform.position;
            levelObject.rotation = obj.transform.rotation;
            levelObject.scale = obj.transform.localScale;

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
                    GameObject obj = Instantiate(prefab, levelObject.position, levelObject.rotation);
                    obj.transform.localScale = levelObject.scale;
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            saveLevel(PlayerPrefs.GetString("mapName"));
        }
    }
}
