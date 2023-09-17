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
    public Button[] menuButtons; // メニューの6つのボタンの参照

    public InputActionAsset playerControls;
    public Vector3 _currentInput;
    private InputAction _moveAction;
    private InputAction X_button;
    private InputAction B_button;

    public void Awake()　//InputSystemの設定
    {
        var gameplayActionMap = playerControls.FindActionMap("PlayerControls");
        _moveAction = gameplayActionMap.FindAction("Move"); // Moveアクションを取得
        X_button = gameplayActionMap.FindAction("X_button");　//X buttonアクションを取得
        B_button = gameplayActionMap.FindAction("B_button");　//B buttonアクションを取得

        _moveAction.performed += context =>
        {
            Vector2 input2D = context.ReadValue<Vector2>();
            _currentInput = new Vector3(input2D.x, input2D.y, 0);
        };
        _moveAction.canceled += context => _currentInput = Vector3.zero; // 入力が終了したときの動作を設定

        X_button.performed += ctx => OpenMenu();　// X button (Shiftキー) でメニューを開く
        B_button.performed += ctx => GoBack();　// B button (Bキー) で一つ前の表示に戻る

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
        // ここで一つ前の表示に戻るロジックを実装
        // 例: サブメニューが開いている場合、メインメニューに戻る
    }


    Coroutine _currentCoroutine;

    void Start()
    {
        _moveAction.Enable();
        _currentCoroutine = StartCoroutine(MovePlayer());

        string[] menuOptions = { "力", "装備", "法術", "隊列", "道具", "記録" };　// メニューボタンのテキストを設定
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i].GetComponentInChildren<Text>().text = menuOptions[i];
        }
    }

    IEnumerator MovePlayer()　//プレイヤーの移動、マップ移動の制御。特定の位置に移動したときにmassDataに
    {
        while (true)　//プレイヤー移動の監視
        {
            Vector3Int move = Vector3Int.RoundToInt(_currentInput); // プレイヤーの入力をVector3Intに変換
            if (move != Vector3Int.zero)　//プレイヤーが移動している場合、移動後の位置を取得、移動後の位置にイベントがある場合は実行
            {
                var movedPos = Player.Pos + move;
                var massData = ActiveMap.GetMassData(movedPos);
                Player.SetDir(move);　//move変数をPlayerにセット
                if (massData.isMovable)　//移動後の位置が移動可能な場合、位置情報更新
                {
                    Player.Pos = movedPos;
                    yield return new WaitWhile(() => Player.IsMoving);

                    if (massData.massEvent != null)　//massEventに何かある場合、イベントを実行(オブジェクトを引数として渡す)
                    {
                        massData.massEvent.Exec(this);
                    }

                }
                else if (massData.character != null && massData.character.Event != null)　//移動後の位置が移動不可で、キャラクターがいる場合、イベントを実行
                {
                    massData.character.Event.Exec(this);
                }
            }
            yield return new WaitWhile(() => IsPauseScene);
        }

        if (X_button.triggered)　//移動中にXボタンでメニュー開く、止まる
        {
            OpenMenu();
        }
    }

    public void ShowMessageWindow(string message)
    {
        MessageWindow.StartMessage(message);
    }

    public bool IsPauseScene　//会話中、メニューを開いているとき、プレイヤーが移動できないようにする
    {
        get
        {
            return !MessageWindow.IsEndMessage || Menu.DoOpen;
        }
    }

    public void OpenMenu()　//メニューを開く
    {
        Menu.Open();
    }
}
