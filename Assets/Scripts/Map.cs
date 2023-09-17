using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map: MonoBehaviour
{
    public Grid Grid { get => GetComponent<Grid>(); }
    Dictionary<string, Tilemap> _tilemaps;

    readonly static string BACKGROND_TILEMAP_NAME = "BackGround";
    readonly static string NONE_OBJECTS_TILEMAP_NAME = "NoneObjects";
    readonly static string OBJECTS_TILEMAP_NAME = "Objects";
    readonly static string HIDEROAD_TILEMAP_NAME = "HideRoad";
    readonly static string EVENT_BOX_TILEMAP_NAME = "EventBox";

    [SerializeField] List<MassEvent> _massEvents; //_massEvents変数の宣言、インスペクターでの表示

    HashSet<CharacterBase> _characters = new HashSet<CharacterBase>();　//？キャラクタ位置記憶用に追加？
    public void AddCharacter(CharacterBase character)
    {
        if (!_characters.Contains(character) && character != null)
        {
            _characters.Add(character);
        }
    }

    public CharacterBase GetCharacter(Vector3Int pos)
    {
        return _characters.FirstOrDefault(_c => _c.Pos == pos);
    }
    

    public MassEvent FindMassEvent(TileBase tile)　//リスト(_massEvent)から引数(tile)と一致するオブジェクト(MassEvent)を返す
    {
        return _massEvents.Find(_c => _c.Tile == tile);
    }

    public bool FindMassEventPos(TileBase tile, out Vector3Int pos)  //イベント用タイル(tile)がeventLayer内でどの位置にあるか検索
    {
        var eventLayer = _tilemaps[EVENT_BOX_TILEMAP_NAME];
        var renderer = eventLayer.GetComponent<Renderer>();
        var min = eventLayer.LocalToCell(renderer.bounds.min);　//タイルマップの左下の角と右上の角の位置をセル座標に変換
        var max = eventLayer.LocalToCell(renderer.bounds.max);
        pos = Vector3Int.zero;
        for (pos.y = min.y; pos.y < max.y; ++pos.y)　//範囲内のセルを走査
        {
            for (pos.x = min.x; pos.x < max.x; ++pos.x)
            {
                var t = eventLayer.GetTile(pos);　//セルの位置タイルを取得
                if (t == tile) return true;　//目的のタイルが見つかった場合tureを返す
            }
        }
        return false;
    }

    private void Awake()
    {
        _tilemaps = new Dictionary<string, Tilemap>();
        foreach (var tilemap in Grid.GetComponentsInChildren<Tilemap>())
        {
            _tilemaps.Add(tilemap.name, tilemap);
        }

        _tilemaps[EVENT_BOX_TILEMAP_NAME].gameObject.SetActive(false);　//EventBoxを非表示にする

        AddCharacter(Object.FindObjectOfType<Player>());

    }

    public Vector3 GetWorldPos(Vector3Int pos)
    {
        return Grid.CellToWorld(pos);
    }

    public class Mass
    {
        public bool isMovable;
        public TileBase eventTile;
        public MassEvent massEvent;
        public CharacterBase character;
    }

    public Mass GetMassData(Vector3Int pos)　//Massオブジェクトのデータ取得と、各タイルマップ移動可能かどうかの設定
    {
        var mass = new Mass();
        mass.eventTile = _tilemaps[EVENT_BOX_TILEMAP_NAME].GetTile(pos);
        mass.isMovable = true;　//デフォルトでは移動可能と設定
        mass.character = GetCharacter(pos);　//各キャラクタの位置記録

        if (mass.character != null)　//キャラクターがいる場合は移動できない
        {
            mass.isMovable = false;
        }
        else if (mass.eventTile != null)　//eventTileに何かある場合はイベント(eventTile)を設定
        {
            mass.massEvent = FindMassEvent(mass.eventTile);
        }
        else if (_tilemaps[OBJECTS_TILEMAP_NAME].GetTile(pos))　//オブジェクトタイルマップに何かある場合場合は移動不可
        {
            mass.isMovable = false;
        }
        else if (_tilemaps[BACKGROND_TILEMAP_NAME].GetTile(pos) == null)　//背景タイルマップに何もない場合は移動不可
        {
            mass.isMovable = false;
        }
        return mass;
    }
}