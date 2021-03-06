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
@file ConstName.cs
@brief 靜態名稱定義 
@author NDark

用來做字串相關的定義及產生

@date 20121110 by NDark 
. add class member 
UnitDataComponentPulseEngineRatio
UnitDataGUIComponentHP
UnitDataGUIComponentReload
UnitDataGUIComponentLabel
...
@date 20121123 by NDark . add class member UnitDataComponentUnitIntagraty
@date 20121202 by NDark . add class member Scene_Recruitment
@date 20121204 by NDark 
. add class method SplitComponentObjectNameToUnitName()
. allow SplitComponentObjectNameToUnitName() to return unit name.
@date 20121205 by NDark 
. add class member GUIUnitDataSelection_UnActive
. add class method SplitComponentObjectNameToComponentName()
@date 20121207 by NDark . add class member UnitDataComponentImpulseEnginePrefix
@date 20121211 by NDark . remove class method CreateComponentEffectObjectTemplateName()
@date 20121213 by NDark 
. add class method CreateWeaponRangeObjectName()
. add class method CreateWeaponRangeMeshName()
@date 20121218 by NDark
. add class method CreateEnemyGenerateObjectName()
. add class method GenerateIterateString()
@date 20121219 by NDark 
. add class method GetSplitVec()
. add class member GUI_UnitIntagratyTemplateName
. add class member UnitDataGUIBackgroundPrefabName
. remove class method CreateGUIBackgroundTexturePath()
@date 20121220 by NDark . add class member BackgroundObjectName
@date 20121225 by NDark 
. add class method FindWeaponKeyword()
. add class method FindWeaponKeyword()
. add class member GUI_BattleScore_TextRow_
. add class method CreateBattleScore_TextRowName()
@date 20121226 by NDark 
. add class member GUIMouseCursor_SelectionTexturePath
. add class member Scene_Acknowledge
@date 20121227 by NDark
. add class member UnitDataComponentShieldPrefix
. add class member UnitDataComponentAuxiliaryEnergy
@date 20121228 by NDark . fix an error of not iterate the class member iterator at CreateEnemyGenerateObjectName()
@date 20121230 by NDark 
. add class member GUIControlPanelMultiAttack_UnActiveName
. add class member GUIControlPanelMultiAttack_ActiveName
@date 20121231 by NDark 
. add class member GUIMouseCursor_MultiAttackTexturePath
. add class member UnitDataComponentWeaponTrackorBeamPrefix
@date 20130103 by NDark 
. add class method GetSplitVec()
. rename class method GetSplitVec() to GetSplitVecConetent()
. add class method GetGUI_ConversationTextObjectName()
@date 20130112 by NDark
. add class member GUI_AcknowledgeScene_Skip
@date 20130121 by NDark . remove class method GetGUI_ConversationTextObjectName() 
@date 20140201 by NDark
. add class member GUIControlPanelNone_ActiveName.
. add class member GUIControlPanelNone_UnActiveName.

*/
using UnityEngine;

public static class ConstName 
{
	private static int iterator = 0 ;// 用來產生新物件讓物件名稱不重複的的序號
	
	// singleton
	public static string GlobalSingletonObjectName = "GlobalSingleton" ;
	public static string BackgroundObjectName = "BackgroundObj" ;
	public static string MainCharacterObjectName = "MainCharacter" ;
	public static string MainCharacterObjectTag = "Player" ;

	public static string MiniMapParentObjectName = "MiniMapParent" ;
	
