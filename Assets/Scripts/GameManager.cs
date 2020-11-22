using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region User Interface

    public TextMeshProUGUI DeathMessage;
    public GameObject DeathBG;

    #endregion

    public PlayerController PlayerController;

    private void Update()
    {
        if (PlayerController.isDead)
        {
            DeathMessage.text = "You Became Not Living.";
            PlayerController.enabled = false;
            DeathBG.SetActive(true);
        }
    }
}
