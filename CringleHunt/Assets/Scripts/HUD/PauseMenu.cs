using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private Controller _isPlayable;
    
    [SerializeField]
    private GameObject pauseMenu;
    private void Start()
    {
        _isPlayable = FindAnyObjectByType<Controller>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPlayable == true)
            {
                pauseMenu.SetActive(false);
            }
            else
            {
                pauseMenu.SetActive(true);
            }
        }
    }
   
}
