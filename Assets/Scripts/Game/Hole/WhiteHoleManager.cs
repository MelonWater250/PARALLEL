using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item;
using DG.Tweening;

namespace Hole
{
    public class WhiteHoleManager : MonoBehaviour
    {
        [SerializeField]
        private bool _isFire = false;
        public bool IsFire() { return _isFire; }

        private ParticleSystem _particle;

        [SerializeField, Tooltip("発射の時間間隔")]
        private float _fireIntervalTime = 0.1f;

        private Vector3 _baseScale = new Vector3();

        [SerializeField, Tooltip("拡大縮小にかかる時間")]
        private float _scalingTime = 2.0f;

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
        public void Fire(ItemManager[] items,GameObject target)
        {
            //発射先を向いて、パーティクル再生、拡大して発射
            _isFire = true;
            transform.LookAt(target.transform.position);
            _particle.Play();
            transform.DOScale(_baseScale, _scalingTime)
                .OnComplete(() => StartCoroutine(FireCol(items,target)));
        }

        private IEnumerator FireCol(ItemManager[] items,GameObject target)
        {

            Debug.Log(gameObject.name + " : Fire!");
            foreach (ItemManager item in items)
            {
                item.gameObject.transform.position = transform.position;
                item.gameObject.SetActive(true);
                item.MoveItem(target);
                yield return new WaitForSeconds(_fireIntervalTime);
            }

            yield return null;
            transform.DOScale(Vector3.zero, _scalingTime)
                .OnComplete(()=>_isFire = false);
        }
    }
}