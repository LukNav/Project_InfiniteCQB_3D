using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public StatsController playerStatsController;
    public GameObject deathScreen;

    public void Start()
    {
        playerStatsController.deathDelegate += SetActiveDeathScreen;
    }

    private void SetActiveDeathScreen()
    {
        deathScreen.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
