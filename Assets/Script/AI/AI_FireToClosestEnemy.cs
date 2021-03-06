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
@file AI_FireToClosestEnemy.cs
@author NDark

# 攻擊型AI的基礎類別
# 狀態
# 參數 透過 RetrieveData() 取得
## 開火後停滯時間 WaitForReloadSec : 在補充資料中設定,通常要設定一個大於填充時間的緩衝時間,避免AI單位一次把所有武器發射出去.但又不可以設定太長,讓AI單位老是使用同一個武器
## 目前所有武器清單 m_WeaponKeywords : 可在補充資料中設定,讓AI單位使用其中一種武器.目前最多可以填10項.如果不填,AI單位會以不指定的方式請求武器系統.
使用時就是自清單中隨機取出一筆資料
# 變數 
## 最小武器距離 m_MaximumWeaponRange 向武器系統請求而來的最小武器距離
武器使用最小距離的原因
如果紀錄最大距離，那麼AI就永遠只知道進入到最長的武器攻擊距離，這會導致較短的武器都不能使用。
因此會分兩種，
1是無指定的，就必須取得最短攻擊距離。
2是有指定的，那麼應該在每次攻擊決定武器後就取得一次攻擊距離。
# 運作
未啟動->取得資料->尋找目標->移動目標->攻擊目標->填充->尋找目標
## 尋找目標 FindATarget()
向感測系統請求最接近的非我族類單位
## 檢查武器並移動 CheckWeaponAndMoveToTarget()
檢查目前是否能夠攻擊，如果可以就切換到攻擊狀態
如果不行就往目標移動
## 發射武器 FireWeapon()
## 檢查填充時間然後回到某個狀態 CheckReloadAndBackTo()

@date 20121206 by NDark . file started.
@date 20121210 by NDark . refactor.
@date 20121218 by NDark 
. refactor.
. add class method SetState()
@date 20130102 by NDark . change argument type of class method FireWeapon()
@date 20130103 by NDark . change argument type of class method FireWeapon()
@date 20130105 by NDark 
. add class member m_TargetSide
. add class method IsValidTarget()
. add class method RetrieveDataTargetSideKeyword()
@date 20130106 by NDark 
. add class method ChangeTarget()
. add class member m_ReFindTargetTimer
@date 20130107 by NDark 
. add code of m_ReFindTargetTimer
. adjust pursue distance smaller than weapon range at CheckWeaponAndMoveToTarget()
@date 20130109 by NDark
. change type of m_ReFindTargetTimer to protected.
@date 20130119 by NDark . add SetState() at ChangeTarget().
@date 20130131 by NDark 
. remove class member m_TargetUnitData
. change type of argument of class method ChangeTarget()
. remove class method IsValidTarget()
. add class method IsAnyObstacleInfrontOfUs() from AI_Fire_ToClosetEnemy2.cs
. add class method CalculateSubTarget() from AI_Fire_ToClosetEnemy2.cs
@date 20130204 by NDark
. remove class member m_TargetSide
. remove class member m_FindATargetTimer
. remove class method RetrieveDataTargetSideKeyword()
. refine code.

*/
using UnityEngine;
using System.Collections.Generic;

public class AI_FireToClosestEnemy : AI_Base 
{
	// AI_FireToTarget 的狀態
	[System.Serializable]
	public enum AI_Fire_State
	{
		UnActive = 0 ,			// 未啟動
		CheckUnitData ,			// 檢查資料是否真確
		FindATarget ,			// 尋找感測器中的可能
		MoveToTarget ,			// 嘗試取得戰略位置
		MoveToSubTarget ,		// 移動到其他目標
		StopAndFireToSubTarget ,	// 攻擊其他目標
		StopAndFireToTarget ,	// 發動攻擊
		WaitForReload ,			// 等待武器填充
		End ,
	}
	
	protected UnitObject m_TargetNow = new UnitObject() ;	// 目標,從參數中取得
	
	protected CountDownTrigger m_ReFindTargetTimer = new CountDownTrigger( 10.0f ) ;// 強制重新找尋目標的倒數器
	
	protected float m_MaximumWeaponRange = 0.0f ; // 檢察靠近的武器距離
		
