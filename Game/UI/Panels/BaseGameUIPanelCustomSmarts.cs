using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Events;

public class BaseGameUIPanelCustomSmarts : GameUIPanelBase {
    
    public static GameUIPanelCustomSmarts Instance;
	
    public GameObject listItemPrefab;
	
	public GameObject helmetObject;
	public GameObject helmetObjectRotator;
	
	public UIImageButton buttonClose;
	
    public UILabel labelPlaySmartScore;
    
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
	
	public override void Start() {
		Init();
	}
	
	public override void Init() {
		base.Init();	
		
		loadData();
		
		double score = GameProfileRPGs.Current.GetGamePlayerProgressXP();
		
		SetScore(score);
	}
	
    public virtual void LoadDefault() {
		LoadData();
	}	
	
	public static void ShowDefault() {
        if(GameUIPanelCustomSmarts.Instance != null) {
            GameUIPanelCustomSmarts.Instance.AnimateIn();
            GameUIPanelCustomSmarts.Instance.LoadDefault();
		}
	}
	 	 
    public virtual void OnButtonClickEventHandler(string buttonName) {
		//LogUtil.Log("OnButtonClickEventHandler: " + buttonName);
	}
	
	public static void LoadData() {
        if(GameUIPanelCustomSmarts.Instance != null) {
            GameUIPanelCustomSmarts.Instance.loadData();
		}
	}
	
    public virtual void loadData() {
		StartCoroutine(loadDataCo());
	}
	
	IEnumerator loadDataCo() {
		
		GameUIController.Instance.currentDraggableGameObject = helmetObjectRotator;
		
		yield return new WaitForSeconds(1f);
	}
	
	public static void HideAll() {
        if(GameUIPanelCustomSmarts.Instance != null) {
            GameUIPanelCustomSmarts.Instance.AnimateOut();
		}
	}			
	
    public virtual void SetScore(double score) {
		UIUtil.SetLabelValue(labelPlaySmartScore, score.ToString("N0"));
	}
	
	public override void AnimateIn() {
				
		base.AnimateIn();		
		
		//ShowPanelDefault();
	}		
	
	public override void AnimateOut() {
		
		base.AnimateOut();		
		
		GameUIController.Instance.currentDraggableGameObject = null;
		
		//HidePanelDefault();
	}

    public virtual void Update() {

        if(GameConfigs.isGameRunning) {
            return;
        }

        if(!isVisible) {
            return;
        }
    }
	
}