	// unit component
	public static string UnitDataComponentSensor = "Sensor" ;
	public static string UnitDataComponentUnitIntagraty = "UnitIntagraty" ;
	public static string UnitDataComponentImpulseEnginePrefix = "ImpulseEngine_" ;
	public static string UnitDataComponentShieldPrefix = "Shield_" ;
	public static string UnitDataComponentWeaponTrackorBeamPrefix = "Weapon_TrackorBeam" ;
	public static string UnitDataComponentImpulseEngineRatio = "ImpulseEngineRatio" ;
	public static string UnitDataComponentImpulseEngineSpeed = "ImpulseEngineSpeed" ;
	public static string UnitDataComponentImpulseEngineAngularRatio = "ImpulseEngineAngularRatio" ;
	public static string UnitDataComponentImpulseEngineAngularSpeed = "ImpulseEngineAngularSpeed" ;
	public static string UnitDataComponentAuxiliaryEnergy = "AuxiliaryEnergy" ;
	
	// gui
	public static string GUI_UnitIntagratyTemplateName = "Template_GUI_Unit_UnitIntagraty" ;
	public static string UnitDataGUIComponentHP = "HP" ;
	public static string UnitDataGUIComponentReload = "Reload" ;
	public static string UnitDataGUIComponentLabel = "Label" ;
	
	public static string UnitDataGUIBackgroundPrefabName = "Template_GUI_SelectUnitDataBackground" ;
	public static string UnitDataGUIMainCharacterBackgroundPrefabName = "Template_GUI_MainCharacterUnitDataBackground" ;
	
	public static string GUIDamageNumberEffectTemplateName = "Template_GUI_DamageNumber" ;

	public static string GUIControlPanelNone_ActiveName = "ControlPanelNone_Active" ;
	public static string GUIControlPanelNone_UnActiveName = "ControlPanelNone_UnActive" ;	
	public static string GUIControlPanelPhaser_ActiveName = "ControlPanelPhaser_Active" ;
	public static string GUIControlPanelPhaser_UnActiveName = "ControlPanelPhaser_UnActive" ;
	public static string GUIControlPanelTorpedo_UnActiveName = "ControlPanelTorpedo_UnActive" ;
	public static string GUIControlPanelTorpedo_ActiveName = "ControlPanelTorpedo_Active" ;
	public static string GUIControlPanelTrakorBeam_UnActiveName = "ControlPanelTrakorBeam_UnActive" ;
	public static string GUIControlPanelTrakorBeam_ActiveName = "ControlPanelTrakorBeam_Active" ;
	public static string GUIControlPanelMultiAttack_UnActiveName = "ControlPanelMultiAttack_UnActive" ;
	public static string GUIControlPanelMultiAttack_ActiveName = "ControlPanelMultiAttack_Active" ;
	
	public static string GUIMouseCursor_SelectionTexturePath = "GUI_MouseCursor_Selection" ;
	public static string GUIMouseCursor_PhaserTexturePath = "GUI_MouseCursor_Phaser" ;
	public static string GUIMouseCursor_TorpedoTexturePath = "GUI_MouseCursor_Torpedo" ;
	public static string GUIMouseCursor_TrakorBeamTexturePath = "GUI_MouseCursor_TrakorBeam" ;
	public static string GUIMouseCursor_MultiAttackTexturePath = "GUI_MouseCursor_MultiAttack" ;

	public static string GUIUnitDataSelection_UnActive = "GUI_UnitDataSelection_UnActive" ;
	
	public static string GUI_BattleScore_TextRow_ = "GUI_BattleScore_TextRow_" ;
	public static string GUI_BattleScore_TextRow_DestroyNum = "GUI_BattleScore_TextRow_DestroyNum" ;
	public static string GUI_BattleScore_TextRow_DamageSuffer = "GUI_BattleScore_TextRow_DamageSuffer" ;
	public static string GUI_BattleScore_TextRow_ElapsedSec = "GUI_BattleScore_TextRow_ElapsedSec" ;	
	
	public static string GUI_AcknowledgeScene_Skip = "GUI_Skip" ;	
	
	// minimap
	public static string MinimapUndefinedUnitTeamplateName = "Template_Minimap_UndefinedUnit" ;

	// effect
	public static string EffectSparksTemplateName = "Template_Effect_PhaserSparks" ;
	
