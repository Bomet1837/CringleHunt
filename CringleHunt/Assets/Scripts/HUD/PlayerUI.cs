using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI playerHealthText;

    private void Start()
    {
        playerHealthText.text = Controller.Instance.startingHealth.ToString("Health: " + "0");
    }
    
    private void Update()
    {
        playerHealthText.text = Controller.Instance.Health.ToString("Health: " + "0");
        
        
    }
    
}
