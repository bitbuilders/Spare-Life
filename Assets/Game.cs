using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : Singleton<Game>
{
    [SerializeField] GameObject m_PauseScreen = null;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (m_PauseScreen.activeSelf)
            {
                UnloadPause();
            }
            else
            {
                LoadPause();
            }
        }
    }

    public void UnloadPause()
    {
        m_PauseScreen.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void LoadPause()
    {
        m_PauseScreen.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("The Start");
    }

    public void LoadWinScreen()
    {
        SceneManager.LoadScene("The Win");
    }
}
