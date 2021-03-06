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
@file AI_CreateSelf.cs
@author NDark

# 沒有繼承AI_Base
# 物件死亡時創造自己
# 但由於關卡結束時物件也會死亡,此時就不可以複製.
# 因此判斷機制為檢查 單位的死亡狀態,呈現死亡時才觸發複製機制
# 必要資訊為
m_RaceName 
m_SideName
m_SelfDataTemplateName 
m_SelfPrefabTemplateName 
m_SelfSupplementalVec 
m_SelfUnitName 
# 名稱會變長
# 重生時會使用 PushForce 讓物件強制移動不要停留在原本的位置.
# 重生前會檢查是否有必要產生子代prefab資源,增加變化.
## m_CreatSelfPercentage 重生的機率(有可能不會重生)


@date 20121125 file started.
@date 20121127 by NDark . add class member m_SelfSupplementalVec
@date 20121203 by NDark . comment.
@date 20121204 by NDark 
. fix an error of clean axis y
. add class member m_CreatSelfPercentage
. add class method CheckCreateSelf()
@date 20121218 by NDark . rename class method CheckInterate() to CheckInterate()
@date 20130109 by NDark . add class member m_SideName
@date 20130204 by NDark . refine code.

*/
using UnityEngine;
using System.Collections.Generic;

public class AI_CreateSelf : MonoBehaviour 
{
	
	protected bool m_RetrieveData = false ;// 是否已經取得必要資訊.
	
	// 必要資訊
	protected string m_SelfUnitName = "" ;
	protected string m_SelfPrefabTemplateName = "" ;
	protected string m_SelfDataTemplateName = "" ;
	protected Dictionary<string,string> m_SelfSupplementalVec= new Dictionary<string, string>() ;
	protected string m_RaceName = "" ;
	protected string m_SideName = "" ;
	
	// 重生時的強制移動
	protected float m_ShiftScale = 3.0f ;
	protected float m_PushForceElspasedSec = 1.0f ;
	
	private float m_CreatSelfPercentage = 0.8f ;// 重生的機率.
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		// 檢查是否勝利與失敗,否則不可執行AI		
		if( true == GlobalSingleton.GetVictoryEventManager().IsWinOrLose() )
			return ;
		
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null == unitData )
			return ;
		
		if( false == m_RetrieveData )
		{
			// do only once
			RetrieveData( unitData ) ;
			m_RetrieveData = true ;
		}
		else
		{
			CheckCreateSelf( unitData ) ;
		}
	}
	
	// 取得必要資訊
	protected void RetrieveData( UnitData _unitData ) 
	{
		m_RaceName = _unitData.m_RaceName ;
		m_SideName = _unitData.m_SideName ;
		m_SelfDataTemplateName = _unitData.m_UnitDataTemplateName ;
		m_SelfPrefabTemplateName = _unitData.m_PrefabTemplateName ;
		m_SelfSupplementalVec = _unitData.m_SupplementalVec ;
		m_SelfUnitName = this.gameObject.name ;
		
		// 檢查是否有必要產生子代prefab資源
		CheckIterate() ;
	}
		
	
	protected void CheckCreateSelf( UnitData _unitData )
	{
		if( _unitData.m_UnitState.state == (int)UnitState.Dead )
		{
			int randomvalue = Random.Range( 0 , 100 ) ;
			if( randomvalue < m_CreatSelfPercentage * 100 )
			{
				// ex. m_CreatSelfPercentage = 90
				// 0~90 < 90 : create
				// 91~100 > 90 : not create
				CreateSelf() ;
			}
		}
	}
	
	// 觸發複製機制
	protected virtual void CreateSelf()
	{
		Vector3 initPos = this.gameObject.transform.position ;
		Quaternion initQuat = this.gameObject.transform.rotation ;
		string NewName = m_SelfUnitName+"_clone" ;
		
		Vector3 pushVec = Random.insideUnitSphere ;
		pushVec.Normalize() ;
		pushVec.y = 0 ;// clear y direction random
		
		pushVec *= m_ShiftScale ;
		
		Vector3 shiftVec = pushVec * m_ShiftScale ;
		
		CreateASelfObject( NewName , initPos + shiftVec , initQuat , pushVec ) ;
	}	
	
	// 依照參數創造一個自己
	protected void CreateASelfObject( string _Name , 
									  Vector3 _initPos , 
									  Quaternion _initQuat , 
									  Vector3 _pushVec )
	{
		LevelGenerator levelGen = GlobalSingleton.GetLevelGeneratorComponent() ;
		if( null == levelGen )
			return ;
		
		GameObject newObj = levelGen.GenerateUnit( _Name , 
												   m_SelfPrefabTemplateName , 
												   m_SelfDataTemplateName , 
												   m_RaceName ,
												   m_SideName ,			
												   _initPos, 
												   _initQuat ,
													m_SelfSupplementalVec ) ;		
		
		// 把新物件推開
		PushForce pushForce = newObj.AddComponent<PushForce>() ;
		pushForce.SetupByTime( _pushVec , m_PushForceElspasedSec ) ;
	}
	
	// 檢查是否有子代prefab資源可以產生,如果有,產生的物件改為子代prefab資源
	private void CheckIterate()
	{
		string testTemplateName = m_SelfPrefabTemplateName ;
		if( -1 != testTemplateName.IndexOf( "01" ) )
		{
			testTemplateName = testTemplateName.Replace( "01" , "02" ) ;
			if( null != ResourceLoad.LoadPrefab( testTemplateName ) )
			{
				m_SelfPrefabTemplateName = testTemplateName ;
				m_SelfDataTemplateName = m_SelfDataTemplateName.Replace( "01" , "02" ) ;
			}
		}		
	}	
}
