using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MassEvent : ScriptableObject�@//MassEvent�N���X�̒�`�A���e��RPGSceneManager�Ŏ��{
{
    public TileBase Tile;

    public virtual void Exec(RPGSceneManager manager) { }
}