using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelLoader : MonoBehaviour {
  public GameObject mainMenu;
  public GameObject loadingPanel;
  public Slider loadingBar;
  public void LoadLevel(int sceneIndex)
  {
    mainMenu.SetActive(false);
    loadingPanel.SetActive(true);
    StartCoroutine(LoadLevelAsync(sceneIndex));
  }
  IEnumerator LoadLevelAsync(int sceneIndex)
  {
    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
    while(!operation.isDone)
    {
      float progress = Mathf.Clamp01(operation.progress / 0.9f);
      loadingBar.value = progress;
      yield return null;
    }
  }
}
