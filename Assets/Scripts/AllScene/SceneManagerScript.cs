using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sound;

/// <summary>
/// シーン管理、遷移処理
/// </summary>
public class SceneManagerScript : SingletonMonoBehaviour<SceneManagerScript>
{
    //string test = "TestScene";
    //決定キーのキーコード
    const string SUBMIT_KEY = "Submit";

    //タイトルシーン
    private const string TITLESCENE = "TitleScene";
    //ゲームシーン
    private const string GAMESCENE = "GameScene";


    private string NowScene = TITLESCENE;

    //フェードイン・アウト用コンポーネント
    private Fade _fade = null;

    [SerializeField, Tooltip("フェードイン・アウト時間")]
    private float _fadeTime = 1.0f;

    private static SceneManagerScript _sceneManagerScript = null;

    void Awake()
    {
        if (null != _sceneManagerScript)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _sceneManagerScript = this;
            DontDestroyOnLoad(gameObject);
        }

        //https://tech.pjin.jp/blog/2018/10/24/unity_scene-manager_event/#10
        // イベントにイベントハンドラーを追加
        SceneManager.sceneLoaded += (nextScene, mode) => SceneLoaded(nextScene, mode);

        _fade = GetComponentInChildren<Fade>();

        switch (NowScene)
        {
            case TITLESCENE:
                SoundManager.Instance.StartBGMSound(SoundManager.PlayingBGMClipEnum.TitleBGM);
                break;
            case GAMESCENE:
                SoundManager.Instance.StartBGMSound(SoundManager.PlayingBGMClipEnum.GameBGM);
                break;
            default:
                SoundManager.Instance.StartBGMSound(SoundManager.PlayingBGMClipEnum.TitleBGM);
                break;
        }
    }

    //シーンが遷移したとき呼ばれる
    // イベントハンドラー（イベント発生時に動かしたい処理）
    void SceneLoaded(Scene nextScene, LoadSceneMode mode)
    {
        //フェードアウト
        _fade.FadeOut(_fadeTime);
        switch (NowScene)
        {
            case TITLESCENE:
                SoundManager.Instance.StartBGMSound(SoundManager.PlayingBGMClipEnum.TitleBGM);
                break;
            case GAMESCENE:
                SoundManager.Instance.StartBGMSound(SoundManager.PlayingBGMClipEnum.GameBGM);
                break;
            default:
                SoundManager.Instance.StartBGMSound(SoundManager.PlayingBGMClipEnum.TitleBGM);
                break;
        }
    }
    //public void LoadTestScene()
    //{
    //    //_fadeTime秒かけてフェードイン、その後シーン遷移
    //    _fade.FadeIn(_fadeTime,
    //    () =>
    //    {
    //        SceneManager.LoadScene(test);
    //        NowScene = TITLESCENE;
    //    });
    //}

    /// <summary>
    /// タイトルシーン遷移
    /// </summary>
    public void LoadTitleScene()
    {
        //_fadeTime秒かけてフェードイン、その後シーン遷移
        _fade.FadeIn(_fadeTime,
        () =>
        {
            SceneManager.LoadScene(TITLESCENE);
            NowScene = TITLESCENE;
        });
    }


    /// <summary>
    /// ゲームシーン遷移
    /// </summary>
    public void LoadGameScene()
    {

        //_fadeTime秒かけてフェードイン、その後シーン遷移
        _fade.FadeIn(_fadeTime,
        () =>
        {
            SceneManager.LoadScene(GAMESCENE);
            NowScene = GAMESCENE;
        });
    }
    
    void Update()
    {
        //F1キーでタイトルシーンへ
        if (Input.GetKeyDown(KeyCode.F1))
        {
            //_submitSESource.Play();
            LoadTitleScene();
            //LoadTestScene();
        }
        //F2キーでゲームシーンへ
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            //_submitSESource.Play();
            LoadGameScene();
        }

        //タイトルシーン中に
        //スペースキー、Xboxの決定キーのどれかが押されたらゲームシーンへ遷移
        if (NowScene == TITLESCENE)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown(SUBMIT_KEY))
            {
                SoundManager.Instance.StartSceneChangeSE();
                LoadGameScene();
            }
        }
        //ゲームシーン中で、勝敗が決まっている時に
        //スペースキー、Xboxの決定キーのどれかが押されたらタイトルシーンへ遷移
        else if (NowScene == GAMESCENE && GameManager.Instance.GameState() == GameManager.GameStateEnum.GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown(SUBMIT_KEY))
            {
                SoundManager.Instance.StartSceneChangeSE();
                LoadTitleScene();
            }
        }
    }
}