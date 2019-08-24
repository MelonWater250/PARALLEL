using Planet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    public class ItemManager : MonoBehaviour
    {
        [HideInInspector]
        //攻撃者のプレイヤー番号(スポーン時に書き換え)
        private int _attackPlayerNum = 1;
        public void SetPlayerNum(int playerNum) { _attackPlayerNum = playerNum; }
        public bool IsMine(int playerNum) { return playerNum == _attackPlayerNum ? true : false; }

        [SerializeField, Tooltip("攻撃力")]
        private float _power = 10.0f;

        [SerializeField, Tooltip("移動速度")]
        private float _speed = 10.0f;

        [SerializeField]
        private bool _isMove = false;
        public bool IsMove() { return _isMove; }
        public void StopMove() { _isMove = false; }

        /// <summary>
        /// 移動
        /// </summary>
        /// <param name="direction">方向</param>
        /// <param name="state">変更後のState</param>
        public void MoveItem(GameObject targetObj)
        {
            if (IsMove()) return;
            StopMove();
            _isMove = true;
            StartCoroutine(MoveItemCol(targetObj));
        }

        private IEnumerator MoveItemCol(GameObject targetObj)
        {
            while(IsMove())
            {
                transform.position += (targetObj.transform.position - transform.position).normalized * _speed * Time.deltaTime;
                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            switch(other.tag)
            {
                case TagContainer.PLAYER_TAG:
                    StopMove();
                    break;

                case TagContainer.PLANET_TAG:
                    PlanetManager planet = other.GetComponent<PlanetManager>();
                    if (planet.PlayerNum != _attackPlayerNum)
                    {
                        StopMove();
                        planet.TakeDamage(_power);
                    }
                    break;
            }
        }
    }
}