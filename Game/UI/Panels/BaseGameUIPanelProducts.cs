using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelProducts : GameUIPanelBase {
    
    public static GameUIPanelProducts Instance;
	
    public GameObject listItemItemPrefab;
	
    public string currentProductType;
    
    public string productCodeUse = "";
    public string productTypeUse = "";
    public string productCharacterUse = "";
    
    string buttonBuyUseName = "ButtonActionItemBuyUse";
    
    public static bool isInst {
        get {
            if(Instance != null) {
                return true;
            }
            return false;
        }
    }
    
    public virtual void Awake() {
        
    }
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();	
	}

    public override void OnEnable() {

        Messenger<string>.AddListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.AddListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.AddListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);
    }

    public override void OnDisable() {

        Messenger<string>.RemoveListener(ButtonEvents.EVENT_BUTTON_CLICK, OnButtonClickEventHandler);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateIn,
            OnUIControllerPanelAnimateIn);

        Messenger<string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateOut,
            OnUIControllerPanelAnimateOut);

        Messenger<string, string>.RemoveListener(
            UIControllerMessages.uiPanelAnimateType,
            OnUIControllerPanelAnimateType);
    }

    public override void OnUIControllerPanelAnimateIn(string classNameTo) {
        if(className == classNameTo) {
            AnimateIn();
        }
    }

    public override void OnUIControllerPanelAnimateOut(string classNameTo) {
        if(className == classNameTo) {
            AnimateOut();
        }
    }

    public override void OnUIControllerPanelAnimateType(string classNameTo, string code) {
        if(className == classNameTo) {
           //
        }
    }

    public virtual void OnButtonClickEventHandler(string buttonName) {
        //LogUtil.Log("OnButtonClickEventHandler: " + buttonName);
    
        /*
        if(buttonName == buttonSatchelClothing.name) {
        changeList(BaseGameUIPanelStoreListType.Clothing);
        }
        else */

        if(buttonName.IndexOf(buttonBuyUseName + "$") > -1) {

             productCodeUse = "";
             productTypeUse = "";
             productCharacterUse = "";
             
             string[] commandActionParams = buttonName.Replace(buttonBuyUseName + "$", "").Split('$');
            
             if(commandActionParams.Length > 0)
                 productTypeUse = commandActionParams[0];
             if(commandActionParams.Length > 1)
                 productCodeUse = commandActionParams[1];
             if(commandActionParams.Length > 2)
                 productCharacterUse = commandActionParams[2];

            if(!string.IsNullOrEmpty(productTypeUse)
                && !string.IsNullOrEmpty(productCodeUse)
                && !string.IsNullOrEmpty(productCharacterUse)) {
                GameStoreController.Purchase(productCodeUse, 1);

                loadData(productTypeUse);
            }
        }
    }

    /*
 public static void ChangeList(BaseGameUIPanelStoreListType listType) {
     if(isInst) {
         Instance.changeList(listType);
     }
 }
 
 public void changeList(BaseGameUIPanelStoreListType listType) {
     panelListType = listType;
     loadData();
     AnimateInList();
 }
 */
	
	public static void LoadData(string productType) {
        if(GameUIPanelProducts.Instance != null) {
            GameUIPanelProducts.Instance.loadData(productType);
		}
	}
	
    public virtual void loadData(string productType) {

        //if(currentProductType == productType) {
        //    return;
        //}

		StartCoroutine(loadDataCo(productType));
	}
	
	IEnumerator loadDataCo(string productType) {
		
		LogUtil.Log("LoadDataCo");

        currentProductType = productType;
		
		if (listGridRoot != null) {
			listGridRoot.DestroyChildren();
			
	        yield return new WaitForEndOfFrame();
					
			loadDataProducts(productType);
			
	        yield return new WaitForEndOfFrame();
	        listGridRoot.GetComponent<UIGrid>().Reposition();
	        yield return new WaitForEndOfFrame();				
        }
	}

    public virtual void loadDataRPGUpgrades() {
        loadDataProducts(GameProductType.rpgUpgrade);
    }
	
    public virtual void loadDataPowerups() {		
		loadDataProducts(GameProductType.powerup);
	}
	
    public virtual void loadDataProducts(string type) {

		LogUtil.Log("Load loadDataProducts:type:" + type);
				
		List<GameProduct> products = null;

        if(!string.IsNullOrEmpty(type)) {
            products = GameProducts.Instance.GetListByType(type);
        }
        else {
            products = GameProducts.Instance.GetAll();
        }

        LogUtil.Log("Load products: products.Count: " + products.Count);
		
		int i = 0;
		
        foreach(GameProduct product in products) {
			
            GameObject item = NGUITools.AddChild(listGridRoot, listItemItemPrefab);
            item.name = "WeaponItem" + i;

            GameProductInfo info = product.GetDefaultProductInfoByLocale();

            UIUtil.UpdateLabelObject(item.transform,"LabelName",info.display_name);
            UIUtil.UpdateLabelObject(item.transform,"LabelDescription",info.description);
            UIUtil.UpdateLabelObject(item.transform,"LabelCost",info.cost);

            Transform inventoryItem = item.transform.FindChild("Container/Inventory");
            if(inventoryItem != null) {

                double currentValue = 0;

                if(product.type == GameProductType.rpgUpgrade) {
                    currentValue = GameProfileRPGs.Current.GetUpgrades();
                    UIUtil.UpdateLabelObject(inventoryItem,"LabelCurrentValue", currentValue.ToString("N0"));
                }
                else {
                    inventoryItem.gameObject.Hide();
                }
            }
			
			Transform iconTransform = item.transform.FindChild("Container/Icon");
			if(iconTransform != null) {
				GameObject iconObject = iconTransform.gameObject;	
				UISprite iconSprite = iconObject.GetComponent<UISprite>();			
	
				if(iconSprite != null) {
					iconSprite.alpha = 1f;
					
					// TODO change out image...
				}
			}
			
			// Update button action
			
			
			Transform buttonObject = item.transform.FindChild("Container/Button/ButtonAction");
			if(buttonObject != null) {
				UIImageButton button = buttonObject.gameObject.GetComponent<UIImageButton>();
				if(button != null) {
					
					// TODO change to get from character skin
					string productType = product.type;
					string productCode = product.code;
					string productCharacter = GameProfileCharacters.Current.GetCurrentCharacter().code;
					
					//productCode = productCode.Replace(productType + "-", "");
										
					button.name = buttonBuyUseName + "$" + productType + "$" + productCode + "$" + productCharacter;
				}
			}
			
			i++;
        }
	}
	
    public virtual void ClearList() {
		if (listGridRoot != null) {
			listGridRoot.DestroyChildren();
		}
	}
		
	public override void AnimateIn() {
		
		base.AnimateIn();
		
		LoadData(currentProductType);
	}

	public override void AnimateOut() {
		
		base.AnimateOut();
		
		ClearList();
	}
	
	
}