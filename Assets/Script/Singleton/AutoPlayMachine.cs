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
@file AutoPlayMachine.cs
@author NDark
 
自動播放

# 目前還未完成
# 掛在 GlobalSingleton 下
# 預設是關閉
# 請參考 [[系統記錄及自動播放]]
# 會讀取系統紀錄的文件 SystemLogKeep.txt
# 依照系統紀錄來檢查時間同時觸發內容

@date 20121219 file started .
@date 20121219 by NDark 
. add code SystemLogManager.SysLogType.ClickOnUnit() at ApplyLog()
. add code of SystemLogManager.SysLogType.FireWeapon() at ApplyLog()
@date 20121220 . add code of SystemLogManager.SysLogType.Damage() at ApplyLog()
@date 20121227 by NDark . adjust SystemLogManager.SysLogType.Damage at ApplyLog()
@date 20130113 by NDark . comment.
@date 20130126 by NDark . fix SystemLogManager.SysLogType.Damage at ApplyLog()

*/
using UnityEngine;
using System.IO;

public class AutoPlayMachine : MonoBehaviour 
{
	public bool m_Active = false ;
	
	private StreamReader m_SR = null ;	
	private int m_Index = 0 ;
	// Use this for initialization
	void Start () 
	{
		if( true == m_Active )
		{
			SystemLogManager.m_Active = false ;
			m_SR = new StreamReader( "SystemLogKeep.txt" ) ;
			if( null == m_SR )
			{
				Debug.Log( "AutoPlayMachine::Start() null == m_SR" ) ;
			}
			else
			{
				while( false == m_SR.EndOfStream )
				{
					string StrRead = m_SR.ReadLine() ;
					SystemLogManager.m_Logs.Add( StrRead ) ;
					
				}
				m_Index = 0 ;
				m_SR.Close() ;
			}
			// show now is play
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( true == m_Active )
		{
			string [] strVec = null ;
			string [] strVecNext = null ;
			float timeNow = Time.timeSinceLevelLoad ;
			if( m_Index < SystemLogManager.m_Logs.Count )
			{
				for( int i = m_Index ; i < SystemLogManager.m_Logs.Count ; ++i )
				{
					string NextLog = "" ;
					string Log = SystemLogManager.m_Logs[ i ] ;
					if( i+1 < SystemLogManager.m_Logs.Count )
						NextLog = SystemLogManager.m_Logs[ i+1 ] ;
					
					strVec = Log.Split( ':' ) ;
					strVecNext = NextLog.Split( ':' ) ;
					if( strVec.Length >= 3 )
					{
						// strVec[ 0 ] : time
						// strVec[ 1 ] : type
						// strVec[ 2 ] : content
						float timeLog = 0 ;
						float.TryParse( strVec[ 0 ] , out timeLog ) ;
						float timeNextLog = 0 ;
						if( strVecNext.Length >= 3 )
						{
							float.TryParse( strVecNext[ 0 ] , out timeNextLog ) ;
						}
						if( timeLog <= timeNow && timeNextLog >= timeNow )
						{
							ApplyLog( strVec ) ;
							m_Index = i+1 ;// 強制到下一筆
							continue ;// 避免有兩個同時被夾住,一次處理掉
						}
						else if( i == SystemLogManager.m_Logs.Count - 1 &&
							timeLog <= timeNow )
						{
							// 最後一筆
							ApplyLog( strVec ) ;
							m_Index = i+1 ; // 強制離開,不會再進來
							break ;
						}
						else if( timeLog > timeNow )
						{
							// 走過頭了
							m_Index = i ;
							break ;
						}
					}
				}
			}
			
		}	
	}
	
	void ApplyLog( string [] _strVec )
	{
		// _strVec[ 0 ] : time
		// _strVec[ 1 ] : type
		// _strVec[ 2 ] : content
		if( _strVec[ 1 ] == SystemLogManager.SysLogType.Control.ToString() )
		{
			string valueStr = _strVec[ 3 ] ;
			float controlValue = 0 ;
			float.TryParse( valueStr , out controlValue ) ;
			
			if( _strVec[ 2 ] == "UpDown" )
			{
				GameObject mainChar = GlobalSingleton.GetMainCharacterObj() ;
				UnitData unitData = mainChar.GetComponent<UnitData>() ;
				MainCharacterController control = mainChar.GetComponent<MainCharacterController>() ;
				control.CheckSpeed( unitData , controlValue ) ;
				// Debug.Log( "UpDown=" + controlValue ) ;
			}
			else if( _strVec[ 2 ] == "LeftRight" )
			{
				GameObject mainChar = GlobalSingleton.GetMainCharacterObj() ;
				UnitData unitData = mainChar.GetComponent<UnitData>() ;
				MainCharacterController control = mainChar.GetComponent<MainCharacterController>() ;
				
				control.CheckTurn( unitData , controlValue ) ;
				// Debug.Log( "LeftRight=" + controlValue ) ;
			}
		}
		else if( _strVec[ 1 ] == SystemLogManager.SysLogType.ClickOnUnit.ToString() )
		{
			// _strVec[ 2 ] : Name
			// _strVec[ 3 ] : true / false 
			GameObject mainChar = GlobalSingleton.GetMainCharacterObj() ;
			if( null != mainChar )
			{
				UnitSelectionSystem selectSys = mainChar.GetComponent<UnitSelectionSystem>() ;
				if( null != selectSys )
				{
					selectSys.ClickOnUnit( _strVec[ 2 ] ) ;
					// Debug.Log( "selectSys.ActiveSelectInformation" + boolean ) ;
				}
			}
		}
		else if( _strVec[ 1 ] == SystemLogManager.SysLogType.FireWeapon.ToString() )
		{
			// _strVec[ 2 ] : SrcUnitName
			// _strVec[ 3 ] : WeaponComponent
			// _strVec[ 4 ] : TargetUnitName
			// _strVec[ 5 ] : TargetComponent
			// Debug.Log( "SystemLogManager.SysLogType.FireWeapon" ) ;
			GameObject srcUnit = GameObject.Find( _strVec[ 2 ] ) ;
			if( null != srcUnit )
			{
				UnitWeaponSystem weaponSys = srcUnit.GetComponent<UnitWeaponSystem>() ;
				if( null != weaponSys )
				{
					weaponSys.ActiveWeapon( _strVec[ 3 ] , new NamedObject( _strVec[ 4 ] )  , _strVec[ 5 ] ) ;					
				}
			}
		}
		else if( _strVec[ 1 ] == SystemLogManager.SysLogType.Damage.ToString() )
		{
			/*
			SystemLogManager.AddLog( SystemLogManager.SysLogType.Damage , 
				_AttackerUnit + ":" + 
				_AttackerDisplayName + ":" + 
				_RealDamage + ":" + 
				_AbsortDamage + ":"+ 
				_DefenseUnit + ":" + 
				_ComponentName ) ;
			*/
			// _strVec[ 2 ] : SrcUnitRealName
			// _strVec[ 3 ] : SrcUnitDisplay
			// _strVec[ 4 ] : RealDamageStr
			// _strVec[ 5 ] : AbsortDamageStr
			// _strVec[ 6 ] : TargetUnitName
			// _strVec[ 7 ] : TargetComponent (option)
			Debug.Log( "SystemLogManager.SysLogType.Damage" ) ;
			GameObject targetUnit = GameObject.Find( _strVec[ 4 ] ) ;
			if( null != targetUnit )
			{
				UnitDamageSystem dmgSys = targetUnit.GetComponent<UnitDamageSystem>() ;
				if( null != dmgSys )
				{
					string hitComponentObjectName = "" ;
					float realDmgValue = 0 ;
					float.TryParse( _strVec[ 4 ] , out realDmgValue ) ;
					float absortDmgValue = 0 ;
					float.TryParse( _strVec[ 5 ] , out absortDmgValue ) ;					
					hitComponentObjectName = _strVec[ 7 ] ;
					dmgSys.CauseDamageValueIn( _strVec[ 2 ] ,
												_strVec[ 3 ] ,
												_strVec[ 6 ] ,
												realDmgValue , 
												absortDmgValue , 
												hitComponentObjectName ) ;
				}
			}
		}				
	}
	
}
