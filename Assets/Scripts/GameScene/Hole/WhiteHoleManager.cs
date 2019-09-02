using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item;
using DG.Tweening;

namespace Hole
{
    public class WhiteHoleManager : SingletonMonoBehaviour<WhiteHoleManager>
    {
        private bool _isFire = false;
        public bool IsFire() { return _isFire; }

        private ParticleSystem _particle;

        [SerializeField, Tooltip("発射の時間間隔")]
        private float _fireIntervalTime = 0.5f;

        private Vector3 _baseScale = new Vector3();

        [SerializeField, Tooltip("拡大縮小にかかる時間")]
        private float _scalingTime = 2.0f;

        [SerializeField, Tooltip("最初にアイテムを生成する数")]
        private int _firstFireNum = 5;

        private void Start()
        {
            _particle = GetComponentInChildren<ParticleSystem>();
            _particle.Stop();
            _baseScale = transform.localScale;

        }

        /// <summary>
        /// アイテム発射
        /// </summary>
        /// <param name="items">アイテムの配列</param>
        /// <param name="target">発射先</param>
        public void Fire(BombScript item, GameObject target)
        {
            //発射先を向いて、パーティクル再生、拡大して発射
            _isFire = true;
            transform.LookAt(target.transform.position);
            _particle.Play();
            transform.DOScale(_baseScale, _scalingTime)
                .OnComplete(() => StartCoroutine(FireCol(item, target)));
        }

        private IEnumerator FireCol(BombScript item, GameObject target)
        {
            item.transform.position = transform.position;
            item.transform.up = (transform.position - target.transform.position).normalized;
            item.gameObject.SetActive(true);
            item.MoveItem(target);
            yield return new WaitForSeconds(_fireIntervalTime);
            
            transform.DOScale(Vector3.zero, _scalingTime)
                .OnComplete(() => _isFire = false);
            yield return null;
        }
    }
}