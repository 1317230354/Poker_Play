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
            Debug.Log("���عؿ���ͼ");
            Application.Quit();
        });
        restartBtn.onClick.AddListener(() =>
        {
            DataBase.Instance.ReSet();
            CoreSystem.Instance.DestroyInstance();
            TurnSystem.Instance.DestroyInstance();
            DataReader.Instance.DestroyInstance();
            SceneManager.LoadScene(0, LoadSceneMode.Single); //���¼���Ĭ�ϳ���
        });
    }
}
