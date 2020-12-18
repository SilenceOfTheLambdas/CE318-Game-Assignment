using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public void ClosePauseMenu()
    {
        gameObject.SetActive(false);
    }
    
    public void OpenPauseMenu()
    {
        gameObject.SetActive(true);
    }
}