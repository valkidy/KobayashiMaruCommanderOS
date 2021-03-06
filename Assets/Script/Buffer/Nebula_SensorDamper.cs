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
@file Nebula_SensorDamper.cs
@author NDark

星雲造成的感測器阻尼

# 參數 開始時會去跟 unitData.m_SupplementalVec 取得參數
## SensorDamperDistance 阻尼距離
## SensorDamperMinimum 阻尼造成的最大影響
# 定期檢查 m_RefreshTimer
# 檢查周圍的 單位 CheckUnitsArround()
# AddOrRewindDamperOnUnit() 
重上發條或是加上新的阻尼器 SensorDamper
# 阻尼器自己會消滅.
# AddMessage() 加上阻尼後會發出訊息


@date 20121207 file started.
@date 20121207 by NDark . adjust sec of m_RefreshTimer
@date 20121210 by NDark . add class member m_SensorDamperMinimum , and code of parsing it at RetrieveParam()
@date 20121218 by NDark . comment
@date 20121225 by NDark . add class method AddMessage()
@date 20130204 by NDark . comment

*/
using UnityEngine;

public class Nebula_SensorDamper : MonoBehaviour 
{
	public CountDownTrigger m_RefreshTimer = new CountDownTrigger( 0.9f ) ; // 定期檢查 
	public float m_SensorDamperDistance = 0.0f ; // 阻尼距離
	public float m_SensorDamperDistanceSquare = 0.0f ;
	public float m_SensorDamperMinimum = 0.5f ; // 阻尼造成的最大影響
	
	// Use this for initialization
	void Start () 
	{
		m_RefreshTimer.Rewind() ;
		RetrieveParam() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( true == m_RefreshTimer.IsCountDownToZero() )
		{
			CheckUnitsArround() ;
			m_RefreshTimer.Rewind() ;
		}
	}
	
	void RetrieveParam()
	{
		UnitData unitData = this.gameObject.GetComponent<UnitData>() ;
		if( null != unitData )
		{
			if( true == unitData.m_SupplementalVec.ContainsKey( "SensorDamperDistance" ) )
			{
				string SensorDamperDistanceStr = unitData.m_SupplementalVec[ "SensorDamperDistance" ] ;
				float.TryParse( SensorDamperDistanceStr , out m_SensorDamperDistance ) ;
				m_SensorDamperDistanceSquare = m_SensorDamperDistance * m_SensorDamperDistance ;
			}
			
			if( true == unitData.m_SupplementalVec.ContainsKey( "SensorDamperMinimum" ) )
			{
				string SensorDamperMinimumStr = unitData.m_SupplementalVec[ "SensorDamperMinimum" ] ;
				float.TryParse( SensorDamperMinimumStr , out m_SensorDamperMinimum ) ;
			}			
		}
	}
		
	// 檢查周圍的 單位 
	void CheckUnitsArround()
	{
		Vector3 thisGameObjectPos = this.gameObject.transform.position ;
		MainUpdate mainUpdate = GlobalSingleton.GetMainUpdateComponent() ;
		if( null == mainUpdate )
			return ;
		foreach( NamedObject unit in mainUpdate.m_UnitNamesList )
		{
			if( null == unit.Obj || 
				unit.Name == this.gameObject.name )
				continue ;
			Vector3 distanceVec = unit.Obj.transform.position - thisGameObjectPos ;
			if( distanceVec.sqrMagnitude < m_SensorDamperDistanceSquare )
			{
				AddOrRewindDamperOnUnit( unit.Obj ) ;
			}
		}
	}
	
	// 重上發條或是加上新的阻尼器 
	void AddOrRewindDamperOnUnit( GameObject _unit )
	{
		if( null == _unit )
			return ;
		SensorDamper sensorDamper = _unit.GetComponent<SensorDamper>() ;
		if( null == sensorDamper )
		{
			if( _unit.name == ConstName.MainCharacterObjectName )
			{
				AddMessage() ;
			}
			sensorDamper = _unit.AddComponent<SensorDamper>() ;
		}
		sensorDamper.m_AffectTimer.Rewind() ;
		sensorDamper.m_DamperMinimum = m_SensorDamperMinimum ;
	}
	
	// 加上阻尼後會發出訊息
	private void AddMessage()
	{
		MessageQueueManager manager = GlobalSingleton.GetMessageQueueManager() ;
		if( null != manager )
		{
			string message = StrsManager.Get( 1101 ) ;
			manager.AddMessage( message ) ;
		}
	}
}
