using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedometerAndGadgetValues : MonoBehaviour
{
    public CharacterController characterController;
    public TMP_Text speedText, sliderText, hopperText1, hopperText2;
    
    void Update()
    {
        float ahs = Controller.Instance.hopperStrength / 2;
        
        // Display the speed of the player
        var speedValue = characterController.velocity.magnitude.ToString("0" + "m/s");
        var sliderValue = Controller.Instance.slideSpeedScale.ToString("Slider Scale: " + "0.00");
        var hopperValue1 = Controller.Instance.hopperStrength.ToString("Hopper Strength: " + "0.00");
        var hopperValue2 = ahs.ToString("Auto Hopper Strength: " + "0.00");
        
        speedText.text = speedValue;
        sliderText.text = sliderValue;
        hopperText1.text = hopperValue1;
        hopperText2.text = hopperValue2;
    }
}
