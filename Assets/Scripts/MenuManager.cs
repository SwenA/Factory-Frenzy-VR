using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    public GameObject mapCreationPanel;
    public TMPro.TMP_InputField mapNameInputField;
    public GameObject mapModifyPanel;

    // 3 Main Buttons //
    public void newMapButtonClicked()
    {
        mapCreationPanel.SetActive(true);
    }
    public void modifyMapButtonClicked()
    {
        mapModifyPanel.SetActive(true);
    }
    public void exitMapButtonClicked()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // Map Creation Panel Buttons //
    public void createMapButtonClicked()
    {
        // load Editor Scene and pass the map name
        SceneManager.LoadScene("EditorMap", LoadSceneMode.Single);
        PlayerPrefs.SetString("mapName", mapNameInputField.text);
    }
    public void backButtonClicked()
    {
        mapCreationPanel.SetActive(false);
    }
}
