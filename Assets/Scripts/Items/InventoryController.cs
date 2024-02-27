namespace AFSInterview.Items
{
	using System.Collections.Generic;
	using UnityEngine;

	public class InventoryController : MonoBehaviour
	{
		[SerializeField] private List<Item> items;
		[SerializeField] private int money;

        public static InventoryController Instance;

        public int Money => money;
		public int ItemsCount => items.Count;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if(Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void SellAllItemsUpToValue(int maxValue)
		{
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var itemValue = items[i].Value;
                if (itemValue <= maxValue)
                {
                    money += itemValue;
                    items.RemoveAt(i);
                }
            }
        }

        public void AddMoney(int amount)
        {
            money += amount;
        }

        public void AddItem(Item item)
		{
			items.Add(item);
		}

        public Item GetLastItem()
        {
            if (items.Count > 0)
            {
                return items[^1];
            }
            return null;
        }
    }
}