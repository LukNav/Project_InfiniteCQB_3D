using ICQB.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public StatsController playerStatsController;
    public GameObject deathScreen;
    public Slider hpSlider;

    public void Start()
    {
        hpSlider.value = playerStatsController.initialHealth;
        playerStatsController.deathDelegate += SetActiveDeathScreen;
        playerStatsController.damageDelegate += UpdateHealthBar;
    }

    private void UpdateHealthBar()
    {
        hpSlider.value = playerStatsController.health/ playerStatsController.initialHealth;
    }

    private void SetActiveDeathScreen()
    {
        hpSlider.gameObject.SetActive(false);
        deathScreen.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void OnStart()
    {
        SceneManager.LoadScene(1);
    }
}