	// load scene
	public static string Scene_Acknowledge = "Scene_Acknowledge" ;
	public static string Scene_SelectLevel = "Scene_SelectLevel" ;
	public static string Scene_Recruitment = "Scene_Recruitment" ;
	
	// 產生序號字串
	public static string GenerateIterateString()
	{
		string ret = iterator.ToString() ;
		++iterator ;
		return ret ;
	}
	
	// 產生特效物件名稱
	public static string CreateEffectName( string _UnitName , string _TemplateName , bool _iterator )
	{
		string ret = "(CreateEffectName)" ;
		if( 0 != _UnitName.Length &&
			0 != _TemplateName.Length )
		{
			ret = _UnitName + ":" + _TemplateName ;
			if( true == _iterator )
			{
				ret += GenerateIterateString() ;				
			}
		}	
		return ret ;
	}
	
	// 產生GUI傷害文字物件名稱
	public static string CreateGUIDamageNumberEffectObjectName( string _UnitName )
	{
		string ret = "(CreateGUIDamageNumberEffectObjectName)" ;
		if( 0 != _UnitName.Length )
		{
			ret = _UnitName + ":" + GUIDamageNumberEffectTemplateName ;
		}	
		return ret ;
	}
	
	// 產生部件樣板名稱
	public static string CreateComponent3DObjectTemplateName( string _ComponentName )
	{
		string ret = "(CreateComponent3DObjectTemplateName)" ;
		if( 0 != _ComponentName.Length )
		{
			ret = "Template_Unit_" + _ComponentName ;
		}	
		return ret ;
	}
	
	// 產生部件名稱
	public static string CreateComponent3DObjectName( string _UnitName , string _ComponentName )
	{
		string ret = "(CreateComponent3DObjectName)" ;
		if( 0 != _UnitName.Length &&
			0 != _ComponentName.Length )
		{
			ret = _UnitName + ":" + _ComponentName ;
		}	
		return ret ;
	}
	
	// 產生部件特效物件名稱
	public static string CreateComponentEffectObjectName( string _UnitName , string _ComponentName )
	{
		string ret = "(CreateComponentEffectObjectName)" ;
		string component3DObjectName = CreateComponent3DObjectName( _UnitName , _ComponentName ) ;
		if( ret != component3DObjectName )
		{
			ret = component3DObjectName + "_Effect" ;
		}
		return ret ;
	}
	
	
	// 產生GUI背景的物件名稱
	public static string CreateGUIBackgroundObjName( string _UnitName ) 
	{
		string ret = "(undefine)" ;
		if( 0 != _UnitName.Length )
		{
			ret = "GUI_" + _UnitName + ":GUI_UnitDataBackground" ;
		}	
		return ret ;
	}
	
	
	// 產生GUI部件的名稱
	public static string CreateGUIComponentObjectName( string _UnitName , string _ComponentName ) 
	{
		string ret = "(undefine)" ;
		if( 0 != _UnitName.Length &&
			0 != _ComponentName.Length )
		{
			ret = "GUI_" + _UnitName + ":" + _ComponentName ;
		}	
		return ret ;		
		
	}
	
