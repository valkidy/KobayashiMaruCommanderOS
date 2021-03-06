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
@file AI_MoveRoutes.cs
@author NDark
 
前往一連串位置

# 繼承 AI_MoveToDestination
# 參數 
## 位置點 RoutePosition{0}
目前最多分析10個,從0開始
## "judgeDistance"	
# 前往指定位置 並判斷是否到位後要切換下一個目標 GoToDestination()
## 到達後要停止移動
 
@date 20121209 file started.
@date 20121210 by NDark . refactor.
@date 20121218 by NDark . refactor.
@date 20130204 by NDark . refine code.

*/
using UnityEngine;
using System.Collections.Generic;

public class AI_MoveRoutes : AI_MoveToDestination 
{
	int m_RouteIndexNow = 0 ;
	int m_MaxRouteNum = 10 ;
	
	// param
	// "judgeDistance"	
	protected List<Vector3> m_RoutePositions = new List<Vector3>() ; // "RoutePosition{0}" 
	
	// Use this for initialization
	void Start () 
	{
		SetState( AIBasicState.UnActive ) ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		// Debug.Log( (State) m_State.state ) ;
		m_State.Update() ;
		switch( (AIBasicState) m_State.state )
		{
		case AIBasicState.UnActive :
			SetState( AIBasicState.Initialized ) ;			
			break ;
		case AIBasicState.Initialized :
			if( false == RetrieveData() )
			{
				Debug.Log( "false == RetrieveData()" + this.gameObject.name ) ;
				SetState( AIBasicState.Closed ) ;
			}	
			else
				SetState( AIBasicState.Active ) ;
			break ;
		case AIBasicState.Active :
			GoToDestination() ;
			break ;
		case AIBasicState.Closed :
			break ;			
		}
	}
	


	// 前往目的地
	void GoToDestination()
	{
		UnitData unitData = null ;
		if( false == CheckUnitData( out unitData ) ||
			m_RouteIndexNow >= m_RoutePositions.Count )
		{
			SetState( AIBasicState.Closed ) ;
			return ;
		}
		
		Vector3 targetPosition = m_RoutePositions[ m_RouteIndexNow ] ;
		Vector3 vecToTarget = Vector3.zero ;
		Vector3 norVecToTarget = Vector3.zero ;		
		float angleOfTarget = 0.0f ;
		float dotOfUp = 0.0f ;
		if( true == MathmaticFunc.FindTargetRelation( this.gameObject , 
													 targetPosition ,
													 ref vecToTarget , 
													 ref norVecToTarget , 
													 ref angleOfTarget ,
													 ref dotOfUp ) )
		{
			unitData.AngularRatioHeadTo( angleOfTarget , 
										 dotOfUp , 
										 0.1f ) ;		
			
			// Debug.Log( "vecToTarget.magnitude" + vecToTarget.magnitude ) ;
			string IMPULSE_ENGINE_ANGULAR_RATIO = ConstName.UnitDataComponentImpulseEngineAngularRatio ;
			string IMPULSE_ENGINE_RATIO = ConstName.UnitDataComponentImpulseEngineRatio ;
			if( true == unitData.standardParameters.ContainsKey( IMPULSE_ENGINE_RATIO ) &&
				true == unitData.standardParameters.ContainsKey( IMPULSE_ENGINE_ANGULAR_RATIO ))
			{
				StandardParameter impulseRatio = unitData.standardParameters[ IMPULSE_ENGINE_RATIO ] ;
				if( vecToTarget.magnitude < m_JudgeDistance )
				{
					++m_RouteIndexNow ;// 抵達換下一個
					impulseRatio.now = 0 ;// 先停止(避免一直跑)

					unitData.standardParameters[ IMPULSE_ENGINE_ANGULAR_RATIO ].now = 0 ;
				}
				else
					impulseRatio.ToMax() ;
			}
		}
	}


	// 從 m_SupplementalVec 取得必要資訊
	protected override bool RetrieveData()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null != unitData )
		{
			m_RouteIndexNow = 0 ;
			m_RoutePositions.Clear() ;
			for( int i = 0 ; i < m_MaxRouteNum ; ++i )
			{
				Vector3 routeValue = Vector3.zero ;
				string routeKey = string.Format( "RoutePosition{0}" , i ) ;
				if( true == RetrieveParam( unitData , routeKey , ref routeValue ) )
				{
					m_RoutePositions.Add( routeValue ) ;
					// Debug.Log( "m_RoutePositions" + m_RoutePositions[ m_RoutePositions.Count - 1 ] );
				}
			}
			
			RetrieveParam( unitData , "judgeDistance" , ref m_JudgeDistance ) ;
		}
		return true ;
	}
		
}
