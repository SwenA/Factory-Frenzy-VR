using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    public GameObject mapCreationPanel;
    public TMPro.TMP_InputField mapNameInputField;
    public GameObject mapModifyPanel;
    public GameObject gridLevels;
    public GameObject levelPanelPrefab;

    // 3 Main Buttons //
    public void newMapButtonClicked()
    {
        mapCreationPanel.SetActive(true);
    }
    public void modifyMapButtonClicked()
    {
        mapModifyPanel.SetActive(true);

        // delete all the level panels in the gridLevels
        foreach (Transform child in gridLevels.transform)
        {
            Destroy(child.gameObject);
        }

        // load all the levels in the levels folder
        string[] levels = Directory.GetFiles(Path.Combine(Application.dataPath, "levels"), "*.json");

        foreach (string level in levels)
        {
            // instantiate a new level panel child of gridLevels
            GameObject levelPanel = Instantiate(levelPanelPrefab, gridLevels.transform);

            // set the level panel name to the level name
            levelPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = Path.GetFileNameWithoutExtension(level);

            // set the level panel button on click listener
            levelPanel.GetComponent<Button>().onClick.AddListener(() => loadLevel(levelPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text));
        }
    }
    public void exitMapButtonClicked()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // Map Creation Panel Buttons //
    public void createMap()
    {   
        // check if there is already a file with the same name
        if (File.Exists(Path.Combine(Application.dataPath, "levels", mapNameInputField.text + ".json")))
        {
            Debug.LogError("Map with the same name already exists");
            return;
        }

        // load Editor Scene and pass the map name
        PlayerPrefs.SetString("mapName", mapNameInputField.text);
        SceneManager.LoadScene("EditorMap", LoadSceneMode.Single);
    }
    public void backFromCreationPanel()
    {
        mapCreationPanel.SetActive(false);
    }

    // Map Modify Panel Buttons //
    public void importMap()
    {
        // use StandaloneFileBrowser to open a file panel
        string[] paths = SFB.StandaloneFileBrowser.OpenFilePanel("Open Map", "", "json", false);

        // check if a file was selected
        if (paths.Length > 0)
        {
            // get the file name
            string fileName = Path.GetFileNameWithoutExtension(paths[0]);

            // check if there is already a file with the same name
            if (File.Exists(Path.Combine(Application.dataPath, "levels", fileName + ".json")))
            {
                Debug.LogError("Map with the same name already exists");
                return;
            }

            // copy the file to the levels folder
            File.Copy(paths[0], Path.Combine(Application.dataPath, "levels", fileName + ".json"));

            // instantiate a new level panel child of gridLevels
            GameObject levelPanel = Instantiate(levelPanelPrefab, gridLevels.transform);

            // set the level panel name to the level name
            levelPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = fileName;

            // set the level panel button on click listener
            levelPanel.GetComponent<Button>().onClick.AddListener(() => loadLevel(levelPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text));
        }

    }
    public void backFromModifyPanel()
    {
        mapModifyPanel.SetActive(false);
    }
    public void loadLevel(string levelName)
    {
        PlayerPrefs.SetString("mapName", levelName);
        SceneManager.LoadScene("EditorMap", LoadSceneMode.Single);
    }
}
