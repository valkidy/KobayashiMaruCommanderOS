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
@file LevelGenerator.cs
@brief 關卡讀檔
@author NDark
 
說明

# 進入關卡場景後，再依照目前的關卡關鍵字去取得關卡檔，然後再把關卡使用動態的方式建立起來。
# 關卡關鍵字目前有兩種指定方式
## Debug_UseGlobalSingletonLevelString 開啟情形下(預設)，向　GlobalSingleton取得。而GlobalSingleton是由選擇關卡場景所設定。
## Debug_UseGlobalSingletonLevelString 關閉情形下，直接使用原本設定值，方便直接開啟關卡，方便除錯。
# 目前有五大資料檔要讀取
## 關卡表 LevelTable.xml
## 單位樣版檔 UnitTable.xml
## 元件參數檔 ComponentParamTable.xml
## 武器參數檔 WeaponParamTable.xml
## 真正開啟的關卡檔

初始化

# LoadComponentParamTable() load component param table
# LoadWeaponParamTable() load weapon param table
# LoadUnitTemplateTable() 讀入單位樣本表格
儲存單位樣本 UnitTemplateData 的所有資訊.
每次單位要產生時就把單位樣本的資料複製到物件上
每個 單位樣板資料 包含
樣板名稱
開啟/加入元件名稱
標準資料清單
單位部件清單
# LoadLevelTable() 讀入關卡清單
節點 <LevelFilePath>
# RetrieveLevelIndex() 取得正確的關卡索引
# LoadLevelAccordingToIndexofLevel() 依據索引讀入正確的關卡參數檔
其中每個單位是以 單位初始資料的 資料結構組成
每筆 單位初始資料 UnitInitializationData 包含 單位名稱 樣板名稱 初始位置 初始轉向
呼叫 ParseUnitInitData() 取出每筆單位的資料
存入 levelUnitInitializationTable[] 中
注意目前的位置指定與 BackgroundObj 的縮放及大小有絕對關係。
位置的指定可以用　重生點　也可以指定絕對位置
# GenerateAllUnitsInThisLevel() 依照讀入的資料建立本關單位
注意目前關卡檔產生出來的敵人單位也要列入勝負判定的表格中。
未來勝負判定模組化時再行調整。
呼叫 GenerateUnit() 來創造單位
其中除了具現化單位之外 還要指定初始位置旋轉 自unitTemplateTable[]中複製對應的單位資料
單位產生後呼叫　GUI_Unit_Update() 來確實產生血條物件
# GenerateAllVictoryConditionInThisLevel() 產生勝負條件
# GenerateEnemyGenerationInThisLevel() 產生單位產生事件
# GenerateUsualEvnetInThisLevel() 產生一般事件
# CreateMainCharacterUnitDataGUI() 單位產生完畢後產生玩家的UnitData GUI
Create Main Character Unit Data GUI after all unit been created.
		

@date 20121110 by NDark . add comment.
...
@date 20130103 by NDark . add code of XMLParseLevelUtility.ParseConversationSet() at LoadLevelAccordingToIndexofLevel()
@date 20130103 by NDark . fix an error of UnitInitialization at LoadLevelAccordingToIndexofLevel()
@date 20130109 by NDark . add argument of GenerateUnit() 
@date 20130115 by NDark 
. replace templateName by ConditionName at ParseVictoryCondition()
. replace templateName by ConditionName at ParseLoseCondition()
@date 20130126 by NDark
. add argument _UnitDisplayNameIndex of ParseUnitTemplateData()
. add code of GlobalSingleton.m_CurrentModeSelectSceneString at RetrieveLevelIndex()
. refactor class method ParseUnitTemplateData()
@date 20130204 by NDark . add class method CheckCustomMainCharacter()
@date 20130206 by NDark . replace m_CurrentModeSelectSceneString by m_InformationSceneEnd at RetrieveLevelIndex()
@date 20130213 by NDark . add checking of unitObj at GenerateUnit()
*/
// #define DEBUG

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml;


public class LevelGenerator : MonoBehaviour 
{
	// level table filepath 
	public string levelTableFilepath = "LevelTable.xml" ;
	
	// levet table read from levelTableFilepath
	public List<string> levelTable = new List<string>() ;

	// level index preparing to be load.
	public string levelString = "" ;
	public bool Debug_UseGlobalSingletonLevelString = true ;
	
