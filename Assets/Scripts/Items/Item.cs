namespace AFSInterview.Items
{
	using System;
    using System.Collections.Generic;
    using UnityEngine;
    using static UnityEditor.Progress;

    [Serializable]
    public class Item
    {
		[SerializeField] private string name;
		[SerializeField] private int value;
        [SerializeField] private bool isConsumable;
        private int randomAction = 0;
        private static readonly Dictionary<string, int> itemsValuePairs = new Dictionary<string, int>
        {
            { "Apple", 10 },
            { "Orange", 20 },
            { "Carrot", 30 },
            { "Broccoli", 40 }
        };

        public string Name => name;
		public int Value => value;
        public bool IsConsumable => isConsumable;
        public Item(string name, int value, bool isConsumable = false)
		{
			this.name = name;
			this.value = value;
            this.isConsumable = isConsumable;
        }

		public void Use()
		{
			Debug.Log("Using" + Name);

            if (IsConsumable)
            {
                InventoryController inventoryController = InventoryController.Instance;
                if(randomAction == 0) 
                {
                    randomAction = UnityEngine.Random.Range(1, 3);
                }

                if (randomAction == 1)
                {
                    inventoryController.AddMoney(value);
                    Debug.Log($"Added {value} money. Current money: {inventoryController.Money}");
                }
                else
                {
                    Item newItem = DrawNewItem();
                    if (newItem != null)
                    {
                        inventoryController.AddItem(newItem);
                        Debug.Log($"New item added: {newItem.Name} with value of {newItem.Value}. Now you have {inventoryController.ItemsCount} items.");
                    }
                }
            }
        }
        private Item DrawNewItem() 
        {
            int randomIndex = UnityEngine.Random.Range(0, 4);
            List<string> keys = new(itemsValuePairs.Keys);
            string randomItemName = keys[randomIndex];
            int randomItemValue = itemsValuePairs[randomItemName];
            Item newItem = new(randomItemName, randomItemValue, UnityEngine.Random.value > 0.5f);
            return newItem;
        }
    }
}