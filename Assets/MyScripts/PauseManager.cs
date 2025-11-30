using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    // 유니티 인스펙터 창에서 Pause Panel 연결
    public GameObject pausePanel;
    private bool isPaused = false;
    // 플레이어 카메라/회전 스크립트 컴포넌트 (MouseLook.cs 등으로 가정)
    public MonoBehaviour playerLookScript;

    // 게임 중 표시되는 HUD 전체 (TextMeshPro 포함)
    public GameObject gameHUD;


    void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                // 일시 정지 상태라면 -> 게임 계속
                ResumeGame();
            }
            else
            {
                // 게임 진행 중이라면 -> 일시 정지
                PauseGame();
            }
        }
    }

    // '게임 계속' 버튼에 연결
    public void ResumeGame()
    {
        // 1. UI 및 HUD 제어
        pausePanel.SetActive(false);
        gameHUD.SetActive(true); // HUD 다시 활성화

        // 2. 마우스 움직임 제어
        playerLookScript.enabled = true; // 마우스 회전 스크립트 활성화
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 다시 잠금
        Cursor.visible = false;

        // 3. 게임 재개
        Time.timeScale = 1f;
        isPaused = false;
    }

    // ESC 키 입력으로 호출
    public void PauseGame()
    {
        // 1. UI 및 HUD 제어
        pausePanel.SetActive(true);
        gameHUD.SetActive(false); // HUD 비활성화

        // 2. 마우스 움직임 제어
        playerLookScript.enabled = false; // 마우스 회전 스크립트 비활성화
        Cursor.lockState = CursorLockMode.None; // 마우스 커서 해제
        Cursor.visible = true;

        // 3. 게임 일시 정지
        Time.timeScale = 0f;
        isPaused = true;
    }

    // '게임 나가기' 버튼에 연결 (이전과 동일)
    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}