	// unit initialization table of this level
	public List<UnitInitializationData> levelUnitInitializationTable = new List<UnitInitializationData>() ;
	public List<Condition> levelVictoryConditonTable = new List<Condition>() ;
	public List<Condition> levelLoseConditonTable = new List<Condition>() ;
	public List<UnitGenerationData> levelEnemyGenerationTable = new List<UnitGenerationData>() ;
	public List<UsualEvent> levelEventTable = new List<UsualEvent>() ;
	
	// unit template table filepath
	public string unitTemplateTableFilepath = "UnitTable.xml" ;

	// unit template table read from unitTemplateTableFilepath
	public Dictionary< string , UnitTemplateData > unitTemplateTable = new Dictionary< string , UnitTemplateData > () ;
	
	// component param
	public string componentParamTableFilepath = "ComponentParamTable.xml" ;
	public Dictionary< string , ComponentParam > componentParamTable = new Dictionary<string, ComponentParam>() ;
	public ComponentParam [] Debug_componentParamTable_valueVec ;
	
	// weapon param
	public string weaponParamTableFilepath = "WeaponParamTable.xml" ;
	public Dictionary< string , WeaponParam > weaponParamTable = new Dictionary<string, WeaponParam>() ;
	public WeaponParam [] Debug_weaponParamTable_valueVec ;	
	
	
	public int Debug_LevelUnitInitSize = 0 ;
	public int Debug_LevelUnitTemplateTableSize = 0 ; 
	
	
	/*
	 * @brief Generate the specified Unit.
	 * 
	 */
	public GameObject GenerateUnit( string _UnitName , 
									string _PrefabeTemplateName , 
									string _UnitDataTemplateName ,
									string _RaceName ,		
									string _SideName ,	
									Vector3 _InitPos , 
									Quaternion _InitOreintation , 
									Dictionary<string,string> _SupplementalVec )
	{
		bool GUIIsFlip = ( _UnitName != ConstName.MainCharacterObjectName ) ;
		
		// Debug.Log( "GenerateUnit:" + unitName.ToString() ) ;
		GameObject unitObj = PrefabInstantiate.CreateByInit( _PrefabeTemplateName ,
														     _UnitName,
															 _InitPos,
															 _InitOreintation ) ;
		if( null == unitObj )
			return unitObj ;
		
		// confirm init position and orientation
		UnitInitialization unitInitScript = unitObj.GetComponent<UnitInitialization>() ;
		if( null == unitInitScript )
		{
			Debug.Log( "GenerateUnit() : null == unitInitScript" ) ;
		}
		else
		{
			unitInitScript.UnitInitializationPoint = _InitPos ;
			unitInitScript.UnitInitializationOrientation = _InitOreintation ;
		}
		
		// copy unit data 
		UnitData unitData = unitObj.GetComponent<UnitData>() ;
		unitData.m_PrefabTemplateName = _PrefabeTemplateName ;
		unitData.m_UnitDataTemplateName = _UnitDataTemplateName ;
		unitData.m_RaceName = _RaceName ;
		unitData.m_SideName = _SideName ;
		unitData.m_SupplementalVec = _SupplementalVec ;		
		
		// retrieve unit data from unitTemplateTable to component of UnitData
		if( false == this.unitTemplateTable.ContainsKey( _UnitDataTemplateName ) )
		{
			Debug.Log( "GenerateUnit() : false == this.unitTemplateTable.ContainsKey()=" + _UnitDataTemplateName ) ;
		}
		else if( null == unitData )
		{
			Debug.Log( "GenerateUnit() : null == unitData" ) ;
		}
		else
		{
			UnitTemplateData unitDataTemplateData = this.unitTemplateTable[ _UnitDataTemplateName ] ;
			unitData.m_UnitDataGUITextureName = unitDataTemplateData.unitDataGUITextureName ;
			unitData.m_DisplayNameIndex = unitDataTemplateData.unitDisplayNameIndex ;
			
			// assign component
			for( int i = 0 ; i < unitDataTemplateData.unitComponents.Count ; ++i )
			{
				// Debug.Log( "unitData.AssignComponent " + unitDataTemplateData.unitComponents[ i ].m_Name ) ;
				string componentName = unitDataTemplateData.unitComponents[ i ].m_Name ;
				unitData.AssignComponent( componentName ,
										  unitDataTemplateData.unitComponents[ i ] ) ;
				
				// set gui flip flag at component added.
				unitData.componentMap[ componentName ].m_ComponentParam.m_IsFlip = GUIIsFlip ;
			}
			
			// assign standard parameters
			string [] keyarray = new string[ unitDataTemplateData.standardParameters.Count ];
			unitDataTemplateData.standardParameters.Keys.CopyTo( keyarray , 0 ) ;
			
			for( int j = 0 ; j < unitDataTemplateData.standardParameters.Count ; ++j )
			{				
				unitData.AssignStandardParameter( keyarray[j] , 
												  unitDataTemplateData.m_standardParameters[ keyarray[j] ] ) ;				
			}
			
			// add component
			for( int i = 0 ; i < unitDataTemplateData.m_AddComponentList.Count ; ++i )
			{
				// Debug.Log( "templateData.m_AddComponentList " + unitDataTemplateData.m_AddComponentList[ i ] ) ;
				unitObj.AddComponent( unitDataTemplateData.m_AddComponentList[ i ] ) ;
			}
		}
		
		
		
			
		// active gui of this unit after we assign the parameter
		GUI_Unit_Update guiUnitUpdateComponent = unitObj.GetComponent<GUI_Unit_Update>() ;
		if( null != guiUnitUpdateComponent )
		{
			guiUnitUpdateComponent.Active_GUI_Unit( true ) ;
		}
		
		return unitObj ;
	}
	
