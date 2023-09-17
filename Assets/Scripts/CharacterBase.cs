using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [Range(0, 2)] public float MoveSecond = 0.1f;　//移動速度の設定、最大値上限2
    [SerializeField] protected RPGSceneManager RPGSceneManager; //インスペクターから設定できるように

    Coroutine _moveCoroutine;
    [SerializeField] Vector3Int _pos;

    public MassEvent Event;　//イベントデータの設定用

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

    [SerializeField] Direction _currentDir = Direction.Down;　// 現在の移動方向を保存

    public virtual Direction CurrentDir //方向が変更されるたびにアニメーションをセット
    {
        get => _currentDir;
        set
        {
            if (_currentDir == value) return;
            _currentDir = value;
            SetDirAnimation(value);
        }
    }
    public virtual void SetDir(Vector3Int move)　//Vector3の数値をもとに左右か上下かの方向を決定
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

    protected Animator Animator { get => GetComponent<Animator>(); }　//読み取り専用Animatorの定義
    static readonly string TRIGGER_MoveDown = "MoveDownTrigger";　//各方向移動のアニメータートリガーの名前を保存
    static readonly string TRIGGER_MoveLeft = "MoveLeftTrigger";
    static readonly string TRIGGER_MoveRight = "MoveRightTrigger";
    static readonly string TRIGGER_MoveUp = "MoveUpTrigger";

    protected void SetDirAnimation(Direction dir)　//Directionにもとづいて方向に合わせたアニメーショントリガーをセット
    {
        if (Animator == null || Animator.runtimeAnimatorController == null) return;　//Animatorコンポーネントとアタッチの確認

        string triggerName = null;　//triggerNameの初期化
        switch (dir)　//与えられたdirにもとづいてtriggerNameを設定
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

    protected virtual void Awake()　//初期方向(_currentDir)にもとづいてアニメーションの方向を設定
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

    private Vector3 cameraOffset = new Vector3(0.32f, -0.64f, 0f); 　//スプライトスライス時に中心からずれた分のカメラ調整値
    private void MoveCamera()
    {
        if(DoMoveCamera == true) Camera.main.transform.position = transform.position + Vector3.forward * -10 + cameraOffset;
    }

}