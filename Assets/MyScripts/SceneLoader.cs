using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요
#if UNITY_EDITOR // 유니티에서 실행 했을경우 exit 적용
using UnityEditor;
#endif

public class SceneLoader : MonoBehaviour
{
    public void StartGame()
    {
        // "GameScene"은 실제 게임 플레이 씬의 이름으로 변경해야 합니다.
        SceneManager.LoadScene("Feature_GameMap2");
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        // 유니티 에디터에서 실행 중일 때 Exit
        EditorApplication.isPlaying = false;
#else
        // 빌드된 애플리케이션에서 실행 중일 때 Exit
        Application.Quit();
#endif
    }
}