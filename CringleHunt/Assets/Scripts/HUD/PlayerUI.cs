using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI playerHealthText;
    
    private void Update()
    {
        playerHealthText.text = Controller.Instance.Health.ToString("Health: " + "0");
        
        
    }
    
}
