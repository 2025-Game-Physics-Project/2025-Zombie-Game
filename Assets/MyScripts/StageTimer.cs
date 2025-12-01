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
        guiStyle.fontSize = 36;
        guiStyle.normal.textColor = Color.white;
        guiStyle.fontStyle = FontStyle.Bold;
    }

    private void OnGUI() //화면 표시
    {
        float remaining = timeLimit - ElapsedTime;

        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);

        string timeText = $"{minutes:00}:{seconds:00}";
        Vector2 size = guiStyle.CalcSize(new GUIContent(timeText));
        float x = (Screen.width - size.x) * 0.5f;
        float y = 64;

        if (remaining < 0) {
            remaining = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (remaining <= 61)
        {
            guiStyle.fontSize = 40;
            guiStyle.normal.textColor = Color.red;
        }
        if (remaining <= 31)
        {
            guiStyle.fontSize = 44;
            x += Random.Range(-5.0f, 5.0f);
            y += Random.Range(-5.0f, 5.0f);
        }

        if (remaining <= 16)
        {
            guiStyle.fontSize = 52;
        }

        GUI.Label(new Rect(x, y, size.x, size.y), timeText, guiStyle);
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
