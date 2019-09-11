using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    /// <summary>
    /// BGM、SEの再生処理
    /// </summary>
    public class SoundManager : SingletonMonoBehaviour<SoundManager>
    {
        [Header("BGM")]
        [SerializeField,Tooltip("タイトルシーンのBGM")]
        private AudioClip _BGM_Title;

        [SerializeField, Tooltip("ゲームシーンのBGM")]
        private AudioClip _BGM_Game;

        /// <summary>
        /// 再生のBGM
        /// </summary>
        public enum PlayingBGMClipEnum
        {
            TitleBGM,
            GameBGM
        }


        [Header("システム")]
        [SerializeField, Tooltip("決定サウンド")]
        private AudioClip _clickSound;

        [SerializeField, Tooltip("ゲーム終了サウンド")]
        private AudioClip _gameOverSound;

        [Header("ゲーム")]
        [Tooltip("爆発サウンド(アイテム用)")]
        public AudioClip ExplosionSound;
        

        [Header("AudioSource")]
        [SerializeField,Tooltip("BGM再生用")]
        private AudioSource _BGMSource;

        [SerializeField, Tooltip("SE再生用")]
        private AudioSource _SESource;


        /// <summary>
        /// BGM再生開始
        /// </summary>
        public void StartBGMSound(PlayingBGMClipEnum sceneBGM)
        {
            switch (sceneBGM)
            {
                case PlayingBGMClipEnum.TitleBGM:
                    _BGMSource.clip = _BGM_Title;
                    break;

                case PlayingBGMClipEnum.GameBGM:
                    _BGMSource.clip = _BGM_Game;
                    break;

                default:
                    _BGMSource.clip = _BGM_Title;
                    break;
            }
            //再生
            _BGMSource.Play();
            Debug.Log("Play");
        }

        /// <summary>
        /// シーン遷移時のSE再生
        /// </summary>
        public void StartSceneChangeSE()
        {
            _SESource.clip = _clickSound;
            _SESource.Play();
        }

        /// <summary>
        /// ゲーム終了時のSE再生
        /// </summary>
        public void StartGameOverSE()
        {
            _SESource.clip = _gameOverSound;
            _SESource.Play();
        }
    }
}