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
@file AI_FireToTarget2.cs
@author NDark

A AI based on AI_FireToTarget.
The advance part is this AI will evasive the obstacle.
對目標開火 並閃避障礙物

# 繼承 AI_FireToTarget
# 變數 
## 閃避後的轉角 m_EvasionRotateAngle
## 距離第二目標停下的距離 m_ReachDistanceSquare
# 記算第二(移動)目標 CalculateSubTarget()
記算對障礙物的左右轉各一個位置,判定哪個位置比較好.
# 前往第二目標 CheckMoveToTarget()
前往第二目標時速度減半

@date 20121205 by NDark . file started.
. adjust parameter
@date 20121210 by NDark . refactor.
@date 20121218 by NDark . refactor.
@date 20130109 by NDark . move some class members to AI_FireToClosestEnemy.
@date 20130131 by NDark . remove class method IsAnyObstacleInfrontOfUs()
@date 20130204 by NDark . refine code.

*/
using UnityEngine;

public class AI_FireToTarget2 : AI_FireToTarget 
{
	// param
	// "WaitForReloadSec"	
	// "TargetName"
	
	// Use this for initialization
	void Start () 
	{
		m_State.state = (int)AI_Fire_State.UnActive ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// check if the AI object is still alive or not
		UnitData unitData = null ;
		UnitWeaponSystem weaponSys = null ;
		if( false == CheckAbleRunAI( out unitData , out weaponSys ) )
			return ;
		
		m_State.Update() ;
		switch( (AI_Fire_State) m_State.state )
		{
		case AI_Fire_State.UnActive :
			m_State.state = (int)AI_Fire_State.CheckUnitData ;
			break ;
		case AI_Fire_State.CheckUnitData :
			
			if( m_State.IsFirstTime() )
				m_MaximumRetrieveDataTime.Rewind() ;
			
			KeepRetrieveData() ;
			
			break ;
			
		case AI_Fire_State.MoveToTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				m_State.state = (int)AI_Fire_State.End ;
				break ;
			}
			
			
			if( true == CheckWeaponAndMoveToTarget( unitData , weaponSys , m_TargetNow.Obj.transform.position ) )
			{
				
			}
			else if( true == IsAnyObstacleInfrontOfUs( m_TargetNow.Name , 
												  		ref m_SubTargetPosition ) )
			{
				m_State.state = (int)AI_Fire_State.MoveToSubTarget ;
				break ;
			}
				
			break ;
		case AI_Fire_State.MoveToSubTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				m_State.state = (int)AI_Fire_State.End ;
				break ;
			}
			
			// 前往第二目標時速度減半
			if( true == CheckMoveToTarget( unitData , weaponSys , m_SubTargetPosition , 0.5f ) )
			{
				
			}
			else if( true == IsAnyObstacleInfrontOfUs( m_TargetNow.Name , 
												  	   ref m_SubTargetPosition ) )
			{
				// Debug.Log( "true == IsAnyObstacleInfrontOfUs") ;
			}						
			break ;
			
		case AI_Fire_State.StopAndFireToTarget :
			
			if( false == CheckTarget( m_TargetNow ) )
			{
				m_State.state = (int)AI_Fire_State.End ;
				break ;
			}
			
			if( true == m_State.IsFirstTime() )
			{
				unitData.AllStop() ;
			}

			FireWeapon( weaponSys , m_TargetNow ) ;
			
			break ;
			
		case AI_Fire_State.WaitForReload :
			this.CheckReloadAndBackTo( AI_Fire_State.MoveToTarget ) ;			
			break ;
			
		case AI_Fire_State.End :
			// nothing.
			break ;
		}
	
	}
	
	

	// 記算第二(移動)目標
	protected override void CalculateSubTarget( GameObject _SrcObj , 
											   GameObject _ObstacleObj , 
											   ref Vector3 _SubTarget )
	{
		Vector3 vecToTarget = _ObstacleObj.transform.position - _SrcObj.transform.position ;

		Quaternion rotate = Quaternion.identity ;
		rotate = Quaternion.AngleAxis( m_EvasionRotateAngle , _SrcObj.transform.up ) ;
		Vector3 newVec1 = rotate * vecToTarget ;
		rotate = Quaternion.AngleAxis( m_EvasionRotateAngle * -1.0f , _SrcObj.transform.up ) ;
		Vector3 newVec2 = rotate * vecToTarget ;
		
		float Angle1 = Vector3.Angle( _SrcObj.transform.forward , newVec1 ) ;
		float Angle2 = Vector3.Angle( _SrcObj.transform.forward , newVec2 ) ;
		if( Angle1 > Angle2 )
			_SubTarget = _SrcObj.transform.position + newVec2 ;
		else
			_SubTarget = _SrcObj.transform.position + newVec1 ;
		
	}
	
	// 前往第二目標 
	protected bool CheckMoveToTarget( UnitData _unitData , 
									  UnitWeaponSystem _weaponSys ,
									  Vector3 _TargetPosition , 
									  float _ImpulseSpeed = 1.0f )
	{
		
		Vector3 vecToTarget = Vector3.zero ;
		Vector3 norVecToTarget = Vector3.zero ;
		float angleOfTarget = 0.0f ;
		float dotOfUp = 0.0f ;
		string IMPLUSE_ENGINE_RATIO = ConstName.UnitDataComponentImpulseEngineRatio ;
	
		if( true == MathmaticFunc.FindTargetRelation( this.gameObject , 
													 _TargetPosition ,
													 ref vecToTarget ,
													 ref norVecToTarget ,
													 ref angleOfTarget ,
													 ref dotOfUp ) )
		{
			
			if( vecToTarget.sqrMagnitude < m_ReachDistanceSquare )
			{
				this.SetState( AI_Fire_State.MoveToTarget ) ;
				return true ;
			}
			
			if( true == _unitData.standardParameters.ContainsKey( IMPLUSE_ENGINE_RATIO ) )
			{
				_unitData.standardParameters[ IMPLUSE_ENGINE_RATIO ].now =
					_unitData.standardParameters[ IMPLUSE_ENGINE_RATIO ].max * _ImpulseSpeed ;
					
			}
			
			// angular speed
			_unitData.AngularRatioHeadTo( angleOfTarget ,
										  dotOfUp , 
										  0.1f ) ;

		}
		
		return false ;
	}	
}
