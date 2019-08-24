using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item;

namespace Hole
{
    public class BlackHoleManager : MonoBehaviour
    {
        [SerializeField]
        private WarpManager _warpManager = null;
        
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == TagContainer.ITEM_TAG)
            {
                ItemManager item = other.GetComponent<ItemManager>();
                if (item.IsMove() == false) return;
                item.StopMove();
                _warpManager.AddItem(item);
            }
        }
    }
}