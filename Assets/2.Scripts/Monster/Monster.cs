using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> sprites;


    public void InitializeMonster(MonsterDetailsSO monsterData, LayerMask lane)
    {
        sprites.Zip(monsterData.sprites, (sprite, dataSprite) => sprite.sprite = dataSprite).ToList();

        this.gameObject.layer = lane;
        Debug.Log(LayerMask.LayerToName(lane));
    }
}
