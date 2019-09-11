using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// キャンバスの描画制御
/// </summary>
public class UIManager : SingletonMonoBehaviour<UIManager>
{
    //ゲーム終了時のメッセージ(勝者)
    private static readonly string MESSAGE_WINNER = "Player{0} WIN!";

    //ゲーム終了時のメッセージ(引き分け)
    private static readonly string MESSAGE_DRAW = "Draw!";

    [Header("ゲーム中")]
    [SerializeField, Tooltip("与えたダメージ量表示用テキスト(1P用)")]
    private Text _damageText_Player1 = null;
    [SerializeField, Tooltip("与えたダメージ量表示用テキスト(2P用)")]
    private Text _damageText_Player2 = null;

    [SerializeField, Tooltip("制限時間常時表示用テキスト")]
    private Text _gameTimeText = null;

    [SerializeField, Tooltip("制限時間常時表示用スライダー")]
    private Slider _gameTimeSlider = null;

    [SerializeField, Tooltip("制限時間が一定以下になったときに表示するテキスト")]
    private Text _gameTimeBigText_1 = null;
    [SerializeField, Tooltip("制限時間が一定以下になったときに表示するテキスト")]
    private Text _gameTimeBigText_2 = null;

    [SerializeField, Tooltip("残り何秒からBigTextをプレイヤーに表示するか")]
    private float _beginDisplayBigTextMaxTime = 10;


    [Header("ゲーム終了時")]
    [SerializeField, Tooltip("勝者名表示用Text")]
    private Text _resultText = null;

    [SerializeField, Tooltip("ゲーム終了時に表示するタイトル遷移UI")]
    private GameObject _toTitleImage = null;


    /// <summary>
    /// UI初期化
    /// </summary>
    public void InitUI()
    {
        //_gameTimeSlider.minValue = 0;
        //_gameTimeSlider.maxValue = GameManager.Instance.GameTime();
        //_gameTimeSlider.value = _gameTimeSlider.maxValue;

        _gameTimeBigText_1.gameObject.SetActive(false);
        _gameTimeBigText_2.gameObject.SetActive(false);
        _toTitleImage.SetActive(false);
        _resultText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 受けているダメージUI更新
    /// </summary>
    /// <param name="playerNum">プレイヤー番号</param>
    /// <param name="point">与えたダメージ</param>
    public void UpdateDamageText(int playerNum,int point)
    {
        if(playerNum == 1)
        {
            _damageText_Player1.text = point.ToString();
        }
        else if(playerNum == 2)
        {
            _damageText_Player2.text = point.ToString();
        }
    }

    /// <summary>
    /// 残り時間UI更新
    /// </summary>
    /// <param name="time">残り時間</param>
    public void UpdateGameTimeUI(float time)
    {
        //制限時間のテキスト移動、表示更新
        _gameTimeSlider.value = time;
        _gameTimeText.text = ((int)time).ToString();

        //一定時間以下ならそれぞれのプレイヤー画面に残り時間を表示する
        if (time < _beginDisplayBigTextMaxTime)
        {
            //プレイヤー用タイムテキスト表示
            _gameTimeBigText_1.gameObject.SetActive(true);
            _gameTimeBigText_2.gameObject.SetActive(true);
        }

        DisplayBigText(time);
    }
    
    /// <summary>
    /// それぞれのプレイヤー画面に残り時間を表示する
    /// </summary>
    private void DisplayBigText(float time)
    {
        _gameTimeBigText_1.text = ((int)time).ToString();
        _gameTimeBigText_2.text = ((int)time).ToString();

        //小数でアルファ値を変化
        Color newColor = _gameTimeBigText_1.color;
        //1で割ったあまり＝小数点以下の値のみ
        float fraction = time % 1;
        newColor.a = Mathf.Lerp(0, 1, fraction);
        //不透明度のみ変更した色を代入
        _gameTimeBigText_1.color = newColor;
        _gameTimeBigText_2.color = newColor;
    }
    
    /// <summary>
    /// ゲーム終了時のUI一括表示
    /// </summary>
    /// <param name="winPlayerNum">勝ったほうのプレイヤー番号</param>
    public void DisplayGameOverUI(int winPlayerNum)
    {
        //タイトル遷移画像表示
        _toTitleImage.gameObject.SetActive(true);

        //勝負結果を表示
        _resultText.text = ResultText(winPlayerNum);
        _resultText.gameObject.SetActive(true);

        _toTitleImage.SetActive(true);
        _resultText.gameObject.SetActive(true);
    }

    /// <summary>
    /// 勝敗によって表示する文章変更
    /// </summary>
    /// <param name="winPlayerNum">勝ったほうのプレイヤー番号</param>
    private string ResultText(int winPlayerNum)
    {
        string text = "";
        if (winPlayerNum <= 0) text = MESSAGE_DRAW;
        else
        {
            text = string.Format(MESSAGE_WINNER, winPlayerNum);
        }
        return text;
    }
}