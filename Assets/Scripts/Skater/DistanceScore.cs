using UnityEngine;

public class DistanceScore : MonoBehaviour
{
    public float distance;
    public float score;
    public float highScore;
    float startZ;
    bool isPlaying = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startZ = transform.position.z;

        highScore = PlayerPrefs.GetFloat("HighScore", 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaying) return;
        distance = transform.position.z - startZ;
        score = Mathf.FloorToInt(distance / 10f);
    }

    public void StopScoring()
    {
        isPlaying = false;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }
}
