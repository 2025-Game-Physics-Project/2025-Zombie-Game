using UnityEngine;
using UnityEngine.SceneManagement;

public class StageCheckpoint : MonoBehaviour
{
    public string nextSceneName = ""; //다음 Scene 설정. 비워두면 현재 씬 다시 로드.

    public bool isLastStage = false; //-> 마지막 스테이지 체크. 아래는 굿엔딩/배드엔딩 씬 이름
    public string goodEndingSceneName = "GoodEnding";
    public string badEndingSceneName = "BadEnding";

    public StageTimer stageTimer; //타이머. inspector에 넣어놓는다.

    private void OnTriggerEnter(Collider other)
    {
        // Player만 반응
        if (!other.CompareTag("Player"))
            return;

        // 나중에 여기서 클리어 연출, 사운드, UI 등 넣을 수 있음
        Debug.Log("Checkpoint 도달!");

        if (stageTimer == null) //타이머 없으면 직접 찾는다.
        {
            stageTimer = FindObjectOfType<StageTimer>();
        }

        bool timeOver = false;

        if (stageTimer != null)
        {
            stageTimer.Stop();             // 타이머 멈추기
            timeOver = stageTimer.IsTimeOver;
            Debug.Log($"[Checkpoint] Stage time = {stageTimer.ElapsedTime:F1} / Limit = {stageTimer.timeLimit}");
        }

        // 제한시간 초과했다면 GameManager에 카운트 +1
        if (timeOver && GameManager.Instance != null)
        {
            GameManager.Instance.AddOverTime();
        }

        if (isLastStage)
        {
            LoadEndingScene();
        }
        else
        {
            LoadNextStage();
        }
    }

    private void LoadNextStage()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            // 현재 씬 다시 로드 (지금은 임시로 사용)
            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.buildIndex);
        }
        else
        {
            // 나중에 스테이지 분리되면 여기만 씬 이름으로 변경해주면 됨
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void LoadEndingScene()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager 인스턴스가 없습니다. 임시로 굿엔딩 로드.");
            SceneManager.LoadScene(goodEndingSceneName);
            return;
        }

        int count = GameManager.Instance.overTimeCount;
        Debug.Log($"[Ending] OverTimeCount = {count}");

        if (count >= 2)
        {
            SceneManager.LoadScene(badEndingSceneName);
        }
        else
        {
            SceneManager.LoadScene(goodEndingSceneName);
        }
    }

}
