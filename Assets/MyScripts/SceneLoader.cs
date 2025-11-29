using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요

public class SceneLoader : MonoBehaviour
{
    public void StartGame()
    {
        // "GameScene"은 실제 게임 플레이 씬의 이름으로 변경해야 합니다.
        SceneManager.LoadScene("Feature_GameMap3");
    }

    public void QuitGame()
    {
        Application.Quit(); // 게임 종료
    }
}