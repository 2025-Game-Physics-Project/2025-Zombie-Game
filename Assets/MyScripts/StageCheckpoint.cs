using UnityEngine;
using UnityEngine.SceneManagement;

public class StageCheckpoint : MonoBehaviour
{
    public string nextSceneName = ""; //다음 Scene 설정. 비워두면 현재 씬 다시 로드.

    private void OnTriggerEnter(Collider other)
    {
        // Player만 반응
        if (!other.CompareTag("Player"))
            return;

        // 나중에 여기서 클리어 연출, 사운드, UI 등 넣을 수 있음
        Debug.Log("Checkpoint 도달!");

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
}
