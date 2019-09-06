using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planet;
using Player;
using Item;
using UnityEngine.UI;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    //ゲーム終了時のメッセージ(勝者)
    private static readonly string MESSAGE_WINNER = "Player{0} WIN!";

    //ゲーム終了時のメッセージ(引き分け)
    private static readonly string MESSAGE_DRAW = "Draw!";

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
    [SerializeField, Tooltip("ゲーム時間(秒)")]
    private float _gameTime = 90.0f;


    [Header("UI")]
    [SerializeField, Tooltip("勝者名表示用Text")]
    private Text _resultText = null;

    [SerializeField, Tooltip("ゲーム終了時に表示するタイトル遷移UI")]
    private GameObject _toTitleImage = null;

    [SerializeField, Tooltip("制限時間常時表示用テキスト")]
    private Text _gameTimeText = null;

    [SerializeField, Tooltip("制限時間常時表示用スライダー")]
    private Slider _gameTimeSlider = null;

    [SerializeField, Tooltip("制限時間が一定以下になったときに表示するテキスト")]
    private Text[] _gameTimeBigTexts = null;

    [SerializeField, Tooltip("残り何秒からBigTextをプレイヤーに表示するか")]
    private float _beginDisplayBigTextMaxTime = 10;

    [Header("コンポーネント")]
    [SerializeField, Tooltip("惑星(1P)")]
    private PlanetManager _planetManager_1 = null;

    [SerializeField, Tooltip("惑星(2P)")]
    private PlanetManager _planetManager_2 = null;

    private void Start()
    {
        _toTitleImage.SetActive(false);
        _resultText.gameObject.SetActive(false);
        _gameTimeSlider.minValue = 0;
        _gameTimeSlider.maxValue = _gameTime;
        _gameTimeSlider.value = _gameTimeSlider.maxValue;
        for (int i = 0; i < _gameTimeBigTexts.Length; i++)
        {
            _gameTimeBigTexts[i].gameObject.SetActive(false);
        }


        GameStart();
    }

    /// <summary>
    /// ゲームのループ
    /// </summary>
    private IEnumerator UpdateGameCol()
    {
        //残り時間が0になるまでループ
        float time = _gameTime;
        while (time > 0)
        {
            time -= Time.deltaTime;

            //制限時間のテキスト移動、表示更新
            _gameTimeSlider.value = time;
            _gameTimeText.text = ((int)time).ToString();

            //一定時間以下ならそれぞれのプレイヤー画面に残り時間を表示する
            if (time < _beginDisplayBigTextMaxTime)
            {
                //プレイヤー用タイムテキスト表示
                for (int i = 0; i < _gameTimeBigTexts.Length; i++)
                {
                    if(_gameTimeBigTexts[i].gameObject.activeSelf == false)
                    {
                        _gameTimeBigTexts[i].gameObject.SetActive(true);
                    }
                }

                DisplayBigText(time);
            }

            yield return null;
        }

        //ゲーム終了
        GameOver();
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    private void GameStart()
    {
        _gameState = GameStateEnum.Gaming;

        _planetManager_1.GetComponentInChildren<PlayerBase>().CanMove = true;
        _planetManager_2.GetComponentInChildren<PlayerBase>().CanMove = true;

        //初回アイテム生成&プーリング
        ItemManager.Instance.InstantiateItem();
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

        //ボタン表示
        _toTitleImage.gameObject.SetActive(true);

        //終了サウンド再生用
        //.PlayOneShot(AudioContainer.Instance.EndFightSound);


        //勝負結果を表示
        _resultText.text = ResultText();
        _resultText.gameObject.SetActive(true);

        _toTitleImage.SetActive(true);
        _resultText.gameObject.SetActive(true);
    }

    /// <summary>
    /// それぞれのプレイヤー画面に残り時間を表示する
    /// </summary>
    private void DisplayBigText(float time)
    {
        for (int i = 0; i < _gameTimeBigTexts.Length; i++)
        {
            _gameTimeBigTexts[i].text = ((int)time).ToString();

            //小数でアルファ値を変化
            Color newColor = _gameTimeBigTexts[i].color;
            //1で割ったあまり＝小数点以下の値のみ
            float fraction = time % 1;
            newColor.a = Mathf.Lerp(0, 1, fraction);
            _gameTimeBigTexts[i].color = newColor;
        }
    }

    /// <summary>
    /// 勝ったほうのプレイヤー(引き分けならドロー)
    /// </summary>
    private string ResultText()
    {
        if (_planetManager_1.Damage() > _planetManager_2.Damage())
        {
            return string.Format(MESSAGE_WINNER, 1);
        }
        else if (_planetManager_1.Damage() < _planetManager_2.Damage())
        {
            return string.Format(MESSAGE_WINNER, 2);
        }
        else
        {
            return MESSAGE_DRAW;
        }
    }
}