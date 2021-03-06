/*
IMPORTANT: READ BEFORE DOWNLOADING, COPYING, INSTALLING OR USING. 

By downloading, copying, installing or using the software you agree to this license.
If you do not agree to this license, do not download, install, copy or use the software.

    License Agreement For Kobayashi Maru Commander Open Source

Copyright (C) 2013, Chih-Jen Teng(NDark) and Koguyue Entertainment, 
all rights reserved. Third party copyrights are property of their respective owners. 

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

  * Redistribution's of source code must retain the above copyright notice,
    this list of conditions and the following disclaimer.

  * Redistribution's in binary form must reproduce the above copyright notice,
    this list of conditions and the following disclaimer in the documentation
    and/or other materials provided with the distribution.

  * The name of Koguyue or all authors may not be used to endorse or promote products
    derived from this software without specific prior written permission.

This software is provided by the copyright holders and contributors "as is" and
any express or implied warranties, including, but not limited to, the implied
warranties of merchantability and fitness for a particular purpose are disclaimed.
In no event shall the Koguyue or all authors or contributors be liable for any direct,
indirect, incidental, special, exemplary, or consequential damages
(including, but not limited to, procurement of substitute goods or services;
loss of use, data, or profits; or business interruption) however caused
and on any theory of liability, whether in contract, strict liability,
or tort (including negligence or otherwise) arising in any way out of
the use of this software, even if advised of the possibility of such damage.  
*/
/*
@file UnitSelectionSystem.cs
@brief 單位的描準系統
@author NDark
 
目前只有主角在用
請參看 文件 單位的瞄準系統

# 本瞄準系統主要是紀錄目前的瞄準資訊
## 目前主要瞄準框為 GUI_Selection00
## 其他瞄準框為特殊攻擊模式使用
# 點選瞄準的流程如下
## 點選物件 : 檢查滑鼠單擊 觸發 掛在物件下cube的 script: ClickOnThisCollider.cs
## 因為只有主角(玩家)可以點擊
所以此瞄準系統必定是找主角物件的瞄準系統 
呼叫 主角的瞄準系統 紀錄點擊物件 UnitSelectionSystem::ClickOnUnit()
判斷後 呼叫 函式 ActiveSelectInformation() 啟動/關閉瞄準框
## 因為畫面上要顯示玩家的瞄準狀態,所以GUIUpdate會去找主角物件的瞄準系統來顯示
## 如果未來AI也要瞄準,則AI腳本會自己找自己的瞄準系統.
# 目前只有主角有瞄準系統
# 當取消點選目標時，會順帶通知GUIUpdate摧毀目標的UnitDataGUI
# 檢查點選單位 ClickOnUnit()
## 自己不點選
## 目前只有一個選擇框
## 判定是要切換對象還是開啟關閉
## 通知 TutorialEvent 點選測試完畢
# ActiveSelectInformation() 啟動瞄準指定物件 
## 啟動對象的武器範圍
## 呼叫 GUIUpdate 創造或摧毀選擇物件的 UnitDataGUI 
## 關閉顯示 UnitDataGUI 部位瞄準的顯示
# GetSelectUnitName() 取得目前主要瞄準單位名稱
# GetPrimarySelectInfo() 取得目前主要瞄準資訊
# SetPrimarySelectInfo() 設定目前主要瞄準資訊
# ClickOnUnit() 紀錄點擊物件
# ClearUnitComponent() 清除部位資訊
# SpecifiedUnitComponent() 指定部位資訊
# 更新
## 如果瞄準部位下線，則立即關閉部位資訊
# 檢查點選教學


@date 20121110 by NDark 
. change type of guiPosition to Rect.
. rename local variable guiPosition to guiRect
. change label to use ComponentParam DisplayName
. move class member m_GUI_SelectUnitBackgroundTemplateName to GUIUpdate.cs
. move class method CreateUnitDataGUI() to GUIUpdate.cs
. move class method DestroyTargetUnitDataGUI() to GUIUpdate.cs
. refine class method ActiveSelectInformation()
. move class GUIObjNameSet to GUIUpdate.cs
. fix an error of destroy GUI object twice at ClickOnUnit()
...
@date 20130103 by NDark . add class method GetTargetUnit()

*/
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Main character selection info.
/// -# isValid 啟動與否
/// -# clickActiveTime 啟動時間
/// -# screenPosition 螢幕座標
/// -# targetName 瞄準目標
/// </summary>
public class SelectInformation
{
	public SelectInformation()
	{
	}
	