	// parameter
	protected float m_WaitForReloadSec = 0.0f ;	 // "WaitForReloadSec"	
	int m_WeaponKeywordMaxNum = 10 ;			// 最大能指定武器關鍵字
	protected List<string> m_WeaponKeywords = new List<string>() ;	 // "WeaponKeyword{0}"	武器關鍵字
	
	
	protected UnitObject m_SubTarget = new UnitObject() ;	// sub target unit 子目標 某些AI在主要目標之外還要暫存子目標
	protected Vector3 m_SubTargetPosition = Vector3.zero ;	// sub target position 子目標位置 某些AI在主要目標之外還要移動到其他目標
	protected float m_EvasionRotateAngle = 35.0f ; 			// 閃避的旋轉角度，轉角越大越會離開目前航道
	protected float m_BlockAngle = 30.0f ;					// 被阻擋的檢查角度 
	protected float m_BlockRangeSquare = 900.0f ;			// 被阻擋時的檢查距離平方
	protected float m_ReachDistanceSquare = 20.0f ;			// 抵達第二目標位置的檢查距離
	
	public string DEBUG_StateNow = "" ;// 用來取出state的除錯工具
	
	// 改變主要目標
	public void ChangeTarget( UnitObject _Obj )
	{
		m_TargetNow = new UnitObject( _Obj ) ;
		if( null != m_TargetNow.Obj )
		{			
			this.SetState( AI_Fire_State.MoveToTarget ) ;// 重新設定狀態
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		SetState( AI_Fire_State.UnActive ) ;
		m_ReFindTargetTimer.Rewind() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// check if the AI object is still alive or not
		UnitData unitData = null ;
		UnitWeaponSystem weaponSys = null ;
		if( false == CheckAbleRunAI( out unitData , out weaponSys ) )
			return ;
		
		// 檢查是否要重新尋找目標
		if( m_State.state > (int) AI_Fire_State.CheckUnitData
			&& true == m_ReFindTargetTimer.IsCountDownToZero() )
		{
			SetState( AI_Fire_State.FindATarget ) ;
			m_ReFindTargetTimer.Rewind() ;
			return ;
		}

		m_State.Update() ;
		switch( (AI_Fire_State) m_State.state )
		{
		case AI_Fire_State.UnActive :
			SetState( AI_Fire_State.CheckUnitData ) ;
			break ;	
			
		case AI_Fire_State.CheckUnitData :
			if( false == RetrieveData() )
			{
				Debug.Log( "false == RetrieveData()" + this.gameObject.name ) ;
			}
			SetState( AI_Fire_State.FindATarget ) ;
			break ;
			
		case AI_Fire_State.FindATarget :
			
			if( true == FindATarget( unitData ) )
			{
				this.SetState( AI_Fire_State.MoveToTarget ) ;
			}			
			break ;
			
		case AI_Fire_State.MoveToTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				SetState( AI_Fire_State.FindATarget ) ;
				break ;
			}
			
			CheckWeaponAndMoveToTarget( unitData , 
										weaponSys , 
										m_TargetNow.Obj.transform.position ) ;
			
			break ;
			

		case AI_Fire_State.StopAndFireToTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				SetState( AI_Fire_State.FindATarget ) ;
				break ;
			}
			
			if( true == m_State.IsFirstTime() )
			{
				unitData.AllStop() ;
			}			
			
			FireWeapon( weaponSys , m_TargetNow ) ;

			break ;				
			
			
		case AI_Fire_State.WaitForReload :
			CheckReloadAndBackTo( AI_Fire_State.FindATarget ) ;
			break ;
		}
	}
	
	// set m_State by AI_Fire_State
	public void SetState( AI_Fire_State _Set )
	{
		m_State.state = (int)_Set ;
		DEBUG_StateNow = _Set.ToString() ;
	}
	
	protected bool CheckAbleRunAI( out UnitData _UnitData ,
								   out UnitWeaponSystem _weaponSys )
	{
		return ( true == CheckUnitData( out _UnitData ) &&
				 true == CheckWeaponSys( out _weaponSys ) ) ;
	}
		
	protected override bool RetrieveData()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		UnitWeaponSystem weaponSys = this.gameObject.GetComponent<UnitWeaponSystem>() ;
		if( null != unitData && 
			null != weaponSys )
		{
			RetrieveDataWeaponKeyword( unitData ) ;
			RetrieveDataMaximumWeaponRange( weaponSys , ref m_WeaponKeywords ) ;
			
			RetrieveParam( unitData , "WaitForReloadSec" , ref m_WaitForReloadSec ) ;			
		}			
		return true ;
	}	
	
	// 取得武器關鍵字參數
	protected void RetrieveDataWeaponKeyword( UnitData _unitData )
	{
		m_WeaponKeywords.Clear() ;
		for( int i = 0 ; i < m_WeaponKeywordMaxNum ; ++i )
		{
			string label = string.Format( "WeaponKeyword{0}" , i ) ;
			string valueStr = "" ;
			if( false == RetrieveParam( _unitData , label , ref valueStr ) )
				return ;
			// Debug.Log( "RetrieveDataWeaponKeyword() " +label+"=" + valueStr ) ;
			m_WeaponKeywords.Add( valueStr ) ;
		}
	}
	
	// 取得武器距離參數
	protected void RetrieveDataMaximumWeaponRange( UnitWeaponSystem _WeaponSys , ref List<string> _weaponKeywords )
	{
		float minWeaponRange = 9999 ;
		if( _weaponKeywords.Count > 0 )
		{
			foreach( string keyword in _weaponKeywords )
			{
				float weaponRange = _WeaponSys.FindMaximumWeaponRange( keyword ) ;
				if( weaponRange < minWeaponRange )
					minWeaponRange = weaponRange ;
			}
			m_MaximumWeaponRange = minWeaponRange ;
		}
		else
		{
			m_MaximumWeaponRange = _WeaponSys.FindMaximumWeaponRange( "" ) ;
		}
		// Debug.Log( "RetrieveDataMaximumWeaponRange() " +m_MaximumWeaponRange ) ;
	}	
	
	// 從關鍵字清單中隨機取回一個關鍵字
	protected string RandomAWeaponKeyword()
	{
		string ret = "" ;
		if( m_WeaponKeywords.Count > 0 )
		{
			int index = Random.Range( 0 , m_WeaponKeywords.Count - 1 ) ;
			ret = m_WeaponKeywords[ index ] ;
		}
		// Debug.Log( "RandomAWeaponKeyword() " +ret ) ;
		return ret ;
	}
	
	// save result to m_TargetNow and m_TargetUnitData
	protected virtual bool FindATarget( UnitData _unitData )
	{
		bool found = false ;
		float minDistance = 999.9f ;
		UnitSensorSystem sensorSys = this.gameObject.GetComponent<UnitSensorSystem>() ;
		foreach( NamedObject unit in sensorSys.m_SensorUnitList )
		{
			if( null == unit.Obj )
				continue ;
			UnitData targetUnitData = unit.Obj.GetComponent<UnitData>() ;
			if( null == targetUnitData )
				continue ;
			Vector3 distVec = unit.Obj.transform.position - this.gameObject.transform.position ;
			
			// Debug.Log( "distVec.magnitude" + distVec.magnitude ) ;
			if( true == targetUnitData.IsAlive() &&
				_unitData.m_SideName != targetUnitData.m_SideName &&
				distVec.magnitude < minDistance 
				)
			{
				
				minDistance = distVec.magnitude ;
				
				m_TargetNow.Setup( unit.Name , unit.Obj );
				
				// Debug.Log( "m_TargetNow.Name" + m_TargetNow.Name ) ;
				found = true ;
			}
		}
		
		if( false == found )
		{
			m_TargetNow.Clear() ;
		}
		return found ;

	}
	
	
	// 檢查武器並移動
	protected virtual bool CheckWeaponAndMoveToTarget( UnitData _unitData , 
													   UnitWeaponSystem _weaponSys ,
													   Vector3 _TargetPosition )
	{
		string weaponComponent = "" ;
		Vector3 vecToTarget = Vector3.zero ;
		Vector3 norVecToTarget = Vector3.zero ;
		float angleOfTarget = 0.0f ;
		float dotOfUp = 0.0f ;
		string IMPLUSE_ENGINE_RATIO = ConstName.UnitDataComponentImpulseEngineRatio ;
		// Debug.Log( "CheckWeaponAndMoveToTarget()" ) ;
		
		// check fire phaser weapon is available
		weaponComponent = _weaponSys.FindAbleWeaponComponent( RandomAWeaponKeyword() , 
															  m_TargetNow.Name ) ;
		// Debug.Log( "CheckWeaponAndMoveToTarget() weaponComponent=" + weaponComponent ) ;
		if( 0 != weaponComponent.Length )
		{
			this.SetState( AI_Fire_State.StopAndFireToTarget ) ;			
			return true ;
		}
		
		// move to target
		if( true == MathmaticFunc.FindTargetRelation( this.gameObject , 
													 _TargetPosition , 
													 ref vecToTarget ,
													 ref norVecToTarget ,
													 ref angleOfTarget ,
													 ref dotOfUp ) )
		{
			if( true == _unitData.standardParameters.ContainsKey( IMPLUSE_ENGINE_RATIO ) )
			{
				// speed to maximum
				float sqrtMaximumWeaponrange = m_MaximumWeaponRange * m_MaximumWeaponRange * 0.81f ;
				if( vecToTarget.sqrMagnitude > sqrtMaximumWeaponrange )
					_unitData.standardParameters[ IMPLUSE_ENGINE_RATIO ].ToMax() ;
				else
					_unitData.standardParameters[ IMPLUSE_ENGINE_RATIO ].Clear() ;
			}
			
			// angular speed			
			_unitData.AngularRatioHeadTo( angleOfTarget ,
										  dotOfUp , 
										  0.1f ) ;
		}
		
		return false ;
	}
	
	// 發射武器
	protected virtual void FireWeapon( UnitWeaponSystem _weaponSys ,
							   		   NamedObject _TargetObject )
	{
		string weaponComponent = "" ;
		// check phaser
		weaponComponent = _weaponSys.FindAbleWeaponComponent( RandomAWeaponKeyword() , 
															  _TargetObject.Name ) ;
		if( 0 == weaponComponent.Length )
		{
			// no available weapon
			this.SetState( AI_Fire_State.FindATarget ) ;			
			return ;
		}
		
		// fire
		if( true == _weaponSys.ActiveWeapon( weaponComponent , 
											 _TargetObject ,
											 ConstName.UnitDataComponentUnitIntagraty ) )
		{
			this.SetState( AI_Fire_State.WaitForReload ) ;			
		}
		
	}
	
	// 檢查填充時間然後回到某個狀態
	protected virtual bool CheckReloadAndBackTo( AI_Fire_State _NewState )
	{
		if( m_State.ElapsedFromLast() > m_WaitForReloadSec )
		{
			this.SetState( _NewState ) ;			
			return true ;
		}
		return false ;
	}	

	// 判斷有無障礙物 
	protected virtual bool IsAnyObstacleInfrontOfUs( string _TargetName , 
										  			 ref Vector3 _SubTargetPosition )
	{
		bool ret = false ;
		
		Vector3 vecToTarget = Vector3.zero ;
		Vector3 norVecToTarget = Vector3.zero ;
		float angleOfTarget = 0.0f ;
		float dotOfUp = 0.0f ;

		UnitSensorSystem sensor = this.gameObject.GetComponent<UnitSensorSystem>() ;
		if( null != sensor )
		{
			foreach( NamedObject unit in sensor.m_SensorUnitList )
			{	
				if( unit.Name == _TargetName )
					continue ;
				
				if( false == MathmaticFunc.FindUnitRelation( this.gameObject , 
															 unit.Obj , 
															 ref vecToTarget ,
															 ref norVecToTarget ,
															 ref angleOfTarget ,
															 ref dotOfUp ) )
					continue ;
				
				if( vecToTarget.sqrMagnitude < m_BlockRangeSquare &&
					angleOfTarget < m_BlockAngle )
				{
					CalculateSubTarget( this.gameObject , 
										unit.Obj , 
										ref _SubTargetPosition ) ;
					// Debug.Log( "block in the way" + unit.Name ) ;
					// Debug.Log( "_SubTargetPosition" + _SubTargetPosition ) ;
					ret = true ;										
					break ;
				}				
			}
		}
		return ret ;
	}
	
	// 計算閃避點
	protected virtual void CalculateSubTarget( GameObject _SrcObj , 
												GameObject _ObstacleObj , 
												ref Vector3 _SubTarget )
	{
		Vector3 vecToTarget = _ObstacleObj.transform.position - _SrcObj.transform.position ;
		Vector3 vecCenterLine = _SrcObj.transform.forward ;
		float standarddistance = 15.0f ;
		if( vecToTarget.sqrMagnitude > standarddistance * standarddistance )
			vecCenterLine *= vecToTarget.magnitude ;
		else
			vecCenterLine *= vecToTarget.magnitude * 2.0f ;

		Quaternion rotate = Quaternion.identity ;
		rotate = Quaternion.AngleAxis( m_EvasionRotateAngle , _SrcObj.transform.up ) ;
		Vector3 newVec1 = rotate * vecCenterLine ;
		rotate = Quaternion.AngleAxis( m_EvasionRotateAngle * -1.0f , _SrcObj.transform.up ) ;
		Vector3 newVec2 = rotate * vecCenterLine ;

		Vector3 DestPos1 = _SrcObj.transform.position + newVec1 ;
		Vector3 DestPos2 = _SrcObj.transform.position + newVec2 ;
		Vector3 DistVec1 = DestPos1 - _ObstacleObj.transform.position ;
		Vector3 DistVec2 = DestPos2 - _ObstacleObj.transform.position ;
		if( DistVec1.sqrMagnitude > DistVec2.sqrMagnitude )
		{
			_SubTarget = DestPos1 ;
		}
		else
			_SubTarget = DestPos2 ;
		
	}	
}
