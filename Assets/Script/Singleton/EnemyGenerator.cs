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
@file EnemyGenerator.cs
@brief 單位產生器 
@author NDark
 
# 依照設定產生單位及敵人的控制
# 分成單位及敵人兩個串列來產生
## m_UnitGenerationTable
## m_EnemyGenerationTable
# 索引也是分為兩個串列
## m_UnitGenerationIndex
## m_EnemyGenerationIndex
# 已經產生的敵人特別紀錄在 m_EnemyGeneratedTable
# AddUnitGeneration() 新增產生的單位
## 目前是依照名稱來決定是單位還是敵人
# 呼叫 GenerateEnemy() 產生的物件是依照資料結構 UnitGenerationData 內的資料來呼叫 LevelGenerator::GenerateUnit() 產生物件
# 目前 產生的順序是依照索引,依次判斷時間產生.
# 目前 呼叫 GetRemainingEnemyNum() 透過此計算場上還有的敵人 讓模組化的勝負判定 判定勝利
其中是要判定 還在排序中的敵人 + 場景中的敵人  是否已經消滅=0
# InsertEnemyGenerationTable() 立即插入敵人
# TryActiveBattleEventCamera() 嘗試啟動戰場特寫



@date 20121103 by NDark . add class member m_EnemyGeneratedTable[].
@date 20121104 by NDark . move m_EnemyGenerationTime to EnemyEvent.
@date 20121110 by NDark . add class member m_EnemyUnitDataTemplateName for GenerateUnit() at GenerateEnemy()
@date 20121114 by NDark . fix an error of not specifing correct data template name.
@date 20121126 by NDark . refactor for UnitGenerationData.
@date 20121204 by NDark . comment.
@date 20121218 by NDark . comment.
@date 20121224 by NDark . add unitName at GenerateEnemy()
@date 20121229 by NDark 
. add class method AddUnitGeneration()
. add class member m_UnitGenerationIndex
. add class member m_UnitGenerationTable
. rename class method GenerateEnemy() to GenerateUnit()
@date 20130102 by NDark . add class method InsertEnemyGenerationTable()
@date 20130113 by NDark . comment.
@date 20130119 by NDark . add class method TryActiveBattleEventCamera()
@date 20130121 by NDark . add checking of IsInScreen() at TryActiveBattleEventCamera()

*/
using UnityEngine;
using System.Collections.Generic;

public class EnemyGenerator : MonoBehaviour 
{
	
	public int m_UnitGenerationIndex = 0 ;// 產生單位的序號
	public int m_EnemyGenerationIndex = 0 ;// 產生單位的序號
	
	
	// 需要產生的單位
	private List<UnitGenerationData> m_UnitGenerationTable = new List<UnitGenerationData>() ;
	
	// 需要產生的"敵人"單位
	private List<UnitGenerationData> m_EnemyGenerationTable = new List<UnitGenerationData>() ;
	
	// 已經產生的"敵人"單位
	public List<string> m_EnemyGeneratedTable = new List<string>() ;
	
	public int Debug_RemainingEnemyNum = 0 ;
	
	public void InsertEnemyGenerationTable( UnitGenerationData _AddGenData ) 
	{
		m_EnemyGenerationTable.Insert( m_EnemyGenerationIndex , _AddGenData ) ;
	}
	
	// 檢查產生後的單位是否已經全消滅
	public int GetRemainingEnemyNum()
	{
		int EnemyShipInTheQueue = m_EnemyGenerationTable.Count - m_EnemyGenerationIndex ;
		int EnemyShipInTheField = m_EnemyGeneratedTable.Count ;		
		return ( EnemyShipInTheQueue ) + ( EnemyShipInTheField ) ;
	}
	
	public void AddUnitGeneration( UnitGenerationData _AddData )
	{
		if( -1 != _AddData.unitName.IndexOf( "Enemy_" ) )
		{
			m_EnemyGenerationTable.Add( _AddData ) ;
		}
		else
		{
			m_UnitGenerationTable.Add( _AddData ) ;			
		}
	}
	
	// Use this for initialization
	void Start () 
	{
	}
	

	
	// Update is called once per frame
	void Update () 
	{
		Debug_RemainingEnemyNum = GetRemainingEnemyNum() ;
		
		CheckGenerateUnit( ref m_UnitGenerationIndex , ref m_UnitGenerationTable ) ;
		
		CheckGenerateUnit( ref m_EnemyGenerationIndex , ref m_EnemyGenerationTable ) ;
		
	}
	
	private void CheckGenerateUnit( ref int _Index , ref List<UnitGenerationData> _Table )
	{
		if( _Index < _Table.Count )
		{
			float NextGenerationTime = _Table[ _Index ].time ;
			// Debug.Log( "NextGenerationTime=" + NextGenerationTime + " Time.timeSinceLevelLoad=" + Time.timeSinceLevelLoad ) ;
			if( Time.timeSinceLevelLoad > NextGenerationTime ) 
			{
				GenerateUnit( _Table[ _Index ] ) ;
				++_Index ;
			}
		}		
	}
	
	private void GenerateUnit( UnitGenerationData _UnitGenData )
	{
		LevelGenerator levelGenerator = this.gameObject.GetComponent<LevelGenerator>() ;
		if( null != levelGenerator )
		{
			string unitName = "" ;
			if( 0 == _UnitGenData.unitName.Length )
				unitName = ConstName.CreateEnemyGenerateObjectName( _UnitGenData.raceName ,
																	_UnitGenData.prefabName ) ;
			else
				unitName = _UnitGenData.unitName ;
			
			GameObject retObj = levelGenerator.GenerateUnit( unitName ,
										 _UnitGenData.prefabName ,
										 _UnitGenData.unitDataTemplateName ,
										 _UnitGenData.raceName ,
										 _UnitGenData.sideName ,				
										 _UnitGenData.initPosition.GetPosition() ,
										 _UnitGenData.initOrientation ,
										 _UnitGenData.supplementalVec ) ;
			

			TryActiveBattleEventCamera( retObj ) ;
			
			if( -1 != unitName.IndexOf( "Enemy_" ) )
				m_EnemyGeneratedTable.Add( unitName ) ;
		}
		
	}
					
	private void TryActiveBattleEventCamera( GameObject _TargetObj )
	{
		BattleEventCameraManager battleEventManager = GlobalSingleton.GetBattleEventCameraManager() ;
		GameObject mainChar = GlobalSingleton.GetMainCharacterObj() ;
		if( null != _TargetObj &&
			null != mainChar &&
			null != battleEventManager &&
			false == battleEventManager.IsActive() )
		{
			if( true == MathmaticFunc.IsInScreen( _TargetObj ) )
				return ;
			
			Vector3 distVec = _TargetObj.transform.position - mainChar.transform.position ;
			int viewportindex = 0 ;
			if( Mathf.Abs( distVec.z ) >= Mathf.Abs( distVec.x ) )
			{
				if( distVec.z >= 0 )
				{
					viewportindex = 0 ;
				}
				else if( distVec.z < 0 )
				{
					viewportindex = 1 ;
				}			
			}
			else
			{
				if( distVec.x >= 0 )
				{
					viewportindex = 3 ;
				}
				else if( distVec.x < 0 )
				{
					viewportindex = 2 ;
				}
			}
			
			NamedObject obj = new NamedObject( _TargetObj ) ;
			battleEventManager.SetupByTime( obj , viewportindex , 
				BaseDefine.BATTLE_EVENT_CAMERA_ELAPSED_SEC ) ;
		}		
	}
}
