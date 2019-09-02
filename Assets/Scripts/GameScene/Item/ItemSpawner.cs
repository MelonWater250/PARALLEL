using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planet;
using UnityEngine.UI;

namespace Item
{
    //一定時間間隔でアイテム生成
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField, Tooltip("ベースの生成のクールタイム")]
        private float _baseCoolTime = 3;

        private PlanetManager _planetManager;
        
        //アイテムを持っているか
        private bool HaveItem() { return GetComponentInChildren<BombScript>(); }

        //生成できるか
        private bool _canSpawn = false;

        [Header("UI")]
        [SerializeField, Tooltip("クールタイム表示用UI")]
        Image _coolTimeImage = null;

        private void Start()
        {
            _planetManager = GetComponentInParent<PlanetManager>();
            transform.LookAt(_planetManager.gameObject.transform.position);
            _canSpawn = true;
        }

        private void Update()
        {
            if(HaveItem() == false && _canSpawn && GameManager.Instance.IsGame())
            {
                _canSpawn = false;
                StartCoroutine(SpawnItemCol());
            }
        }

        /// <summary>
        /// アイテム生成+クールタイム
        /// </summary>
        private IEnumerator SpawnItemCol()
        {
            //時間計測用
            float time = 0;
            float coolTime = _baseCoolTime;
            
            //クールタイム
            while (time <= coolTime)
            {
                time += Time.deltaTime;

                //UI更新
                _coolTimeImage.fillAmount = time / coolTime;
                yield return null;
            }

            //アイテム生成(再表示)
            SpawnItem();
            yield return null;
            _canSpawn = true;
        }

        //アイテム生成(再表示)
        private void SpawnItem()
        {
            BombScript item = ItemManager.Instance.IdleItem();
            if (item == null) return;
            item.transform.SetParent(transform);
            item.StopMove();
            item.transform.SetPositionAndRotation(transform.position, transform.rotation);
            item.IsActive = true;
            item.gameObject.SetActive(true);
        }
    }
}