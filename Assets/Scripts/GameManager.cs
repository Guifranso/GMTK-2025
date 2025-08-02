using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public PlayerMovement playerMovement;

    public void onTopDownClick()
    {
        playerMovement.movementType = 1;
        Debug.Log("Clicou");
    }

    public void onPlatformClick()
    {
        playerMovement.movementType = 2;
        Debug.Log("Clicou");
    }
}
