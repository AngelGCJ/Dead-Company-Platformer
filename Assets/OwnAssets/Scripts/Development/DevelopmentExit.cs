using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopmentExit : MonoBehaviour
{
    [SerializeField] public GameObject panel;

    private void Start()
    {
        panel.SetActive(false);
    }
    public void openExitMenu()
    {
        panel.SetActive(true);
    }

    public void exitForReal()
    {
        Application.Quit();
    }

    public void goBack()
    {
        panel.SetActive(false);
    }
}
