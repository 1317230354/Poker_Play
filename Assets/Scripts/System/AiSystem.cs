using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AiSystem
{

    public static void PlanA(NPC npc)
    {
        ChooseSystem chooseSystem=ChooseSystem.Instance;
        if (chooseSystem == null)
            Debug.Log("û��ʵ��");

        int choose = Random.Range(0, 2);
        switch(choose)
        {
            case 0:
                chooseSystem.Check(npc);
                break;
            case 1:
                choose=Random.Range(0, 2);
                if(choose==0)
                {
                    chooseSystem.AddChips(npc, npc.data.leftChips / 2);
                }
                else
                {
                    chooseSystem.AddChips(npc, npc.data.leftChips);
                }
                break;
        }
    }
    public static void PlanB(NPC npc)
    {
        ChooseSystem chooseSystem = ChooseSystem.Instance;
        if (chooseSystem == null)
            Debug.Log("û��ʵ��");

        int choose = Random.Range(0, 3);
        int gap=0;
        switch (choose)
        {
            case 0:
                chooseSystem.GiveUp(npc);
                //Debug.Log(npc.view.name+"����");
                break;
            case 1:
                gap = Mathf.Abs(TurnSystem.Instance.CalculateGap(npc));
                //Debug.Log(npc.view.name + "���"+gap);
                if (gap>npc.data.leftChips)
                {
                    chooseSystem.AddChips(npc,npc.data.leftChips );
                }
                else
                {
                    chooseSystem.AddChips(npc,gap);
                }
                break;
            case 2:
                choose = Random.Range(0, 2);
                if (choose == 0)
                {
                    gap = Mathf.Abs(TurnSystem.Instance.CalculateGap(npc));
                    //Debug.Log(npc.view.name + "���" + gap);
                    if (gap>npc.data.leftChips/2)
                    {
                        chooseSystem.AddChips(npc, npc.data.leftChips);
                    }
                    else
                    {
                        chooseSystem.AddChips(npc, npc.data.leftChips / 2);
                    }
                    
                }
                else
                {
                    chooseSystem.AddChips(npc, npc.data.leftChips);
                }
                break;
        }
    }


    /// <summary>
    /// �������д�7������ѡȡ5�ŵ����
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="combinationSize"></param>
    /// <returns></returns>
    public static List<List<CardData>> GetCombinations(List<CardData> cards, int combinationSize)
    {
        List<List<CardData>> result = new List<List<CardData>>();
        GenerateCombinations(cards, new List<CardData>(), 0, combinationSize, result);
        return result;
    }

    // �ݹ�������ϵĸ�������
    private static void GenerateCombinations(List<CardData> cards, List<CardData> currentCombination, int start, int combinationSize, List<List<CardData>> result)
    {
        // ����ϴﵽָ����Сʱ������������б�
        if (currentCombination.Count == combinationSize)
        {
            result.Add(new List<CardData>(currentCombination));
            return;
        }

        // �ݹ����ɲ�ͬ�����
        for (int i = start; i < cards.Count; i++)
        {
            currentCombination.Add(cards[i]);
            GenerateCombinations(cards, currentCombination, i + 1, combinationSize, result);
            currentCombination.RemoveAt(currentCombination.Count - 1); // ����
        }
    }
}
