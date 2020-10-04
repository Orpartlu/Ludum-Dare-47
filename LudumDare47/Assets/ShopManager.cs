﻿using amazeIT;
using Enum;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldTile;

public class ShopManager : MonoBehaviour
{
    public GameObject cursor;
    public GameObject worldTilePrefab;

    private SpriteRenderer _spriteRenderer;
    private WorldTileSpecificationType _buildType = WorldTileSpecificationType.Station;
    private GameObject _tempBuilding;
    private bool onDestroyMode;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = cursor.GetComponentInChildren<SpriteRenderer>();
        _spriteRenderer.sprite = null;
        //_spriteRenderer.material.SetFloat("_GrayscaleAmount", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(button: 1))
        {
            GameManager.Instance.buildModeOn = false;
            _spriteRenderer.sprite = null;
            onDestroyMode = false;
            return;
        }

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position: Input.mousePosition);
        Utils.GetXY(worldPosition: worldPosition, x: out int x, y: out int y);
        cursor.transform.position = new Vector2(x, y);

        if (GameManager.Instance.buildModeOn)
        {
            if (GameManager.Instance.GetFieldStatus(x, y,out _).HasFlag(WorldTileStatusType.Buildable))
            {
                _spriteRenderer.color = Color.white;
                if (Input.GetMouseButtonDown(button: 0))
                {
                    GameManager.Instance.BuildSomething(x, y, _buildType);
                }
            }
            else
            {
                //_spriteRenderer.material.SetFloat("_GrayscaleAmount", 0.5f);
                _spriteRenderer.color = Color.red;
            }
        }else if(onDestroyMode && Input.GetMouseButtonDown(0))
        {
            GameManager.Instance.DeletTile(x, y);
        }
    }

    public void SetBuildType(int typeID)
    {
        WorldTileSpecificationType type = (WorldTileSpecificationType)typeID;
        _buildType = type;

        WorldTileClass worldTile = cursor.GetComponent<WorldTileClass>();

        worldTile.Instantiate(worldTileSpecification: type);

        GameManager.Instance.buildModeOn = true;

        _spriteRenderer.material.SetFloat("_GrayscaleAmount", 1);
    }

    public void SetDestroyMode()
    {
        onDestroyMode = true;
        GameManager.Instance.buildModeOn = false;
        if(SpriteManager.Instance.TryGetSpriteByName("bulldozer", out Sprite bulli)){
            _spriteRenderer.sprite = bulli;
        }
        _spriteRenderer.color = Color.white;
        _spriteRenderer.material.SetFloat("_GrayscaleAmount", 0);
    }

}
