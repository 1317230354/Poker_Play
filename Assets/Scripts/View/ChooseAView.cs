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
            //标记为放弃

            //切换回合
            ChooseSystem.Instance.GiveUp(TurnSystem.Instance.player);
            Destroy(gameObject);
        });
        followBtn.onClick.AddListener(() =>
        {
            //计算筹码差，投入筹码到筹码池

            //切换回合
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
            //添加指定数量的筹码到筹码池（一定要大于筹码差）

            //切换回合
            int chips = TurnSystem.Instance.player.data.selectChips;
            if (chips>=Mathf.Abs(TurnSystem.Instance.CalculateGap(0))|| chips == TurnSystem.Instance.player.data.leftChips)
            {
                ChooseSystem.Instance.AddChips(TurnSystem.Instance.player,chips);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("不能投入小于上一个角色的筹码");
            }
            
        });
    }
}
