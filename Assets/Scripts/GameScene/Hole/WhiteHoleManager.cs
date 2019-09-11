using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item;
using DG.Tweening;

namespace Hole
{
    public class WhiteHoleManager : MonoBehaviour
    {
        //発射中か
        private bool _isFire = false;
        public bool IsFire() { return _isFire; }

        private ParticleSystem _particle;

        [SerializeField, Tooltip("発射の時間間隔")]
        private float _fireIntervalTime = 0.5f;

        //初期のスケール
        private Vector3 _baseScale = new Vector3();

        [SerializeField, Tooltip("拡大縮小にかかる時間")]
        private float _scalingTime = 2.0f;

        //[SerializeField, Tooltip("最初にアイテムを生成する数")]
        //private int _firstFireNum = 5;

        private void Start()
        {
            _particle = GetComponentInChildren<ParticleSystem>();
            _particle.Stop();
            _baseScale = transform.localScale;
        }

        /// <summary>
        /// アイテム発射
        /// </summary>
        /// <param name="item">アイテムの配列</param>
        /// <param name="target">発射先</param>
        /// <param name="attackPlayerNum">攻撃者のプレイヤー番号</param>
        public void Fire(ItemScript item, GameObject target, int attackPlayerNum)
        {
            //発射先を向いて、パーティクル再生、拡大して発射
            _isFire = true;
            transform.LookAt(target.transform.position);
            _particle.Play();

            //攻撃者のプレイヤー番号を登録
            item.SetPlayerNum(attackPlayerNum);

            //拡大
            transform.DOScale(_baseScale, _scalingTime)
                .OnComplete(
               () =>
               {
                   //位置＆向き変更
                   item.transform.position = transform.position;
                   item.transform.up = (transform.position - target.transform.position).normalized;
                   item.gameObject.SetActive(true);
                   item.MoveItem(target);

                   transform.DOScale(Vector3.zero, _scalingTime)
                       .OnComplete(() => _isFire = false);
               });
        }
    }
}