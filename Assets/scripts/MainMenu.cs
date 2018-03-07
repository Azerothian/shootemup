using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using TMPro;

public class MainMenu : MonoBehaviour {
  public GameObject mainMenu;
  public GameObject introPanel;
  public GameObject introText;
  public float timeToFade = 4000;
  public float alpha = 255;
  public float step;
  public bool transitioning = true;
  // Use this for initialization
  void Start () {
    step = 255 / timeToFade;
    var text = introText.GetComponent<TextMeshProUGUI>();
    text.CrossFadeColor(new Color(0, 0, 0, 0), 3, false, true);
  }
	
	// Update is called once per frame
	void FixedUpdate () {
    if  (transitioning)
    {
      alpha -= (Time.fixedDeltaTime * step);
      if(alpha < 0)
      {
        transitioning = false;
        mainMenu.SetActive(true);
        introPanel.SetActive(false);
      }
    }
	}
}
