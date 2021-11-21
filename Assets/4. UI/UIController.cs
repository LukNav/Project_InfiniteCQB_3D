using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
