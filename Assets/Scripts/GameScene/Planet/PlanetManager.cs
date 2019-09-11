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

        //生きているか
        [HideInInspector]
        bool _isAlive = true;
        public bool IsAlive() { return _nowHealth > 0; }

        [Header("パラメーター")]
        //現在受けているダメージ量
        private int _damage = 0;
        public int Damage() { return _damage; }

        [SerializeField, Tooltip("自転速度")]
        private float _rotateSpeed = 10.0f;

        //惑星の初期体力
        private float _maxHealth;
        //惑星の体力
        private float _nowHealth;

        [Header("UI")]
        [SerializeField, Tooltip("体力表示用UI")]
        Image[] _planetHealthUI = new Image[2];

        [SerializeField, Tooltip("体力表示用UI(赤)")]
        Image[] _planetHealthUI_Red = new Image[2];

        [SerializeField, Tooltip("赤いバーの移動時間")]
        float _DamageDurationTime = 2;

        [Header("CameraShake")]
        [SerializeField, Tooltip("プレイヤーのカメラ")]
        private Camera _camera = null;

        [SerializeField, Tooltip("ダメージを受けたときの揺れの時間")]
        private float _cameraShakeTime = 0.5f;

        [SerializeField, Tooltip("ダメージを受けたときの揺れの強さ")]
        private float _cameraShakeStrength = 0.3f;

        private void Start()
        {
            //固定値受け取り
            _maxHealth = GameManager.Instance.MaxPlanetHealth;

            //体力のUI表示更新
            _nowHealth = _maxHealth;
            UpdateHealthUI();
        }

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

            //_damage += damage;
            _nowHealth -= damage;
            Mathf.Clamp(_nowHealth,0,_maxHealth);
            UpdateHealthUI();

            //カメラを揺らす
            _camera.transform.DOShakePosition(_cameraShakeTime, _cameraShakeStrength);

            IsAlive();
        }

        /// <summary>
        /// UI表示更新
        /// </summary>
        private void UpdateHealthUI()
        {
            //int damage = _damage;
            //DOTween.To(
            //    () => damage, (int a) => damage = a, _damage, _uiUpdateTime)
            //    .OnUpdate(() => _damageText.text = damage.ToString());
            
            foreach (Image HPUI in _planetHealthUI)
            {
                HPUI.fillAmount = _nowHealth / _maxHealth;
            }

            foreach (Image HPUI_Red in _planetHealthUI_Red)
            {
                HPUI_Red.DOFillAmount(_nowHealth / _maxHealth, _DamageDurationTime);
            }

            //UIManager.Instance.UpdateDamageText(PlayerNum, Damage());
        }
    }
}