using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Unity.XR.CoreUtils;
using Unity.VisualScripting;
using UnityEngine.XR.Interaction.Toolkit;


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
  public SerializableVector3 MoveToPosition;
}


public class LevelManager : MonoBehaviour
{
    public GameObject[] levelObjectsPrefab;

    public void saveLevel(string levelName)
    {
      List<LevelObject> levelObjects = new List<LevelObject>();

      foreach (GameObject obj in GameObject.FindGameObjectsWithTag("LevelObject"))
      {
        Debug.Log("Saving object: " + obj.name);

        LevelObject levelObject = new LevelObject();
        levelObject.prefabName = obj.name.Split('(')[0].Trim();
        levelObject.position = obj.transform.position;
        levelObject.rotation = obj.transform.rotation;
        levelObject.scale = obj.transform.localScale;

        if (levelObject.prefabName == "Platform Move 520" || levelObject.prefabName == "MovingPlatform")
        {
          // ajouter a obj une variable vec 3
          levelObject.MoveToPosition = obj.GetComponent<MovePlat>().MoveToSphere.GetComponent<Transform>().position;
        }
        else
        {
          levelObject.MoveToPosition = Vector3.zero;
        }

        levelObjects.Add(levelObject);
      }

      Debug.Log("Saving level with " + levelObjects.Count + " objects");

      string filePath = Path.Combine(getLevelFolderPath(), levelName + ".json");

      string json = JsonConvert.SerializeObject(levelObjects, Formatting.Indented);
      File.WriteAllText(filePath, json);

      Debug.Log("Level saved to: " + filePath);
    }

    public void loadLevel(string levelName)
    {
        string filePath = Path.Combine(getLevelFolderPath(), levelName + ".json");
        string json = File.ReadAllText(filePath);

        List<LevelObject> levelObjects = JsonConvert.DeserializeObject<List<LevelObject>>(json);

        foreach (LevelObject levelObject in levelObjects)
        {
            foreach(GameObject prefab in levelObjectsPrefab)
            {
                if (prefab.name == levelObject.prefabName)
                {
                    GameObject obj = Instantiate(prefab, levelObject.position, levelObject.rotation);
                    obj.transform.localScale = levelObject.scale;
                    obj.GetComponent<Item>().itemPos = levelObject.position;
                    obj.GetComponent<Item>().itemRot = levelObject.rotation;
                    obj.GetComponent<Item>().itemScale = levelObject.scale;
                    obj.GetComponent<Item>().isInSlot = false;

                    obj.tag = "LevelObject";

                    if (prefab.name == "Platform Move 520" || prefab.name == "MovingPlatform")
                    {
                        
                        // instanciate the default sphere
                        obj.GetComponent<MovePlat>().instanciateSphere(levelObject.MoveToPosition);

                        Debug.Log("Loading well plat object: " + obj.GetComponent<MovePlat>().MoveToSphere);
                    }

                    break;
                }
            }
        }

        Debug.Log("Level loaded from: " + filePath);
    }

    private string getLevelFolderPath()
    {
        string levelFolderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "FallGuysProj", "levels");

        if (!Directory.Exists(levelFolderPath))
        {
            Directory.CreateDirectory(levelFolderPath);
        }

        return levelFolderPath;
    }

    void Start()
    {
        loadLevel(PlayerPrefs.GetString("mapName"));
    }

    void OnApplicationQuit()
    {
        saveLevel(PlayerPrefs.GetString("mapName"));
    }
}
