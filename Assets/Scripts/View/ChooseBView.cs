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
            //切换到下一角色
            ChooseSystem.Instance.Check(TurnSystem.Instance.player);
            Destroy(gameObject);
        });
        addBtn.onClick.AddListener(() =>
        {
            //投入指定数量的筹码到筹码池

            //切换到下一位角色
            ChooseSystem.Instance.AddChips(TurnSystem.Instance.player, TurnSystem.Instance.player.data.selectChips);
            Destroy(gameObject);

        });
    }
}
