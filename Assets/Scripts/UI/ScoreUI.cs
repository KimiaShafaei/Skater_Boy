using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public DistanceScore distanceScore;
    public TMP_Text scoreText;
    
    void Update()
    {
        if (distanceScore == null) return;
        scoreText.text = distanceScore.score.ToString();
    }
}
