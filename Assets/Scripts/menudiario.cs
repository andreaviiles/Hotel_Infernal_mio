using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class menudiario : MonoBehaviour
{
    [Header("Personal System")]
    public int currentPersonal = 20;
    public TMP_Text personalText;
    public int[] taskPersonalCost;

    [Header("UI Panels")]
    public GameObject confirmPopup;
    public GameObject[] minigamePanels;
    public GameObject[] taskCheckmarks;

    private int selectedTask = -1;

    void Start()
    {
        UpdateEnergyUI();
        confirmPopup.SetActive(false);

        foreach (var mini in minigamePanels)
            mini.SetActive(false);
    }
     void UpdateEnergyUI()
    {
        personalText.text = "Personal: " + currentPersonal;
    }
      public void OnTaskClicked(int taskIndex)
    {
        if (currentPersonal < taskPersonalCost[taskIndex])
        {
            Debug.Log("¡No queda personal disponible!");
            return;
        }

        selectedTask = taskIndex;
        confirmPopup.SetActive(true);
    }

    public void CancelTask()
    {
        confirmPopup.SetActive(false);
        selectedTask = -1;
    }

    public void StartTask()
    {
        currentPersonal -= taskPersonalCost[selectedTask];
        UpdateEnergyUI();

        confirmPopup.SetActive(false);
        minigamePanels[selectedTask].SetActive(true);
    }

    public void CompleteMinigame()
    {
        minigamePanels[selectedTask].SetActive(false);
        taskCheckmarks[selectedTask].SetActive(true);

        selectedTask = -1;
    }
}