	// 使用字串來取得GUI功能面板的功能
	public static SelectFunctionMode FindControlPanelFunction( string _ControlPanelObjectName )
	{
		SelectFunctionMode ret = SelectFunctionMode.None ;
		string funcName = _ControlPanelObjectName.Replace( "ControlPanel" , "" ) ;
		int index = funcName.IndexOf( "_" ) ;
		funcName = funcName.Substring( 0 , index ) ;
		if( -1 != funcName.IndexOf( "Phaser" ) )
			ret = SelectFunctionMode.WeaponPhaser ;
		else if( -1 != funcName.IndexOf( "Torpedo" ) )
			ret = SelectFunctionMode.WeaponTorpedo ;
		else if( -1 != funcName.IndexOf( "TrakorBeam" ) )
			ret = SelectFunctionMode.FunctionTrakorBeam ;
		else if( -1 != funcName.IndexOf( "MultiAttack" ) )
			ret = SelectFunctionMode.SpecialModeMultipleAttack ;

		return ret ;
	}
	// 使用字串來取得GUI功能面板的功能
	public static string FindWeaponKeyword( WeaponType _Type )
	{
		string ret = "" ;
		switch( _Type )
		{
		case WeaponType.Phaser :
			ret = "Weapon_Phaser" ;
			break ;
		case WeaponType.Torpedo :
			ret = "Weapon_PhotonTorpedo" ;
			break ;
		case WeaponType.Cannon :
			ret = "Weapon_Cannon" ;
			break ;			
		case WeaponType.TrakorBeam :
			ret = "Weapon_TrackorBeam" ;
			break ;
		}
		
		return ret ;
	}
	// 使用字串來取得GUI功能面板的功能
	public static string FindWeaponKeyword( SelectFunctionMode _Mode )
	{
		string ret = "" ;
		switch( _Mode )
		{
		case SelectFunctionMode.WeaponPhaser :
			ret = "Weapon_Phaser" ;
			break ;
		case SelectFunctionMode.WeaponTorpedo :
			ret = "Weapon_PhotonTorpedo" ;
			break ;
		case SelectFunctionMode.FunctionTrakorBeam :
			ret = "Weapon_TrackorBeam" ;
			break ;
		}
		
		return ret ;
	}		
	
	// 產生MiniMap的物件名稱
	public static string CreateMiniMapObjectName( string _OrgUnitName )
	{
		return "MiniMap_" + _OrgUnitName ;
	}
	
	// 產生MiniMap的貼圖名稱
	public static string CreateMiniMapTextureResourcePath( string _RaceName )
	{
		return "MiniMap/Minimap_Unit_" + _RaceName + "Texture" ;
	}
	
	// 產生武器範圍的物件名稱
	public static string CreateWeaponRangeObjectName( string _Component3DObjectName )
	{
		return _Component3DObjectName + "_Range" ;
	}
	
	// 產生武器範圍的Mesh名稱
	public static string CreateWeaponRangeMeshName( string _Component3DObjectName )
	{
		return _Component3DObjectName + "_Mesh" ;
	}	
	
	// 產生 單位新增的名稱
	public static string CreateEnemyGenerateObjectName( string _RaceName , string _PrefabName )
	{
		string iterStr = GenerateIterateString() ;
		return "Unit_" + 
			   _RaceName + 
			   _PrefabName + 
			   iterStr ;
		
	}
	
	// 將 防護罩物件名稱切成 部件名稱
	public static string[] GetSplitVec( string _String , char _Seperator = ':' )
	{
		string [] Ret = _String.Split( _Seperator ) ;
		return Ret ;
	}	
	public static string GetSplitVecConetent( string _String , int _Index , char _Seperator = ':' )
	{
		string ret = "" ;
		string [] strVec = _String.Split( _Seperator ) ;
		if( strVec.Length >= _Index+1 )
			ret = strVec[ _Index ] ;
		return ret ;
	}
	
	// 切割部件物件為 單位及部件
	public static string SplitComponentObjectNameToUnitName( string _ComponentObjectName )
	{
		string ret = "" ;
		string [] strVec = _ComponentObjectName.Split( ':' ) ;
		if( strVec.Length >= 1 )
			ret = strVec[ 0 ] ;
		return ret ;
	}
	
	// 切割部件物件為 單位及部件
	public static string SplitComponentObjectNameToComponentName( string _ComponentObjectName )
	{
		string ret = "" ;
		string [] strVec = _ComponentObjectName.Split( ':' ) ;
		if( strVec.Length >= 2 )
			ret = strVec[ 1 ] ;
		return ret ;
	}	
	
	public static string CreateBattleScore_TextRowName( ScoreType _Type )
	{
		string ret = "" ;
		ret = GUI_BattleScore_TextRow_ + _Type.ToString() ;
		return ret ;
	}
	
}
