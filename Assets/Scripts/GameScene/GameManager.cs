using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planet;
using Player;
using Item;

public class GameManager : SingletonMonoBehaviour<GameManager>
{ 
    [SerializeField, Tooltip("惑星(1P)")]
    private PlanetManager _planetManager_1 = null;

    [SerializeField, Tooltip("惑星(2P)")]
    private PlanetManager _planetManager_2 = null;
    
    //ゲーム中か
    public bool IsGame() { return _planetManager_1.IsAlive() && _planetManager_2.IsAlive(); }

    private void Start()
    {
        GameStart();
    }

    private void GameStart()
    {
        _planetManager_1.GetComponentInChildren<PlayerBase>().CanMove = true;
        _planetManager_2.GetComponentInChildren<PlayerBase>().CanMove = true;
        StartCoroutine(UpdateGameCol());
    }

    private IEnumerator UpdateGameCol()
    {
        while(IsGame())
        {
            yield return null;
        }
        //すべてのアイテムを爆破
        ItemManager.Instance.ExplosionAllItem();
    }
}
