using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    GameObject pauseCanvas;
    InputManagerIF inputManagerM;
    InputManagerIF inputManagerC;

    Slider p1_hp;
    Slider p2_hp;
    // Start is called before the first frame update
    void Start()
    {
        pauseCanvas = GameObject.Find("PauseCanvas");
        pauseCanvas.SetActive(false);
        inputManagerM = new InputManagerMouse();
        inputManagerC = new InputManagerController();
        p1_hp = GameObject.Find("P1_HPBar").GetComponent<Slider>();
        p2_hp = GameObject.Find("P2_HPBar").GetComponent<Slider>();
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
}