	public SelectInformation( SelectInformation _src )
	{
		isValid = _src.isValid ;
		clickActiveTime = _src.clickActiveTime ;
		screenPosition = _src.screenPosition ;
		m_TargetComponentName = _src.m_TargetComponentName ;
		m_TargetUnit = new NamedObject( _src.m_TargetUnit ) ;
	}	
	public SelectInformation( bool _Valid , 
							  Vector3 _ScreenPosition , 
							  string _TargetUnitName )
	{
		isValid = _Valid ;
		screenPosition = _ScreenPosition ;
		TargetUnitName = _TargetUnitName ;		
	}
	
	// This selection is valid (active)
	public bool isValid = false ;
	
	// the time of active
	public float clickActiveTime = 0.0f ;
	
	// the screen position of this selection
	public Vector3 screenPosition = new Vector3( 0 , 0 , 0 ) ;
	
	// the target component name of this selection
	public string m_TargetComponentName = ConstName.UnitDataComponentUnitIntagraty ;	
	
	public NamedObject GetTargetUnit()
	{
		return m_TargetUnit ;
	}
	public string TargetUnitName
	{
		get
		{
			return m_TargetUnit.Name ;
		}
		set
		{
			m_TargetUnit.Name = value ;
			m_TargetComponentName = ConstName.UnitDataComponentUnitIntagraty ;// reset each time.
		}
	}
	public GameObject TargetUnitObject
	{
		get
		{
			return m_TargetUnit.Obj ;
		}
		set
		{
			m_TargetUnit.Name = value.name ;
			m_TargetUnit.Obj = value ;			
		}
	}	
	// the target unit name of this selection
	private NamedObject m_TargetUnit = new NamedObject() ;
	
}

public class UnitSelectionSystem : MonoBehaviour 
{
	// the table of all select GUI object , the key is the name of GUI Object
	public Dictionary<string , SelectInformation> m_Selections = new Dictionary<string, SelectInformation>() ;
	
	public int m_MaxSelectionNum = 5 ;
	private string m_SelectionKeyPrefix = "GUI_Selection" ;
	private string m_PrimarySelectionKey = "GUI_Selection00" ;
	
	// Is select enemy for tutorial event.
	public bool m_IsSelectEnemy = false ;
	
	// 創造或摧毀選擇物件的 UnitDataGUI 
	public void ActiveSelectInformation( string _TargetName , bool _Active )
	{
		// Debug.Log( "ActiveSelectInformation" + _TargetName + " " + _Active ) ;
		GUIUpdate guiUpdate = GlobalSingleton.GetGUIUpdateComponent() ;
		if( null == guiUpdate )
			return ;					

		if( true == _Active )
		{
			guiUpdate.CreateSelectTargetUnitDataGUI( _TargetName ) ;
			GameObject TargetUnit = GameObject.Find( _TargetName ) ;
			
			if( null != TargetUnit )
			{
				UnitWeaponSystem weaSys = TargetUnit.GetComponent<UnitWeaponSystem>() ;
				if( null != weaSys )
				{
					weaSys.ActiveWeaponRangeObject("" ) ;
				}
			}
		}
		else
		{
			// close selection
			guiUpdate.DestroyTargetUnitDataGUI() ;
			
			// hide the unit data selection 
			ShowGUITexture.Show( ConstName.GUIUnitDataSelection_UnActive , false , false , true ) ;
		}
	}
	
	// 取得目前瞄準單位名稱
	public string GetPrimarySelectUnitName() 
	{
		if( false == m_Selections.ContainsKey( m_PrimarySelectionKey ) )
			return "" ;
		SelectInformation selectInfo = m_Selections[m_PrimarySelectionKey] ;
		if( selectInfo.isValid == true )
		{
			return selectInfo.TargetUnitName ;
		}
		return "" ;
	}
	
	public SelectInformation GetPrimarySelectInfo() 
	{
		if( false == m_Selections.ContainsKey( m_PrimarySelectionKey ) )
			return null ;
		SelectInformation ret = m_Selections[m_PrimarySelectionKey] ;
		return ret ;
	}	
	public void SetPrimarySelectInfo( SelectInformation _Set ) 
	{
		if( true == m_Selections.ContainsKey( m_PrimarySelectionKey ) )
		{
			m_Selections[m_PrimarySelectionKey] = _Set ;
		}
	}	
	
