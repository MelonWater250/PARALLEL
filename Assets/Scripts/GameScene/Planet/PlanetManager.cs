using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Planet
{
    /// <summary>
    /// 惑星上のプレイヤーの管理、自転、相手の攻撃(アイテムに当たったときの判定)
    /// </summary>
    public class PlanetManager : MonoBehaviour
    {
        public int PlayerNum = 1;

        [Header("パラメーター")]
        //現在受けているダメージ量
        private int _damage = 0;
        public int Damage() { return _damage; }

        [SerializeField, Tooltip("自転速度")]
        private float _rotateSpeed = 10.0f;

        [Header("UI")]
        [SerializeField, Tooltip("受けているダメージ表示用テキスト")]
        private Text _damageText = null;

        [SerializeField, Tooltip("ダメージをカウントアップするのにかかる時間")]
        private float _uiUpdateTime = 2.0f;

        void Update()
        {
            RotatePlanet();
        }

        /// <summary>
        /// 自転
        /// </summary>
        private void RotatePlanet()
        {
            transform.Rotate(0, _rotateSpeed * Time.deltaTime, 0);
        }

        /// <summary>
        /// 攻撃を受けたときのダメージ計算
        /// </summary>
        /// <param name="damage">攻撃者の攻撃力</param>
        public void TakeDamage(int damage = 0)
        {
            //体力が0以下ならリターン
            if (GameManager.Instance.IsGame() == false) { return; }

            _damage += damage;
            UpdateUI();
            //カメラを揺らす
        }

        /// <summary>
        /// UI表示更新
        /// </summary>
        private void UpdateUI()
        {
            //int damage = _damage;
            //DOTween.To(
            //    () => damage, (int a) => damage = a, _damage, _uiUpdateTime)
            //    .OnUpdate(() => _damageText.text = damage.ToString());

            _damageText.text = _damage.ToString();
        }
    }
}