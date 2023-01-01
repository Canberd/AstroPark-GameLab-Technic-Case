using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menuManager : MonoBehaviour
{
    #region SINGLETON
    public static menuManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public int level;
    public TextMeshProUGUI levelText;

    [Header("PANELS")]
    public GameObject startPanel;
    public GameObject inGamePanel;
    public GameObject levelCompletedPanel;
    public GameObject levelFailedPanel;


    private void Start()
    {
        GetLevelData();
    }

    public void StartTheGame()
    {
        PlayerManager.PlayerManagerInstance.isForwardMove = true;
        ChangePanel(inGamePanel);
        PlayerManager.PlayerManagerInstance.gameState = true;

        PlayerManager.PlayerManagerInstance.stickmanParent.GetChild(0).GetComponent<Animator>().SetBool("run", true);
    }

    public void ChangePanel(GameObject openedPanel)
    {
        startPanel.SetActive(false);
        inGamePanel.SetActive(false);
        levelCompletedPanel.SetActive(false);
        levelFailedPanel.SetActive(false);

        openedPanel.SetActive(true);
    }

    public void NextLeveButton()
    {
        level++;
        if (level > SceneManager.sceneCountInBuildSettings)
        {
            level = 1;
        }
        PlayerPrefs.SetInt("leveldata", level);
        SceneManager.LoadScene(level - 1);
    }

    public void FailedLevelButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GetLevelData()
    {
        if (PlayerPrefs.HasKey("leveldata"))
        {
            level = PlayerPrefs.GetInt("leveldata");
        }
        else
        {
            level = 1;
        }

        levelText.SetText("LEVEL " + level);
    }
}
