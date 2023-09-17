using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction　//列挙型で４つの方向を定義
{
    Up,
    Down,
    Left,
    Right,
}

public class Player : CharacterBase　//Playerクラスの定義。CharacterBaseから継承
{
    public BattleParameter InitialBattleParameter;　//パラメーターの組み込み
    public BattleParameterBase BattleParameter;

    protected override void Start()　//カメラの動きの有効化とstartメゾッドを実行
    {
        DoMoveCamera = true;
        base.Start();
        InitialBattleParameter.Data.CopyTo(BattleParameter);　//パラメーターの組み込み
    }
}