using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "MassEvent/Move Map")]�@//Unity���j���[�ɒǉ�
public class MoveMapEvent : MassEvent�@//MoveMapEvent�N���X�̒�`�A�v���C���[������̃^�C���Ɉړ������Ƃ��ɐV�����}�b�v�Ɉړ�����C�x���g
{
    public Map MoveMapPrefab;�@//�v���C���[���ړ�����V�����}�b�v�̃v���n�u��ێ�
    public TileBase StartPosTile;�@//�v���C���[���V�����}�b�v�Ɉړ������Ƃ��̊J�n�ʒu�������^�C����ێ�
    public Direction StartDirection;�@//�v���C���[���V�����}�b�v�Ɉړ�������̊J�n������ێ�

    public override void Exec(RPGSceneManager manager)
    {
        Object.Destroy(manager.ActiveMap.gameObject);�@// ���݂̃}�b�v�̃Q�[���I�u�W�F�N�g���폜
        manager.ActiveMap = Object.Instantiate(MoveMapPrefab);�@//�V�����}�b�v�̃v���n�u���C���X�^���X�����āA�A�N�e�B�u�ȃ}�b�v�Ƃ��Đݒ�

        if (manager.ActiveMap.FindMassEventPos(StartPosTile, out var pos))�@//�V�����}�b�v��ł̊J�n�ʒu���������A�ݒ�
        {
            manager.Player.SetPosNoCoroutine(pos);
            manager.Player.CurrentDir = StartDirection;
        }
    }
}