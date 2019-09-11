using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アニメーションイベント受け取り用
/// </summary>
public class UIAnimationScript : SingletonMonoBehaviour<UIAnimationScript>
{
    private static readonly string GAMEOVER_BOOL = "GameOver";

    Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.SetBool(GAMEOVER_BOOL, false);
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void OnGameStartAnimationEvent()
    {
        GameManager.Instance.GameStart();
    }

    /// <summary>
    /// ゲームオーバー時にアニメーション再生
    /// (GameManagerから呼び出し)
    /// </summary>
    public void StartGameOverAnimation()
    {
        _animator.SetBool(GAMEOVER_BOOL,true);
    }
}
