using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    GameObject pauseCanvas;
	GameObject deathCanvas;
    InputManagerIF inputManagerM;
    InputManagerIF inputManagerC;
	private short playersAlive;
	private GameObject[] golemA;
	private Golem golem;
    Slider p1_hp;
    Slider p2_hp;
    Image p1_tpcd;
    Image p2_tpcd;
    // Start is called before the first frame update
    void Start()
    {
        pauseCanvas = GameObject.Find("PauseCanvas");
        pauseCanvas.SetActive(false);
		deathCanvas = GameObject.Find("DeathCanvas");
        deathCanvas.SetActive(false);
        inputManagerM = new InputManagerMouse();
        inputManagerC = new InputManagerController();
        p1_hp = GameObject.Find("P1_HPBar").GetComponent<Slider>();
        p2_hp = GameObject.Find("P2_HPBar").GetComponent<Slider>();
        p1_tpcd = GameObject.Find("P1_TPCD").GetComponent<Image>();
        p2_tpcd = GameObject.Find("P2_TPCD").GetComponent<Image>();
        playersAlive = 2;
		golemA = GameObject.FindGameObjectsWithTag("Enemy");
		golem = golemA[0].GetComponent<Golem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManagerM.PauseButton() || inputManagerC.PauseButton())
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        pauseCanvas.SetActive(!pauseCanvas.activeSelf);
        if (pauseCanvas.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }

    internal void UpdateHP(int playerID, float hp)
    {
        if (playerID == 0)
        {
            p1_hp.value = hp;
        }
        else if (playerID == 1)
        {
            p2_hp.value = hp;
        }
    }
	
	public void ToggleDeath(){
		
        deathCanvas.SetActive(!deathCanvas.activeSelf);
        if (deathCanvas.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
	}
	
		public void reloadMap(){
		SceneManager.LoadScene("LavaCave");
		ToggleDeath();
	}
	
	public void informPlayerDeath(){
		golem.refreshTargetList();
		playersAlive--;
		if(playersAlive <= 0){
			ToggleDeath();
		}
	}

    public void UpdateTPCD(int playerID, float cd)
    {
        if (playerID == 0)
        {
            p1_tpcd.fillAmount = cd;
            if (cd > 1)
            {
                p1_tpcd.color = Color.yellow;
            }
            else
            {
                p1_tpcd.color = Color.black;
            }
        }
        else if (playerID == 1)
        {
            p2_tpcd.fillAmount = cd;
            if (cd > 1)
            {
                p2_tpcd.color = Color.yellow;
            }
            else
            {
                p2_tpcd.color = Color.black;
            }
        }
    }
}
