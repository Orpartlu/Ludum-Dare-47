﻿using amazeIT;
using Enum;
using Manager;
using UnityEngine;
using WorldTile;

public class ShopManager : MonoBehaviour
{
    public GameObject cursor;

    private SpriteRenderer _spriteRenderer;
    private WorldTileSpecificationType _buildType = WorldTileSpecificationType.Station;
    private int _buildPrice;
    private int _level;
    private GameObject _tempBuilding;
    private bool _onDestroyMode;
    private static readonly int GrayscaleAmount = Shader.PropertyToID("_GrayscaleAmount");

    private void Start()
    {
        _spriteRenderer = cursor.GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.sprite = null;

        ShopItem.OnShopItemPressed += SetBuildType;
    }

    private void SetBuildType(ShopItem item)
    {
        if (item.type == WorldTileSpecificationType.None)
        {
            SetDestroyMode();
        }
        else
        {
            SoundManager.Instance.PlaySoundUiClick();
            SetBuildType(type: item.type);
            _buildPrice = item.price;
            _level = item.level;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(button: 1))
        {
            GameManager.Instance.buildModeOn = false;
            _spriteRenderer.sprite = null;
            _onDestroyMode = false;
            return;
        }

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position: Input.mousePosition);
        Utils.GetXY(worldPosition: worldPosition, x: out int x, y: out int y);
        cursor.transform.position = new Vector2(x: x, y: y);

        if (GameManager.Instance.buildModeOn)
        {
            if (GameManager.Instance.GetFieldStatus(x: x, y: y, worldTile: out _).HasFlag(flag: WorldTileStatusType.Buildable))
            {
                _spriteRenderer.color = Color.white;
                if (Input.GetMouseButtonDown(button: 0) && CanBuy(price: _buildPrice))
                {
                    GameManager.Instance.ChangeMoney(sumToAdd: -_buildPrice,new Vector3(x,y,0));
                    GameManager.Instance.BuildSomething(x: x, y: y, buildType: _buildType,_level);
                }
            }
            else
            {
                _spriteRenderer.color = Color.red;
            }
        }
        else if (_onDestroyMode && Input.GetMouseButtonDown(button: 0))
        {
            GameManager.Instance.DeleteTile(x: x, y: y);
        }
    }

    private void SetBuildType(WorldTileSpecificationType type)
    {
        _buildType = type;

        WorldTileClass worldTile = cursor.GetComponent<WorldTileClass>();

        worldTile.InstantiateForShop(worldTileSpecification: type, _level);

        GameManager.Instance.buildModeOn = true;

        _spriteRenderer.material.SetFloat( nameID: GrayscaleAmount,  value: 1f);
    }

    public void SetDestroyMode()
    {
        _onDestroyMode = true;
        GameManager.Instance.buildModeOn = false;
        if (SpriteManager.Instance.TryGetSpriteByName(spriteName: "bulldozer", outSprite: out Sprite bulli))
        {
            _spriteRenderer.sprite = bulli;
        }

        _spriteRenderer.color = Color.white;
        _spriteRenderer.material.SetFloat(nameID: GrayscaleAmount, value: 0);
    }

    private bool CanBuy(int price)
    {
        return GameManager.Instance.Money >= price;
    }
}