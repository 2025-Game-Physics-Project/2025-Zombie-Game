using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 제한 시간 초과한 스테이지 수
    public int overTimeCount = 0;

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  // Scene이 바뀌어도 살아있게
    }

    public void AddOverTime()
    {
        overTimeCount++;
        Debug.Log($"[GameManager] OverTimeCount = {overTimeCount}");
    }

    public void ResetGame()
    {
        overTimeCount = 0;

        // 처음 스테이지 이름으로 바꿀것
        SceneManager.LoadScene("Stage1");
    }
}
