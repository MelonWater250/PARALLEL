using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planet;
using Player;
using Item;
using Sound;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    //ゲームの状態Enum
    public enum GameStateEnum
    {
        BeforeStart,   //開始前
        Gaming,    //ゲーム中
        GameOver    //ゲーム終了
    }
    //ゲームの状態
    private GameStateEnum _gameState = GameStateEnum.BeforeStart;
    public GameStateEnum GameState() { return _gameState; }

    //ゲーム中か
    public bool IsGame() { return GameState() == GameStateEnum.Gaming; }
    

    [Header("ステータス")]
    //[SerializeField, Tooltip("ゲーム時間(秒)")]
    //private float _gameTime = 90.0f;
    //public float GameTime() { return _gameTime; }

    
    [Header("コンポーネント")]
    [SerializeField, Tooltip("惑星(1P)")]
    private PlanetManager _planetManager_1 = null;

    [SerializeField, Tooltip("惑星(2P)")]
    private PlanetManager _planetManager_2 = null;

    [Header("プレイヤーの体力関連")]
    [Tooltip("惑星の初期体力")]
    public int MaxPlanetHealth = 100;


    private void Start()
    {
        _planetManager_1.PlayerNum = 1;
        _planetManager_2.PlayerNum = 2;

        //UI初期化
        UIManager.Instance.InitUI();
        //UI更新
        //UIManager.Instance.UpdateGameTimeUI(_gameTime);
        //初回アイテム生成&プーリング
        ItemManager.Instance.InitItem();
    }

    private IEnumerator UpdateGameCol()
    {
        //どちらかが死んでいたら
        bool isGame = _planetManager_1.IsAlive() && _planetManager_2.IsAlive();
        while (isGame == true&& _gameState == GameStateEnum.Gaming)
        {
            //どちらかが死んでいたら
            isGame = _planetManager_1.IsAlive() && _planetManager_2.IsAlive();
            yield return null;
        }
        //ゲーム終了
        GameOver();
    }

    ///// <summary>
    ///// ゲームのループ
    ///// </summary>
    //private IEnumerator UpdateGameCol()
    //{
    //    //残り時間が0になるまでループ
    //    float time = _gameTime;
    //    while (time > 0)
    //    {
    //        time -= Time.deltaTime;

    //        //UI更新
    //        UIManager.Instance.UpdateGameTimeUI(time);
    //        yield return null;
    //    }

    //    //ゲーム終了
    //    GameOver();
    //}

    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void GameStart()
    {
        _gameState = GameStateEnum.Gaming;

        _planetManager_1.GetComponentInChildren<PlayerBase>().CanMove = true;
        _planetManager_2.GetComponentInChildren<PlayerBase>().CanMove = true;
        
        //ゲーム開始
        StartCoroutine(UpdateGameCol());
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    private void GameOver()
    {
        _gameState = GameStateEnum.GameOver;

        //すべてのアイテムを爆破
        ItemManager.Instance.ExplosionAllItem();

        //UI表示
        UIManager.Instance.DisplayGameOverUI(WinPlayerNum());

        UIAnimationScript.Instance.StartGameOverAnimation();

        //終了サウンド再生用
        SoundManager.Instance.StartGameOverSE();
    }


    /// <summary>
    /// 勝ったほうのプレイヤー番号(引き分けなら0)
    /// </summary>
    private int WinPlayerNum()
    {
        //if (_planetManager_1.Damage() > _planetManager_2.Damage())
        //{
        //    return 1;
        //}
        //else if (_planetManager_1.Damage() < _planetManager_2.Damage())
        //{
        //    return 2;
        //}
        //else
        //{
        //    return 0;
        //}
        if (_planetManager_1.IsAlive() && _planetManager_2.IsAlive())
        {
            return 0;
        }
        else if (_planetManager_1.IsAlive())
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }
}