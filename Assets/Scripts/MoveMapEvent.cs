using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "MassEvent/Move Map")]　//Unityメニューに追加
public class MoveMapEvent : MassEvent　//MoveMapEventクラスの定義、プレイヤーが特定のタイルに移動したときに新しいマップに移動するイベント
{
    public Map MoveMapPrefab;　//プレイヤーが移動する新しいマップのプレハブを保持
    public TileBase StartPosTile;　//プレイヤーが新しいマップに移動したときの開始位置を示すタイルを保持
    public Direction StartDirection;　//プレイヤーが新しいマップに移動した後の開始方向を保持

    public override void Exec(RPGSceneManager manager)
    {
        Object.Destroy(manager.ActiveMap.gameObject);　// 現在のマップのゲームオブジェクトを削除
        manager.ActiveMap = Object.Instantiate(MoveMapPrefab);　//新しいマップのプレハブをインスタンス化して、アクティブなマップとして設定

        if (manager.ActiveMap.FindMassEventPos(StartPosTile, out var pos))　//新しいマップ上での開始位置を検索し、設定
        {
            manager.Player.SetPosNoCoroutine(pos);
            manager.Player.CurrentDir = StartDirection;
        }
    }
}