using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseBView : MonoBehaviour
{
    public Button checkBtn, addBtn;
    private void Start()
    {
        checkBtn.onClick.AddListener(() =>
        {
            //�л�����һ��ɫ
            ChooseSystem.Instance.Check(TurnSystem.Instance.player);
            Destroy(gameObject);
        });
        addBtn.onClick.AddListener(() =>
        {
            //Ͷ��ָ�������ĳ��뵽�����

            //�л�����һλ��ɫ
            ChooseSystem.Instance.AddChips(TurnSystem.Instance.player, TurnSystem.Instance.player.data.selectChips);
            Destroy(gameObject);

        });
    }
}
