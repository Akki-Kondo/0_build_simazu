using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RPGSceneManager : MonoBehaviour
{
    public Player Player;
    public Map ActiveMap;
    public MessageWindow MessageWindow;
    public Menu Menu;
    public Button[] menuButtons; // ���j���[��6�̃{�^���̎Q��

    public InputActionAsset playerControls;
    public Vector3 _currentInput;
    private InputAction _moveAction;
    private InputAction X_button;
    private InputAction B_button;

    public void Awake()�@//InputSystem�̐ݒ�
    {
        var gameplayActionMap = playerControls.FindActionMap("PlayerControls");
        _moveAction = gameplayActionMap.FindAction("Move"); // Move�A�N�V�������擾
        X_button = gameplayActionMap.FindAction("X_button");�@//X button�A�N�V�������擾
        B_button = gameplayActionMap.FindAction("B_button");�@//B button�A�N�V�������擾

        _moveAction.performed += context =>
        {
            Vector2 input2D = context.ReadValue<Vector2>();
            _currentInput = new Vector3(input2D.x, input2D.y, 0);
        };
        _moveAction.canceled += context => _currentInput = Vector3.zero; // ���͂��I�������Ƃ��̓����ݒ�

        X_button.performed += ctx => OpenMenu();�@// X button (Shift�L�[) �Ń��j���[���J��
        B_button.performed += ctx => GoBack();�@// B button (B�L�[) �ň�O�̕\���ɖ߂�

    }

    void OnEnable()
    {
        _moveAction.Enable();
        X_button.Enable();
        B_button.Enable();
    }

    void OnDisable()
    {
        _moveAction.Disable();
        X_button.Disable();
        B_button.Disable();
    }


    private void GoBack()
    {
        // �����ň�O�̕\���ɖ߂郍�W�b�N������
        // ��: �T�u���j���[���J���Ă���ꍇ�A���C�����j���[�ɖ߂�
    }


    Coroutine _currentCoroutine;

    void Start()
    {
        _moveAction.Enable();
        _currentCoroutine = StartCoroutine(MovePlayer());

        string[] menuOptions = { "��", "����", "�@�p", "����", "����", "�L�^" };�@// ���j���[�{�^���̃e�L�X�g��ݒ�
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i].GetComponentInChildren<Text>().text = menuOptions[i];
        }
    }

    IEnumerator MovePlayer()�@//�v���C���[�̈ړ��A�}�b�v�ړ��̐���B����̈ʒu�Ɉړ������Ƃ���massData��
    {
        while (true)�@//�v���C���[�ړ��̊Ď�
        {
            Vector3Int move = Vector3Int.RoundToInt(_currentInput); // �v���C���[�̓��͂�Vector3Int�ɕϊ�
            if (move != Vector3Int.zero)�@//�v���C���[���ړ����Ă���ꍇ�A�ړ���̈ʒu���擾�A�ړ���̈ʒu�ɃC�x���g������ꍇ�͎��s
            {
                var movedPos = Player.Pos + move;
                var massData = ActiveMap.GetMassData(movedPos);
                Player.SetDir(move);�@//move�ϐ���Player�ɃZ�b�g
                if (massData.isMovable)�@//�ړ���̈ʒu���ړ��\�ȏꍇ�A�ʒu���X�V
                {
                    Player.Pos = movedPos;
                    yield return new WaitWhile(() => Player.IsMoving);

                    if (massData.massEvent != null)�@//massEvent�ɉ�������ꍇ�A�C�x���g�����s(�I�u�W�F�N�g�������Ƃ��ēn��)
                    {
                        massData.massEvent.Exec(this);
                    }

                }
                else if (massData.character != null && massData.character.Event != null)�@//�ړ���̈ʒu���ړ��s�ŁA�L�����N�^�[������ꍇ�A�C�x���g�����s
                {
                    massData.character.Event.Exec(this);
                }
            }
            yield return new WaitWhile(() => IsPauseScene);
        }

        if (X_button.triggered)�@//�ړ�����X�{�^���Ń��j���[�J���A�~�܂�
        {
            OpenMenu();
        }
    }

    public void ShowMessageWindow(string message)
    {
        MessageWindow.StartMessage(message);
    }

    public bool IsPauseScene�@//��b���A���j���[���J���Ă���Ƃ��A�v���C���[���ړ��ł��Ȃ��悤�ɂ���
    {
        get
        {
            return !MessageWindow.IsEndMessage || Menu.DoOpen;
        }
    }

    public void OpenMenu()�@//���j���[���J��
    {
        Menu.Open();
    }
}
