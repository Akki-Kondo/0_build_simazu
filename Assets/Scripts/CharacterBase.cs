using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [Range(0, 2)] public float MoveSecond = 0.1f;�@//�ړ����x�̐ݒ�A�ő�l���2
    [SerializeField] protected RPGSceneManager RPGSceneManager; //�C���X�y�N�^�[����ݒ�ł���悤��

    Coroutine _moveCoroutine;
    [SerializeField] Vector3Int _pos;

    public MassEvent Event;�@//�C�x���g�f�[�^�̐ݒ�p

    public virtual Vector3Int Pos
    {
        get => _pos;
        set
        {
            if (_pos == value) return;

            if (RPGSceneManager.ActiveMap == null)
            {
                _pos = value;
            }
            else
            {
                if (_moveCoroutine != null)
                {
                    StopCoroutine(_moveCoroutine);
                    _moveCoroutine = null;
                }
                _moveCoroutine = StartCoroutine(MoveCoroutine(value));
            }
        }
    }

    public virtual void SetPosNoCoroutine(Vector3Int pos)
    {
        _pos = pos;
        transform.position = RPGSceneManager.ActiveMap.Grid.CellToWorld(pos);
        MoveCamera();
    }

    public bool IsMoving { get => _moveCoroutine != null; }
    public bool DoMoveCamera = false;

    [SerializeField] Direction _currentDir = Direction.Down;�@// ���݂̈ړ�������ۑ�

    public virtual Direction CurrentDir //�������ύX����邽�тɃA�j���[�V�������Z�b�g
    {
        get => _currentDir;
        set
        {
            if (_currentDir == value) return;
            _currentDir = value;
            SetDirAnimation(value);
        }
    }
    public virtual void SetDir(Vector3Int move)�@//Vector3�̐��l�����Ƃɍ��E���㉺���̕���������
    {
        if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
        {
            CurrentDir = move.x > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            CurrentDir = move.y > 0 ? Direction.Up : Direction.Down;
        }
    }

    protected Animator Animator { get => GetComponent<Animator>(); }�@//�ǂݎ���pAnimator�̒�`
    static readonly string TRIGGER_MoveDown = "MoveDownTrigger";�@//�e�����ړ��̃A�j���[�^�[�g���K�[�̖��O��ۑ�
    static readonly string TRIGGER_MoveLeft = "MoveLeftTrigger";
    static readonly string TRIGGER_MoveRight = "MoveRightTrigger";
    static readonly string TRIGGER_MoveUp = "MoveUpTrigger";

    protected void SetDirAnimation(Direction dir)�@//Direction�ɂ��ƂÂ��ĕ����ɍ��킹���A�j���[�V�����g���K�[���Z�b�g
    {
        if (Animator == null || Animator.runtimeAnimatorController == null) return;�@//Animator�R���|�[�l���g�ƃA�^�b�`�̊m�F

        string triggerName = null;�@//triggerName�̏�����
        switch (dir)�@//�^����ꂽdir�ɂ��ƂÂ���triggerName��ݒ�
        {
            case Direction.Up: triggerName = TRIGGER_MoveUp; break;
            case Direction.Down: triggerName = TRIGGER_MoveDown; break;
            case Direction.Left: triggerName = TRIGGER_MoveLeft; break;
            case Direction.Right: triggerName = TRIGGER_MoveRight; break;
            default: throw new System.NotImplementedException("");
        }
        Animator.SetTrigger(triggerName);
    }

    protected IEnumerator MoveCoroutine(Vector3Int pos)
    {
        _pos = pos;

        var startPos = transform.position;
        var goalPos = RPGSceneManager.ActiveMap.Grid.CellToWorld(pos);
        var t = 0f;
        while (t < MoveSecond)
        {
            yield return null;
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, goalPos, t / MoveSecond);
            MoveCamera();
        }
        _moveCoroutine = null;
    }

    protected virtual void Awake()�@//��������(_currentDir)�ɂ��ƂÂ��ăA�j���[�V�����̕�����ݒ�
    {
        SetDirAnimation(_currentDir);
    }

    protected virtual void Start()
    {
        if (RPGSceneManager == null) RPGSceneManager = Object.FindObjectOfType<RPGSceneManager>();

        _moveCoroutine = StartCoroutine(MoveCoroutine(Pos));

        var joinedMap = GetJoinedMap();
        if (joinedMap != null)
        {
            joinedMap.AddCharacter(this);
        }
        else
        {
            RPGSceneManager.ActiveMap.AddCharacter(this);
        }

    }

    protected void OnValidate()
    {
        if (RPGSceneManager != null && RPGSceneManager.ActiveMap != null)
        {
            transform.position = RPGSceneManager.ActiveMap.Grid.CellToWorld(Pos);
        }
        else if (transform.parent != null)
        {
            var map = transform.parent.GetComponent<Map>();
            if (map != null)
            {
                transform.position = map.Grid.CellToWorld(Pos);
            }
        }
    }

    public Map GetJoinedMap()
    {
        if (transform.parent != null)
        {
            return transform.parent.GetComponent<Map>();
        }
        else
        {
            return null;
        }
    }

    private Vector3 cameraOffset = new Vector3(0.32f, -0.64f, 0f); �@//�X�v���C�g�X���C�X���ɒ��S���炸�ꂽ���̃J���������l
    private void MoveCamera()
    {
        if(DoMoveCamera == true) Camera.main.transform.position = transform.position + Vector3.forward * -10 + cameraOffset;
    }

}