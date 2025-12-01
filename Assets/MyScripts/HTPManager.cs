using UnityEngine;
using UnityEngine.SceneManagement;

public class HTPManager : MonoBehaviour
{
    public GameObject HTP_panel;
    public GameObject Mainmenu;



    public void HTP_open()
    {
        Mainmenu.SetActive(false);
        HTP_panel.SetActive(true);
        
    }

    public void HTP_exit()
    {
        Mainmenu.SetActive(true);
        HTP_panel.SetActive(false);

    }
}