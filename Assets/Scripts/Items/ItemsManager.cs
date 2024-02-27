namespace AFSInterview.Items
{
    using TMPro;
	using UnityEngine;
    using UnityEngine.InputSystem;

    public class ItemsManager : MonoBehaviour
	{
		[SerializeField] private InventoryController inventoryController;
		[SerializeField] private int itemSellMaxValue;
		[SerializeField] private Transform itemSpawnParent;
		[SerializeField] private GameObject itemPrefab;
		[SerializeField] private BoxCollider itemSpawnArea;
		[SerializeField] private float itemSpawnInterval;
		[SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private PlayerInput playerInput;

        private float nextItemSpawnTime;
        private InputAction pickUpItem;
        private InputAction sellItems;
        private InputAction useItem;
        private Camera mainCamera;

        private void Start()
        {
            pickUpItem = playerInput.actions["TryPickUpItem"];
            sellItems = playerInput.actions["SellItems"];
            useItem = playerInput.actions["UseItem"];

            pickUpItem.started += TryPickUpItem;
            sellItems.started += SellItems;
            useItem.started += UseLastItem;

            UpdateMoneyUI();
            mainCamera = Camera.main;
        }

        private void OnDisable()
        {
            pickUpItem.started -= TryPickUpItem;
            sellItems.started -= SellItems;
            useItem.started -= UseLastItem;
        }
        private void Update()
		{
			if (Time.time >= nextItemSpawnTime)
				SpawnNewItem();
        }

        private void UpdateMoneyUI() 
        {
            moneyText.text = $"Money: {inventoryController.Money}";
        }

		private void SpawnNewItem()
		{
			nextItemSpawnTime = Time.time + itemSpawnInterval;
			
			var spawnAreaBounds = itemSpawnArea.bounds;
			var position = new Vector3(
				Random.Range(spawnAreaBounds.min.x, spawnAreaBounds.max.x),
				0f,
				Random.Range(spawnAreaBounds.min.z, spawnAreaBounds.max.z)
			);
			
			Instantiate(itemPrefab, position, Quaternion.identity, itemSpawnParent);
		}

        private void SellItems(InputAction.CallbackContext context) 
        {
            inventoryController.SellAllItemsUpToValue(itemSellMaxValue);
            UpdateMoneyUI();
        }

        private void TryPickUpItem(InputAction.CallbackContext context)
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            var layerMask = LayerMask.GetMask("Item");
            if (Physics.Raycast(ray, out var hit, 100f, layerMask) && hit.collider.TryGetComponent<IItemHolder>(out var itemHolder)) 
            {
                var item = itemHolder.GetItem(true);
                inventoryController.AddItem(item);
                Debug.Log($"Picked up {item.Name} with value of {item.Value} and now have {inventoryController.ItemsCount} items");
            }
        }

        private void UseLastItem(InputAction.CallbackContext context)
        {
            inventoryController.GetLastItem().Use();
            UpdateMoneyUI();
        }
    }
}