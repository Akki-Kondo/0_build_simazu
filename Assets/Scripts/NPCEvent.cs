using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MassEvent/NPC Event")]　//NPCイベント多数作成のためのメニュー追加
public class NPCEvent : MassEvent
{
    [TextArea(3, 15)] public string Message;

    public override void Exec(RPGSceneManager manager)
    {
        manager.ShowMessageWindow(Message);
    }
}