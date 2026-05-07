using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleFileBrowser;

public enum Teams
{
    TeamA, TeamB, TeamC, TeamD
}

public enum ScoreChangeModes
{
    INCREASE,DECREASE
}

[System.Serializable]
public class SaveScore
{
    public int TeamAScore;
    public int TeamBScore;
    public int TeamCScore;
    public int TeamDScore;
}

public class ScoreManager : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_InputField ScoreInputField;
    public TextMeshProUGUI TeamA_Display;
    public TextMeshProUGUI TeamB_Display;
    public TextMeshProUGUI TeamC_Display;
    public TextMeshProUGUI TeamD_Display;

    [Header("FX Overlay")]
    public EffectBehaviour SaveFX;
    public EffectBehaviour LoadFX;

    [Header("Team A FX")]
    public EffectBehaviour TeamA_UpdateFx;
    public EffectBehaviour TeamA_PositiveFx;
    public EffectBehaviour TeamA_NegativeFx;

    [Header("Team B FX")]
    public EffectBehaviour TeamB_UpdateFx;
    public EffectBehaviour TeamB_PositiveFx;
    public EffectBehaviour TeamB_NegativeFx;

    [Header("Team C FX")]
    public EffectBehaviour TeamC_UpdateFx;
    public EffectBehaviour TeamC_PositiveFx;
    public EffectBehaviour TeamC_NegativeFx;

    [Header("Team D FX")]
    public EffectBehaviour TeamD_UpdateFx;
    public EffectBehaviour TeamD_PositiveFx;
    public EffectBehaviour TeamD_NegativeFx;

    [Header("Variables")]
    public int TeamA = 0;
    public int TeamB = 0;
    public int TeamC = 0;
    public int TeamD = 0;

    [Header("Misc. Settings")]
    public float cooldownduration = 1f;
    private bool isOnCooldown = false;

    private void Awake()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Json", ".json"));
        FileBrowser.SetDefaultFilter(".json");
    }

    public void ChangeScore(Teams InputTeam,ScoreChangeModes scoremode)
    {
        if(isOnCooldown)
            return;

        if (ScoreInputField.text == null)
            return;

        if (int.Parse(ScoreInputField.text) <= 0)
            return;

        int scoreinput = 0;

        switch (scoremode)
        {
            case ScoreChangeModes.INCREASE:
                int.TryParse(ScoreInputField.text, out scoreinput);
                break;
            case ScoreChangeModes.DECREASE:
                int.TryParse(ScoreInputField.text, out scoreinput);
                scoreinput = scoreinput * -1;
                break;
        }

        switch (InputTeam)
        {
            case Teams.TeamA:
                
                TeamA += scoreinput;

                switch(scoremode){
                    case ScoreChangeModes.INCREASE: TeamA_PositiveFx.PlayFX(); break;
                    case ScoreChangeModes.DECREASE: TeamA_NegativeFx.PlayFX(); break;
                }

                break;

            case Teams.TeamB:
                
                TeamB += scoreinput;

                switch (scoremode)
                {
                    case ScoreChangeModes.INCREASE: TeamB_PositiveFx.PlayFX(); break;
                    case ScoreChangeModes.DECREASE: TeamB_NegativeFx.PlayFX(); break;
                }

                break;

            case Teams.TeamC:
                
                TeamC += scoreinput;

                switch (scoremode)
                {
                    case ScoreChangeModes.INCREASE: TeamC_PositiveFx.PlayFX(); break;
                    case ScoreChangeModes.DECREASE: TeamC_NegativeFx.PlayFX(); break;
                }

                break;

            case Teams.TeamD:
                
                TeamD += scoreinput;

                switch (scoremode)
                {
                    case ScoreChangeModes.INCREASE: TeamD_PositiveFx.PlayFX(); break;
                    case ScoreChangeModes.DECREASE: TeamD_NegativeFx.PlayFX(); break;
                }

                break;
        }


        UpdateScore();

        StartCoroutine(ScoreChangeCooldown());
    }

    IEnumerator ScoreChangeCooldown()
    {
        isOnCooldown = true;

        yield return new WaitForSeconds(cooldownduration);

        isOnCooldown = false;
    }

    public void UpdateScore()
    {
        TeamA_Display.text = TeamA.ToString();
        TeamB_Display.text = TeamB.ToString();
        TeamC_Display.text = TeamC.ToString();
        TeamD_Display.text = TeamD.ToString();
    }

    public void SaveData()
    {
       FileBrowser.ShowSaveDialog(null, null, FileBrowser.PickMode.Files, false, "C:\\", "SaveData.json", "Save As", "Save");
       StartCoroutine(ShowSaveDialogCoroutine());
    }

    IEnumerator ShowSaveDialogCoroutine()
    {
        yield return FileBrowser.WaitForSaveDialog(FileBrowser.PickMode.Files, false,null, null,"Save","Save");

        if (FileBrowser.Success)
            OnFileSaved(FileBrowser.Result);
    }

    void OnFileSaved(string[] filePaths)
    {
        SaveScore data = new SaveScore();

        data.TeamAScore = TeamA;
        data.TeamBScore = TeamB;
        data.TeamCScore = TeamC;
        data.TeamDScore = TeamD;

        string JSON = JsonUtility.ToJson(data);

        string filePath = filePaths[0];

        File.WriteAllText(filePath, JSON);

        SaveFX.PlayFX();
    }

    public void LoadData()
    {
        FileBrowser.ShowLoadDialog(null, null, FileBrowser.PickMode.Files, false, null, null, "Load", "Select");
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Load", "Select");

        if(FileBrowser.Success)
            OnFileLoaded(FileBrowser.Result);
    }

    void OnFileLoaded(string[] filePaths)
    {
        string filePath = filePaths[0];

        string Json = File.ReadAllText(filePath);

        SaveScore readData = JsonUtility.FromJson<SaveScore>(Json);

        TeamA = readData.TeamAScore;
        TeamB = readData.TeamBScore;
        TeamC = readData.TeamCScore;
        TeamD = readData.TeamDScore;

        TeamA_UpdateFx.PlayFX();
        TeamB_UpdateFx.PlayFX();
        TeamC_UpdateFx.PlayFX();
        TeamD_UpdateFx.PlayFX();

        UpdateScore();

        LoadFX.PlayFX();
    }

    public void OnExitClick()
    {
        Application.Quit();
    }
}