	// 紀錄點擊物件 
	public void ClickOnUnit( string _Name )
	{
		SystemLogManager.AddLog( SystemLogManager.SysLogType.ClickOnUnit , _Name ) ;
		
		// Debug.Log( "click" + _Name ) ;
		if( this.gameObject.name == _Name )
		{
			Debug.Log( "no main character click on itself" ) ;
			return ;
		}

		if( false == m_Selections.ContainsKey( m_PrimarySelectionKey ) )
			return ;
		
		SelectInformation selectInfo = m_Selections[m_PrimarySelectionKey] ;
		bool oldvalid = selectInfo.isValid ;
		string oldName = selectInfo.TargetUnitName ;
		if( _Name == selectInfo.TargetUnitName )
		{
			selectInfo.isValid = !selectInfo.isValid ;
		}
		else
		{
			selectInfo.isValid = true ;
		}
		
		selectInfo.TargetUnitName = _Name ;
		
		if( selectInfo.TargetUnitName == oldName &&
			selectInfo.isValid != oldvalid &&
			0 != selectInfo.TargetUnitName.Length )
		{
			// switch in the same object
			if( false == selectInfo.isValid )
				ActiveSelectInformation( selectInfo.TargetUnitName , false ) ;
			else
				ActiveSelectInformation( selectInfo.TargetUnitName , true ) ;
		}
		else if( selectInfo.TargetUnitName != oldName )
		{
			// switch in different target
			if( 0 != oldName.Length )
				ActiveSelectInformation( oldName , false ) ;
			
			if( 0 != selectInfo.TargetUnitName.Length )
				ActiveSelectInformation( selectInfo.TargetUnitName , selectInfo.isValid ) ;
		}		
		
		// Debug.Log( "ClickOnUnit " + _Name ) ;
		
		// for tutorial event.
		if( true == selectInfo.isValid )
		{
			// check ever select enemy
			ConfirmTutorialSelectTarget() ;

			selectInfo.clickActiveTime = Time.time ;
		}
	}
	
	public void ClearUnitComponent()
	{
		if( false == m_Selections.ContainsKey( m_PrimarySelectionKey ) )
			return ;
		m_Selections[ m_PrimarySelectionKey ].m_TargetComponentName = ConstName.UnitDataComponentUnitIntagraty ;
	}
	
	public void SpecifiedUnitComponent( string _ComponentName )
	{
		if( false == m_Selections.ContainsKey( m_PrimarySelectionKey ) )
			return ;
		if( false == m_Selections[ m_PrimarySelectionKey ].isValid )
			return ;
		m_Selections[ m_PrimarySelectionKey ].m_TargetComponentName = _ComponentName ;
	}
	
	// Use this for initialization
	void Start () 
	{
		for( int i = 0 ; i < m_MaxSelectionNum ; ++i )
		{
			string key = m_SelectionKeyPrefix + string.Format( "{0:00}" , i ) ;
			// Debug.Log( key ) ;
			this.m_Selections.Add( key , 
				new SelectInformation( false , new Vector3( 0.5f , 0.5f , 0.0f ) , "" ) ) ;	
			
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		// 檢查部件下線時清除指定部件資訊
		Dictionary<string , SelectInformation>.Enumerator selectionEnum = m_Selections.GetEnumerator() ;
		while( selectionEnum.MoveNext() )
		{
			SelectInformation selectInfo = selectionEnum.Current.Value ;
			UnitData unitData = GlobalSingleton.GetUnitData( selectInfo.TargetUnitName ) ;
			if( null != unitData )
			{
				if( true == unitData.componentMap.ContainsKey( selectInfo.m_TargetComponentName ) )
				{
					if( true == unitData.componentMap[ selectInfo.m_TargetComponentName ].IsOffline() )
					{
						ClearUnitComponent() ;
					}
				}
			}
		}
	}

	private void ConfirmTutorialSelectTarget()
	{
		// check ever select enemy
		if( false == m_IsSelectEnemy )
		{
			m_IsSelectEnemy = true ;
			TutorialEvent tutorialEvent = GlobalSingleton.GetTutorialEvent() ;
			if( null != tutorialEvent )
				tutorialEvent.m_IsPressedSelectEnemy = m_IsSelectEnemy ;
		}
	}	
}
