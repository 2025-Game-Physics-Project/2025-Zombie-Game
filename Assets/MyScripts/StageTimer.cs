using UnityEngine;
using UnityEngine.SceneManagement;

public class StageTimer : MonoBehaviour
{
    public float timeLimit = 180f;   // 3분=180, 5분=300 이런 식으로 Scene마다 다르게

    public float ElapsedTime { get; private set; }
    public bool IsTimeOver => ElapsedTime >= timeLimit;

    private bool isRunning = true;

    private GUIStyle guiStyle; //GUI 표시용.

    private void Awake()
    {
        // 글자 스타일 준비
        guiStyle = new GUIStyle();
        guiStyle.fontSize = 24;
        guiStyle.normal.textColor = Color.white;
        guiStyle.fontStyle = FontStyle.Bold;
    }

    private void OnGUI() //화면 표시
    {
        float remaining = timeLimit - ElapsedTime;
        if (remaining < 0) {
            remaining = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);

        string timeText = $"{minutes:00}:{seconds:00}";

        // 화면 좌측 상단에 표시
        GUI.Label(new Rect(60, 40, 150, 40), timeText, guiStyle);
    }

    private void Update()
    {
        if (!isRunning) return;

        ElapsedTime += Time.deltaTime;
    }

    public void Stop()
    {
        isRunning = false;
    }
}
