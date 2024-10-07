using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseAView : MonoBehaviour
{
    public Button giveUpBtn, followBtn, addBtn;

    private void Start()
    {
        giveUpBtn.onClick.AddListener(() =>
        {
            //���Ϊ����

            //�л��غ�
            ChooseSystem.Instance.GiveUp(TurnSystem.Instance.player);
            Destroy(gameObject);
        });
        followBtn.onClick.AddListener(() =>
        {
            //�������Ͷ����뵽�����

            //�л��غ�
            int gap = Mathf.Abs(TurnSystem.Instance.CalculateGap(0));
            int left = TurnSystem.Instance.player.data.leftChips;
            if (gap>left)
            {
                ChooseSystem.Instance.AddChips(TurnSystem.Instance.player, left);
            }
            else
            {
                ChooseSystem.Instance.AddChips(TurnSystem.Instance.player, gap);
            }
            Destroy(gameObject);
        });
        addBtn.onClick.AddListener(() =>
        {
            //���ָ�������ĳ��뵽����أ�һ��Ҫ���ڳ���

            //�л��غ�
            int chips = TurnSystem.Instance.player.data.selectChips;
            if (chips>=Mathf.Abs(TurnSystem.Instance.CalculateGap(0))|| chips == TurnSystem.Instance.player.data.leftChips)
            {
                ChooseSystem.Instance.AddChips(TurnSystem.Instance.player,chips);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("����Ͷ��С����һ����ɫ�ĳ���");
            }
            
        });
    }
}
