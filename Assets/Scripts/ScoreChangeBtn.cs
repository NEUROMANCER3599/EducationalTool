using UnityEngine;

public class ScoreChangeBtn : MonoBehaviour
{
    public ScoreManager ScoreCore;
    public Teams team;
    public ScoreChangeModes ScoreChangeMode;

    public void OnClicked()
    {
        ScoreCore.ChangeScore(team, ScoreChangeMode);
    }
}
