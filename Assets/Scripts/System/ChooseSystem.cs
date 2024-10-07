using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseSystem
{ 
    private static ChooseSystem instance=new ChooseSystem();
    public static ChooseSystem Instance => instance;

    public void GiveUp(Player player)
    {
        player.isDead = true;
        player.isOverTurn = true;
        TurnSystem.Instance.ExchangeTurn();
    }
    public void GiveUp(NPC npc)
    {
        npc.isDead = true;
        npc.isOverTurn = true;
        TurnSystem.Instance.ExchangeTurn();
    }

    public void AddChips(Player player, int chips)
    {
        player.data.leftChips -= chips;
        player.data.poolChips += chips;
        player.isOverTurn=true;
        TurnSystem.Instance.ExchangeTurn();
        TurnSystem.Instance.UpdatePanel();
    }
    public void AddChips(NPC npc, int chips)
    {
        npc.data.leftChips -= chips;
        npc.data.poolChips += chips;
        npc.isOverTurn=true;
        TurnSystem.Instance.ExchangeTurn();
        TurnSystem.Instance.UpdatePanel();
    }
    public void Check(Player player)
    {
        player.isOverTurn = true;
        TurnSystem.Instance.ExchangeTurn();
    }
    public void Check(NPC npc)
    {
        npc.isOverTurn = true;
        TurnSystem.Instance.ExchangeTurn();
    }


}
