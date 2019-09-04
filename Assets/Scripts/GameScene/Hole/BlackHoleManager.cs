using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Item;

namespace Hole
{
    public class BlackHoleManager : MonoBehaviour
    {
        [SerializeField]
        private HoleManager _warpManager = null;
        
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == TagContainer.ITEM_TAG)
            {
                ItemScript item = other.GetComponent<ItemScript>();
                if (item.CanMove() == false) return;
                item.StopMove();
                _warpManager.AddItem(item);
            }
        }
    }
}