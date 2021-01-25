﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private int numberOfHearts;
    [SerializeField] private int numberOfBombs;

    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private Text heartsText;
    [SerializeField] private Button placeBombButton;

    void Start()
    {

    }

    void Update()
    {
        heartsText.text = numberOfHearts.ToString();

        if (numberOfBombs > 0)
        {
            placeBombButton.gameObject.SetActive(true);
        } 
        else
        {
            placeBombButton.gameObject.SetActive(false);
        }
    }

    public void IncreaseNumberOfHearts()
    {
        this.numberOfHearts++;
    }
    public void IncreaseNumberOfBombs()
    {
        this.numberOfBombs++;
    }

    public void DecreaseNumberOfBombs()
    {
        this.numberOfBombs--;
    }

    public int GetNumberOfBombs()
    {
        return numberOfBombs;
    }

    public void ToggleWinScreen()
    {
        Time.timeScale = 0.2f;
        winScreen.SetActive(true);
    }

    public void ToggleLoseScreen()
    {
        Time.timeScale = 0.2f;
        loseScreen.SetActive(true);
    }

    public void Die()
    {
        player.SetActive(false);
        ToggleLoseScreen();
    }
}