	// Use this for initialization
	void Start () 
	{
		LoadComponentParamTable() ;
		LoadWeaponParamTable() ;
		LoadUnitTemplateTable() ;
		LoadLevelTable() ;
		
		RetrieveLevelIndex() ;
		LoadLevelAccordingToIndexofLevel() ;
		GenerateAllUnitsInThisLevel() ;
		CheckCustomMainCharacter() ;
		GenerateAllVictoryConditionInThisLevel() ;
		GenerateEnemyGenerationInThisLevel() ;
		GenerateUsualEvnetInThisLevel() ;
		
		// Create Main Character Unit Data GUI after all unit been created.
		CreateMainCharacterUnitDataGUI() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	
	/* 
	 * @brief Parse component from xml 
	 */
	bool ParseComponent( XmlNode _ComponentNode ,
						 out UnitComponentData _Component )
	{

		_Component = new UnitComponentData() ;
		
		if( null == _ComponentNode )
			return false ;
	
		if( null == _ComponentNode.Attributes[ "name" ] )
			return false ;
		
		string name = _ComponentNode.Attributes[ "name" ].Value ;
		if( 0 == name.Length )
			return false ;
		
		// assign component parameter from table componentParamTable[].
		if( true == this.componentParamTable.ContainsKey( name ) )
		{
			_Component.m_ComponentParam = new ComponentParam( this.componentParamTable[ name ] ) ;
		}

		// assign weapon parameter from table weaponParamTable[].
		if( true == this.weaponParamTable.ContainsKey( name ) )
		{
			_Component.m_WeaponParam = new WeaponParam( this.weaponParamTable[ name ] ) ;
		}
		
		_Component.m_Name = name ;
		for( int i = 0 ; i < _ComponentNode.ChildNodes.Count ; ++i )
		{
			string label ;
			StandardParameter newStandardParameter ;			
			XMLParseLevelUtility.ParseStandardParameter( _ComponentNode.ChildNodes[ i ] ,
														out label , 
														out newStandardParameter ) ;
			
			/*
				public StandardParameter m_HP = new StandardParameter() ;
				public StandardParameter m_Generation = new StandardParameter() ;		
				public StandardParameter m_Energy = new StandardParameter() ;
				public StandardParameter m_Effect = new StandardParameter() ;	
				
				public ComponentStatus m_Status = ComponentStatus.ComponentStatus_Normal ;
				public StatusDescription m_StatusDescription = new StatusDescription() ;
				public InterpolateTable m_Effect_HP_Curve = new InterpolateTable() ;
				
				// weapon only
				public StandardParameter m_ReloadEnergy = new StandardParameter() ;
				public StandardParameter m_ReloadGeneration = new StandardParameter() ;
				public WeaponStatus m_WeaponStatus ;
			 
			 */
			if( "hp" == label )
			{
				_Component.m_HP = new StandardParameter( newStandardParameter ) ;
			}
			else if( "energy" == label )
			{
				_Component.m_Energy = new StandardParameter( newStandardParameter ) ;
			}
			else if( "generation" == label )
			{
				_Component.m_Generation = new StandardParameter( newStandardParameter ) ;
			}
			else if( "effect" == label )
			{
				_Component.m_Effect = new StandardParameter( newStandardParameter ) ;
			}
			
			else if( "reloadEnergy" == label )
			{
				_Component.m_ReloadEnergy = new StandardParameter( newStandardParameter ) ;
			}
			else if( "reloadGeneration" == label )
			{
				_Component.m_ReloadGeneration = new StandardParameter( newStandardParameter ) ;
			}
			
		}
		
		return ( 0 != _Component.m_Name.Length ) ;
	}
	

	/* 
	 * @brief Parse unit from xml
	 */
	bool ParseUnitTemplateData( XmlNode _UnitNode , 
								out UnitTemplateData _Data )
	{
		_Data = new UnitTemplateData() ;
		
		if( null == _UnitNode.Attributes[ "unitDataTemplateName" ] )
			return false ;
		
		_Data.unitDataTemplateName = _UnitNode.Attributes[ "unitDataTemplateName" ].Value ;
		
		if( null != _UnitNode.Attributes[ "unitDataGUITextureName" ] )
			_Data.unitDataGUITextureName = _UnitNode.Attributes[ "unitDataGUITextureName" ].Value ;
		
		if( null != _UnitNode.Attributes[ "UnitDisplayNameIndex" ] )
		{
			string UnitDisplayNameIndexStr = _UnitNode.Attributes[ "UnitDisplayNameIndex" ].Value ;
			int.TryParse( UnitDisplayNameIndexStr , out _Data.unitDisplayNameIndex ) ;
		}
		
		for( int i = 0 ; i < _UnitNode.ChildNodes.Count ; ++i )
		{
			if( -1 != _UnitNode.ChildNodes[ i ].Name.IndexOf( "comment" ) )
			{
				// comment
			}
			else if( "AddComponent" == _UnitNode.ChildNodes[ i ].Name )
			{
				if( null != _UnitNode.ChildNodes[ i ].Attributes["name"] )
				{
					_Data.m_AddComponentList.Add( _UnitNode.ChildNodes[ i ].Attributes["name"].Value ) ;
				}
			}
			else if( "Component" == _UnitNode.ChildNodes[ i ].Name )
			{
				// parse compoent
				UnitComponentData componentData ;
				if( true == ParseComponent( _UnitNode.ChildNodes[ i ] , 
											out componentData ) )
				{
					_Data.unitComponents.Add( componentData ) ;
				}
			}
			else 
			{
				// Debug.Log( "_UnitNode.ChildNodes[ i ]" + _UnitNode.ChildNodes[ i ].Name ) ;
				// parse standard parameter
				string name ;
				StandardParameter param ;
				if( true == XMLParseLevelUtility.ParseStandardParameter( _UnitNode.ChildNodes[ i ] ,
																		out name , 
																		out param ) )
				{
					_Data.standardParameters.Add( name ,param ) ;
				}
			}
		}
		return true ;
	}
	
	/*
	 * @brief Load unit template table to unitTemplateTable
	 */
	void LoadUnitTemplateTable()
	{
		XmlDocument doc = new XmlDocument() ;
		if( false == LoadDataToXML.LoadToXML( ref doc , this.unitTemplateTableFilepath ) )
			return ;
		XmlNode root = doc.FirstChild ;
		if( false == root.HasChildNodes )
		{
			Debug.Log( "LoadUnitTemplateTable() : false == root.HasChildNodes" ) ;
			return ;
		}
		
		for( int i = 0 ; i < root.ChildNodes.Count ; ++i )
		{
			if( "Unit" == root.ChildNodes[ i ].Name )
			{
				XmlNode unitNode = root.ChildNodes[ i ] ;
				
				// parse unit
				UnitTemplateData newData = null ;
				if( false == ParseUnitTemplateData( unitNode , 
												    out newData ) )
				{
					continue ;
				}
				
				// add into unitTemplateTable
				if( true == unitTemplateTable.ContainsKey( newData.unitDataTemplateName ) )
				{
					Debug.Log( "LoadUnitTemplateTable() has already the same unitDataTemplateName=" + newData.unitDataTemplateName ) ;
				}
				else 
				{
					unitTemplateTable.Add( newData.unitDataTemplateName , newData ) ;
				}
				Debug_LevelUnitTemplateTableSize = unitTemplateTable.Count ;
			}
		}
	}
	
	/// <summary>
	/// Loads the level table into levelTable[]
	/// </summary>
	void LoadLevelTable()
	{
		levelTable.Clear() ;
		
		XmlDocument doc = new XmlDocument() ;
		if( false == LoadDataToXML.LoadToXML( ref doc , this.levelTableFilepath ) )
			return ;		
		
		XmlNode root = doc.FirstChild ;
		if( false == root.HasChildNodes )
		{
			Debug.Log( "LoadLevelTable() : false == root.HasChildNodes" ) ;
			return ;
		}
		
		for( int i = 0 ; i < root.ChildNodes.Count ; ++i )
		{
			XmlNode levelpathNode = root.ChildNodes[ i ] ;
			if( "LevelFilePath" == levelpathNode.Name )
			{
				XmlAttribute pathAtt = levelpathNode.Attributes[ "path" ] ;
				levelTable.Add( pathAtt.Value ) ;
			}
		}		
	}

	// 取得正確的關卡索引字串
	void RetrieveLevelIndex()
	{
		if( true == Debug_UseGlobalSingletonLevelString )
		{
			// Debug.Log( "GlobalSingleton.m_LevelString=" +GlobalSingleton.m_LevelString ) ;
			this.levelString = GlobalSingleton.m_LevelString ;
		}
		
		// 依照關卡名稱設定 戰場打完之後要跳回哪個選擇關卡?
		if( -1 != this.levelString.IndexOf( "Challenge" ) )
		{
			GlobalSingleton.m_InformationSceneEnd = "Scene_ChallengeModeSelectLevel" ;
		}
		else
			GlobalSingleton.m_InformationSceneEnd = "Scene_SelectLevel" ;
#if DEBUG		
		Debug.Log( "GlobalSingleton.m_InformationSceneEnd=" + GlobalSingleton.m_InformationSceneEnd ) ;
#endif		
	}
	
	// 分析XML 勝負條件
	bool ParseVictoryCondition( XmlNode _UnitNode )
	{
		if( "VictoryCondition" != _UnitNode.Name )
		{
			// Debug.Log( "ParseVictoryCondition() : VictoryCondition != _UnitNode.Name=" + _UnitNode.Name ) ;
			return false ;
		}
		
		if( null == _UnitNode.Attributes["ConditionName"] )
			return false ;
		
		
		string ConditionName = _UnitNode.Attributes["ConditionName"].Value ;
#if DEBUG			
		Debug.Log( "LevelGenerator::ParseVictoryCondition() : ConditionName="+ ConditionName ) ;
#endif				
		Condition addCondition = ConditionFactory.GetByString( ConditionName ) ;
		if( null == addCondition )
			return false ;
		
		if( false == addCondition.ParseXML( _UnitNode ) )
		{
			Debug.Log( "LevelGenerator::ParseVictoryCondition() : addCondition.ParseXML is failed." ) ;
			return false ;
		}		
	
		this.levelVictoryConditonTable.Add( addCondition ) ;
		
		return true ;
	}
	
	// 分析XML 失敗條件
	bool ParseLoseCondition( XmlNode _UnitNode )
	{
		if( "LoseCondition" != _UnitNode.Name )
		{
			// Debug.Log( "ParseLoseCondition() : LoseCondition != _UnitNode.Name=" + _UnitNode.Name ) ;
			return false ;
		}
		
		if( null == _UnitNode.Attributes["ConditionName"] )
			return false ;
		

		string ConditionName = _UnitNode.Attributes["ConditionName"].Value ;
#if DEBUG			
		Debug.Log( "LevelGenerator::ParseLoseCondition() : ConditionName="+ ConditionName ) ;
#endif			
		Condition addCondition = ConditionFactory.GetByString( ConditionName ) ;
		if( null == addCondition )
			return false ;
		
		// insert parameter
		if( false == addCondition.ParseXML( _UnitNode ) )
		{
			Debug.Log( "LevelGenerator::ParseLoseCondition() : addCondition.ParseXML is failed." ) ;
			return false ;
		}
		this.levelLoseConditonTable.Add( addCondition ) ;
		
		return true ;
	}

	
	// 分析XML 創造一般事件
	bool ParseUsualEvent( XmlNode _EventNode )
	{ 
		if( "UsualEvent" != _EventNode.Name ||
			null == _EventNode.Attributes["EventName"] )
			return false ;
		
		string EventName = _EventNode.Attributes["EventName"].Value ;
		UsualEvent addEvent = UsualEventFactory.GetByString( EventName ) ;
		if( null == addEvent )
		{
// #if DEBUG
			Debug.Log( "ParseUsualEvent() : null == addEvent EventName=" + EventName ) ;
// #endif			
			return false ;
		}
		
		if( true == addEvent.ParseXML( _EventNode ) )
		{
			levelEventTable.Add( addEvent ) ;
			return true ;
		}
		
		Debug.Log( "ParseUsualEvent() return false " ) ;
		return false ;
	}
	


	
	/* Load Level According To Index of Level() 
	 * 依據索引讀入正確的關卡參數檔 */
	void LoadLevelAccordingToIndexofLevel()
	{
		string levelfilepath = "" ;
		List<string>.Enumerator eLevelName = this.levelTable.GetEnumerator() ;
		while( eLevelName.MoveNext() )
		{
			if( -1 != eLevelName.Current.IndexOf( this.levelString ) )
			{
				levelfilepath = eLevelName.Current ;
				break ;
			}
		}
		
		if( 0 == levelfilepath.Length )
		{
			// warning
			return ;
		}
		// Debug.Log( "levelfilepath=" + levelfilepath ) ;
		
		XmlDocument doc = new XmlDocument() ;
		if( false == LoadDataToXML.LoadToXML( ref doc , levelfilepath ) )
			return ;			
		
		XmlNode root = doc.FirstChild ;
		if( "Level" != root.Name )
		{
			Debug.Log( "LoadLevelAccordingToIndexofLevel() : Level != root.Name" ) ;
			return ;
		}
		
		// string levelname = root.Attributes[ "name" ].Value ;
		if( false == root.HasChildNodes )
		{
			Debug.Log( "LoadLevelAccordingToIndexofLevel() : Level != root.Name" ) ;
			return ;			
		}
		
		ConversationManager manager = GlobalSingleton.GetConversationManager() ;
		
		for( int i = 0 ; i < root.ChildNodes.Count ; ++i )
		{
			XmlNode unitNode = root.ChildNodes[ i ] ;
			string unitName ;
			string prefabTemplateName ;
			string unitDataTemplateName ;
			string raceName ;
			string sideName ;
			Vector3 position = Vector3.zero ;
			PosAnchor posAnchor = null ;
			Quaternion orientation ;
			Dictionary<string,string> supplemental ;
			float time ;
			string objectPlugByScriptName = "" ;
			string scriptPlugIntoObjectName = "" ;
			string audioPath = "" ;
			ConversationSet conversationSet = null ;
			
			// Debug.Log( "LoadLevelAccordingToIndexofLevel() : unitNode.Name" + unitNode.Name ) ;
			
			if( -1 != unitNode.Name.IndexOf( "comment" ) )
			{
				// comment
			}
			else if( true == XMLParseLevelUtility.ParseUnit( unitNode , 
														    out unitName , 
														    out prefabTemplateName , 
														    out unitDataTemplateName ,
														    out raceName ,
														    out sideName ,				
														    out posAnchor , 
														    out orientation ,
															out supplemental ) )
			{
				
				// add into unit initialization table
				this.levelUnitInitializationTable.Add( 
					new UnitInitializationData( unitName ,
												prefabTemplateName ,
												unitDataTemplateName ,
												raceName ,
												sideName ,
												posAnchor ,
												orientation ,
												supplemental ) ) ;
				
				Debug_LevelUnitInitSize = this.levelUnitInitializationTable.Count ;
			}
			else if( true == ParseVictoryCondition( unitNode ) )
			{
				
			}
			else if( true == ParseLoseCondition( unitNode ) )
			{
				
			}			
			else if( true == XMLParseLevelUtility.ParseEnemyGeneration( unitNode , 
																   out unitName , 
																   out prefabTemplateName , 
																   out unitDataTemplateName ,
																   out raceName ,
																   out sideName ,				
																   out posAnchor , 
																   out orientation ,
																   out supplemental ,
																   out time ) ) 
			{

				this.levelEnemyGenerationTable.Add(
					new UnitGenerationData( unitName ,
											prefabTemplateName ,
											unitDataTemplateName ,
											raceName ,
											sideName ,					
											posAnchor ,
											orientation , 
											supplemental ,
											time ) ) ;
			}						
			else if( true == XMLParseLevelUtility.ParseAddScriptOnObject( unitNode ,
									ref objectPlugByScriptName ,
									ref scriptPlugIntoObjectName ) )
			{
				GameObject obj = GameObject.Find( objectPlugByScriptName ) ;
				if( null != obj )
				{					
					// Debug.Log( "obj=" + objectName + " AddComponent( scriptName ) "+scriptName ) ;
					obj.AddComponent( scriptPlugIntoObjectName ) ;
				}
			}					
			else if( true == ParseUsualEvent( unitNode ) )
			{
				
			}
			else if( true == XMLParseLevelUtility.ParseBackgroundMusic( unitNode ,
										ref audioPath ) )
			{
				GameObject backgroundMusicObject = GlobalSingleton.GetBackgroundMusicObject() ;
				if( null != backgroundMusicObject )
				{
					backgroundMusicObject.audio.clip = ResourceLoad.LoadAudio( audioPath ) ;
					backgroundMusicObject.audio.Play() ;					
				}
			}
			else if( true == XMLParseLevelUtility.ParseStaticObject( unitNode,
										ref unitName ,
										ref prefabTemplateName , 
										ref position , 
										ref orientation ) )
			{
				
				GameObject obj = PrefabInstantiate.CreateByInit( prefabTemplateName , 
										unitName , 
										position , 
										orientation ) ;
				if( null != obj )
				{
					// confirm init position and orientation
					UnitInitialization unitInitScript = obj.GetComponent<UnitInitialization>() ;
					if( null == unitInitScript )
					{
#if DEBUG						
						Debug.Log( "GenerateUnit() : null == unitInitScript obj=" + obj.name ) ;
#endif						
					}
					else
					{
						unitInitScript.UnitInitializationPoint = position ;
						unitInitScript.UnitInitializationOrientation = orientation ;
					}					
					// Debug.Log( "obj.name" + obj.name ) ;
				}
			}
			else if( true == XMLParseLevelUtility.ParseConversationSet( unitNode,
																		out conversationSet ) )
			{
				// Debug.Log( "true == XMLParseLevelUtility.ParseConversationSet" ) ;
				manager.m_ConversationSets.Add( conversationSet.m_Key , conversationSet ) ;
			}
		}
		
	}
	
	/* Generate all units in this level 依照讀入的資料建立本關單位 */
	public void GenerateAllUnitsInThisLevel()
	{
		// Get UnitInitializationData of the unit
		for( int i = 0 ; i < this.levelUnitInitializationTable.Count ; ++i )
		{
			UnitInitializationData data = this.levelUnitInitializationTable[ i ] ;
			
			GenerateUnit( data.unitName , 
						  data.prefabName ,
						  data.unitDataTemplateName ,
						  data.raceName ,
						  data.sideName ,				
						  data.initPosition.GetPosition() , 
						  data.initOrientation ,
						  data.supplementalVec ) ;
			
			// 靜態關卡檔產生出來的敵人也要算入 勝負判定的表格 
			// 未來勝負判定模組化時再行調整
			EnemyGenerator enemyGen = GlobalSingleton.GetEnemyGeneratorComponent() ;
			if( null != enemyGen )
			{
				if( -1 != data.unitName.IndexOf( "Enemy_" ) )
				{
					// Debug.Log( "GenerateAllUnitsInThisLevel() enemyGen.m_EnemyGeneratedTable.Add=" + data.unitName ) ;
					enemyGen.m_EnemyGeneratedTable.Add( data.unitName ) ;
				}
			}
		}
	}
	
	public void CheckCustomMainCharacter()
	{
		if( false == GlobalSingleton.m_CustomActive ) 
			return ;
		UnitInitializationData data = new UnitInitializationData() ;
		data.unitName = ConstName.MainCharacterObjectName ;
		data.prefabName = GlobalSingleton.m_CustomPrefabName ;
		data.unitDataTemplateName = GlobalSingleton.m_CustomUnitDataName ;
		data.raceName = "Federation" ;
		data.sideName = "Ally" ;
		data.initPosition.Setup( "MainCharacterStartPosition" ) ;
		
		GenerateUnit( data.unitName , 
					  data.prefabName ,
					  data.unitDataTemplateName ,
					  data.raceName ,
					  data.sideName ,				
					  data.initPosition.GetPosition() , 
					  data.initOrientation ,
					  data.supplementalVec ) ;

	}
	
	// 產生勝負條件
	public void GenerateAllVictoryConditionInThisLevel()
	{
		VictoryEventManager victoryEvent = GlobalSingleton.GetVictoryEventManager() ;
		if( null == victoryEvent )
			return ;
		victoryEvent.m_VictoryConditions.Clear() ;
		List<Condition>.Enumerator eCondition = levelVictoryConditonTable.GetEnumerator() ;
		while( eCondition.MoveNext() )
		{
			// Debug.Log( "victoryEvent.m_VictoryConditions.Add" ) ;
			victoryEvent.m_VictoryConditions.Add( eCondition.Current ) ;
		}
		levelVictoryConditonTable.Clear() ;
		
		victoryEvent.m_LoseConditions.Clear() ;
		eCondition = levelLoseConditonTable.GetEnumerator() ;
		while( eCondition.MoveNext() )
		{
			// Debug.Log( "victoryEvent.m_LoseConditions.Add" ) ;
			victoryEvent.m_LoseConditions.Add( eCondition.Current ) ;
		}
		levelLoseConditonTable.Clear() ;
		
		
	}
	
	// 產生單位產生事件
	public void GenerateEnemyGenerationInThisLevel()
	{
		EnemyGenerator enemyGenerator = GlobalSingleton.GetEnemyGeneratorComponent() ;
		if( null == enemyGenerator )
			return ;
		List<UnitGenerationData>.Enumerator eEnemyGenItem = levelEnemyGenerationTable.GetEnumerator() ;
		while( eEnemyGenItem.MoveNext() )
		{
			enemyGenerator.AddUnitGeneration( eEnemyGenItem.Current ) ;			
		}		
	}
	
	// 產生一般事件
	void GenerateUsualEvnetInThisLevel()
	{
		UsualEventManager usualEventManager = GlobalSingleton.GetUsualEventManagerComponent() ;
		if( null == usualEventManager )
		{
			return ;
		}
		List<UsualEvent>.Enumerator e = levelEventTable.GetEnumerator() ;
		while( e.MoveNext() )
		{
			usualEventManager.m_EventList.Add( e.Current ) ;
		}
	}
	
	/* 
	 * @brief Create MainCharacter UnitData GUI by calling GUIUpdate after all unit been created.
	 * 
	 */
	public void CreateMainCharacterUnitDataGUI()
	{
		GUIUpdate guiUpdate = GlobalSingleton.GetGUIUpdateComponent() ;
		if( null != guiUpdate )
			guiUpdate.CreateMainCharacterUnitDataGUI() ;
	}
	
	// load component param table
	private void LoadComponentParamTable()
	{
		XmlDocument doc = new XmlDocument() ;		
		if( false == LoadDataToXML.LoadToXML( ref doc , this.componentParamTableFilepath ) )
			return ;
		
		XmlNode root = doc.FirstChild ;
		if( "ComponentParamTable" != root.Name )
		{
			return ;
		}
		
		if( false == root.HasChildNodes )
		{
			return ;
		}
		
		for( int i = 0 ; i < root.ChildNodes.Count ; ++i )
		{
			XmlNode unitNode = root.ChildNodes[ i ] ;
			if( "ComponentParam" == unitNode.Name )
			{
				ComponentParam componentParamResult ;
				if( true == XMLParseComponentParam.Parse( unitNode , 
														  out componentParamResult ) )
				{
					/*
					Debug.Log( componentParamResult.m_ComponentName ) ;
					Debug.Log( componentParamResult.GUIDisplayName ) ;
					Debug.Log( componentParamResult.GUIRect ) ;
					//*/
					string Key = componentParamResult.m_ComponentName ;
					if( true == this.componentParamTable.ContainsKey( Key ) )
					{
						Debug.Log( "LoadComponentParamTable() already contain=" + Key ) ;
					}
					else
					{
						this.componentParamTable.Add( Key ,
													  new ComponentParam( componentParamResult ) ) ;
					}
				}
			}
		}
		
#if DEBUG
		Debug_componentParamTable_valueVec = new ComponentParam[ this.componentParamTable.Count ] ;
		
		this.componentParamTable.Values.CopyTo( Debug_componentParamTable_valueVec , 0 ) ;
#endif		
		
	}
	
	// load weapon param table
	private void LoadWeaponParamTable()
	{
		XmlDocument doc = new XmlDocument() ;		
		if( false == LoadDataToXML.LoadToXML( ref doc , this.weaponParamTableFilepath ) )
			return ;		
		
		XmlNode root = doc.FirstChild ;
		if( "WeaponParamTable" != root.Name )
		{
			return ;
		}
		
		if( false == root.HasChildNodes )
		{
			return ;
		}
		
		for( int i = 0 ; i < root.ChildNodes.Count ; ++i )
		{
			XmlNode unitNode = root.ChildNodes[ i ] ;
			if( "WeaponParam" == unitNode.Name )
			{
				WeaponParam weaponParamResult ;
				if( true == XMLParseWeaponParam.Parse( unitNode , 
													   out weaponParamResult ) )
				{
					/*
					Debug.Log( weaponParamResult.m_ComponentName ) ;
					//*/
					string Key = weaponParamResult.m_ComponentName ;
					if( true == this.weaponParamTable.ContainsKey( Key ) )
					{
						Debug.Log( "LoadWeaponParamTable() already contain=" + Key ) ;
					}
					else
					{
						this.weaponParamTable.Add( Key ,
												   new WeaponParam( weaponParamResult ) ) ;
					}
				}
			}
		}
#if DEBUG
		Debug_weaponParamTable_valueVec = new WeaponParam[ this.weaponParamTable.Count ] ;
		this.weaponParamTable.Values.CopyTo( Debug_weaponParamTable_valueVec , 0 ) ;
#endif 		
	
	}


}
