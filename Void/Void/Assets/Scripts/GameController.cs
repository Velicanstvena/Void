using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameController : MonoBehaviour
{
    private bool alive = true;
    private bool finished = false;
    private int numberOfHearts;
    private int numberOfBombs;
    [SerializeField] GameObject deathParticle;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private TextMeshProUGUI heartsText;
    [SerializeField] private TextMeshProUGUI bombsText;
    [SerializeField] public Button placeBombButton;

    void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        heartsText.text = numberOfHearts.ToString();
        bombsText.text = numberOfBombs.ToString();
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

    public void Win()
    {
        finished = true;
        StartCoroutine(ToggleWinScreen());
    }

    public void Die()
    {
        if (!alive)
        {
            return;
        }

        alive = false;
        finished = true;
        GameObject particle = Instantiate(deathParticle, PlayerMovement.LocalPlayerInstance.transform.position, Quaternion.identity);
        Destroy(particle, 1);
        StartCoroutine(ToggleLoseScreen());
    }

    IEnumerator ToggleWinScreen()
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0.2f;
        winScreen.SetActive(true);
    }

    IEnumerator ToggleLoseScreen()
    {
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0.2f;
        loseScreen.SetActive(true);
    }

    public bool IsAlive()
    {
        return alive;
    }

    public bool IsFinished()
    {
        return finished;
    }

    public Button GetBombButton()
    {
        return placeBombButton;
    }

    public void Leave()
    {
        if (PlayerMovement.LocalPlayerInstance.GetPhotonView().IsMine)
        {
            StartCoroutine(DisconnectAndLoad());
        }
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }

        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }

    private void ResetVars()
    {
        numberOfBombs = 0;
        numberOfHearts = 0;
        alive = true;
        finished = false;
        Time.timeScale = 1;
    }
}
