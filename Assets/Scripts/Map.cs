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

    [SerializeField] List<MassEvent> _massEvents; //_massEvents�ϐ��̐錾�A�C���X�y�N�^�[�ł̕\��

    HashSet<CharacterBase> _characters = new HashSet<CharacterBase>();�@//�H�L�����N�^�ʒu�L���p�ɒǉ��H
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
    

    public MassEvent FindMassEvent(TileBase tile)�@//���X�g(_massEvent)�������(tile)�ƈ�v����I�u�W�F�N�g(MassEvent)��Ԃ�
    {
        return _massEvents.Find(_c => _c.Tile == tile);
    }

    public bool FindMassEventPos(TileBase tile, out Vector3Int pos)  //�C�x���g�p�^�C��(tile)��eventLayer���łǂ̈ʒu�ɂ��邩����
    {
        var eventLayer = _tilemaps[EVENT_BOX_TILEMAP_NAME];
        var renderer = eventLayer.GetComponent<Renderer>();
        var min = eventLayer.LocalToCell(renderer.bounds.min);�@//�^�C���}�b�v�̍����̊p�ƉE��̊p�̈ʒu���Z�����W�ɕϊ�
        var max = eventLayer.LocalToCell(renderer.bounds.max);
        pos = Vector3Int.zero;
        for (pos.y = min.y; pos.y < max.y; ++pos.y)�@//�͈͓��̃Z���𑖍�
        {
            for (pos.x = min.x; pos.x < max.x; ++pos.x)
            {
                var t = eventLayer.GetTile(pos);�@//�Z���̈ʒu�^�C�����擾
                if (t == tile) return true;�@//�ړI�̃^�C�������������ꍇture��Ԃ�
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

        _tilemaps[EVENT_BOX_TILEMAP_NAME].gameObject.SetActive(false);�@//EventBox���\���ɂ���

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

    public Mass GetMassData(Vector3Int pos)�@//Mass�I�u�W�F�N�g�̃f�[�^�擾�ƁA�e�^�C���}�b�v�ړ��\���ǂ����̐ݒ�
    {
        var mass = new Mass();
        mass.eventTile = _tilemaps[EVENT_BOX_TILEMAP_NAME].GetTile(pos);
        mass.isMovable = true;�@//�f�t�H���g�ł͈ړ��\�Ɛݒ�
        mass.character = GetCharacter(pos);�@//�e�L�����N�^�̈ʒu�L�^

        if (mass.character != null)�@//�L�����N�^�[������ꍇ�͈ړ��ł��Ȃ�
        {
            mass.isMovable = false;
        }
        else if (mass.eventTile != null)�@//eventTile�ɉ�������ꍇ�̓C�x���g(eventTile)��ݒ�
        {
            mass.massEvent = FindMassEvent(mass.eventTile);
        }
        else if (_tilemaps[OBJECTS_TILEMAP_NAME].GetTile(pos))�@//�I�u�W�F�N�g�^�C���}�b�v�ɉ�������ꍇ�ꍇ�͈ړ��s��
        {
            mass.isMovable = false;
        }
        else if (_tilemaps[BACKGROND_TILEMAP_NAME].GetTile(pos) == null)�@//�w�i�^�C���}�b�v�ɉ����Ȃ��ꍇ�͈ړ��s��
        {
            mass.isMovable = false;
        }
        return mass;
    }
}