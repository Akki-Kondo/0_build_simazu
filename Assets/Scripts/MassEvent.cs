using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MassEvent : ScriptableObject　//MassEventクラスの定義、内容はRPGSceneManagerで実施
{
    public TileBase Tile;

    public virtual void Exec(RPGSceneManager manager) { }
}