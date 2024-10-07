using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    public Button backBtn;
    public Image table;
    public Button restartBtn;
    public void SetView(string path)
    {
        table.sprite = Resources.Load<Sprite>(path);
        backBtn.onClick.AddListener(() =>
        {
            Debug.Log("返回关卡地图");
            Application.Quit();
        });
        restartBtn.onClick.AddListener(() =>
        {
            DataBase.Instance.ReSet();
            CoreSystem.Instance.DestroyInstance();
            TurnSystem.Instance.DestroyInstance();
            DataReader.Instance.DestroyInstance();
            SceneManager.LoadScene(0, LoadSceneMode.Single); //重新加载默认场景
        });
    }
}
