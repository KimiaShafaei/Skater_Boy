using UnityEngine;

[System.Serializable]
public class HighScoreData
{
    public float[] scores = new float[5];
}

public class DistanceScore : MonoBehaviour
{
    public float distance;
    public float score;
    public HighScoreData highScoreData = new HighScoreData();
    float startZ;
    bool isPlaying = true;

    void Start()
    {
        startZ = transform.position.z;

        LoadHighScores();
    }

    void Update()
    {
        if (!isPlaying) return;
        distance = transform.position.z - startZ;
        score = Mathf.FloorToInt(distance / 10f);
    }

    void LoadHighScores()
    {
        if (PlayerPrefs.HasKey("HighScores"))
        {
            string json = PlayerPrefs.GetString("HighScores");
            highScoreData = JsonUtility.FromJson<HighScoreData>(json);
        }
    }

    void SaveHighScores()
    {
        string json = JsonUtility.ToJson(highScoreData);
        PlayerPrefs.SetString("HighScores", json);
        PlayerPrefs.Save();
    }


    public void StopScoring()
    {
        isPlaying = false;

        for (int i = 0; i < highScoreData.scores.Length; i++)
        {
            if (score > highScoreData.scores[i])
            {
                for (int j = highScoreData.scores.Length - 1; j > i; j--)
                {
                    highScoreData.scores[j] = highScoreData.scores[j - 1];
                }
                highScoreData.scores[i] = score;
                break;
            }
        }
        SaveHighScores();
    }
}
