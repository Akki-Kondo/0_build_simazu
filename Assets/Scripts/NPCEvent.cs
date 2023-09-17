using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MassEvent/NPC Event")]�@//NPC�C�x���g�����쐬�̂��߂̃��j���[�ǉ�
public class NPCEvent : MassEvent
{
    [TextArea(3, 15)] public string Message;

    public override void Exec(RPGSceneManager manager)
    {
        manager.ShowMessageWindow(Message);
    }
}