using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction�@//�񋓌^�łS�̕������`
{
    Up,
    Down,
    Left,
    Right,
}

public class Player : CharacterBase�@//Player�N���X�̒�`�BCharacterBase����p��
{
    public BattleParameter InitialBattleParameter;�@//�p�����[�^�[�̑g�ݍ���
    public BattleParameterBase BattleParameter;

    protected override void Start()�@//�J�����̓����̗L������start���]�b�h�����s
    {
        DoMoveCamera = true;
        base.Start();
        InitialBattleParameter.Data.CopyTo(BattleParameter);�@//�p�����[�^�[�̑g�ݍ���
    }
}