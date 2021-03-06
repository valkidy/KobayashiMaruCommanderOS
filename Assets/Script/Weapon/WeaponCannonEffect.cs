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
@file WeaponCannonEffect.cs
@author NDark
 
# 繼承自 WeaponEffect
# 掛載在加農炮類型武器特效物件上
# 更新特效物件前進
# 檢查碰撞
# 碰撞後關閉動畫
# 製造傷害
# m_MoveSpeed 移動速度

@date 20121211 file created.
@date 20121219 . comment.
@date 20121219 by NDark . re-factor.
@date 20121220 by NDark . fix an error of use unit name as component name at CheckHit()
@date 20130119 by NDark . add class method CheckIsOutOfScreen()
@date 20130130 by NDark 
. add class method CloseFireAnimation()
. remove init of m_MoveSpeed at Start()

*/
using UnityEngine;

public class WeaponCannonEffect : WeaponEffect 
{
	public float m_MoveSpeed = 30.0f ;

	// Use this for initialization
	void Start () 
	{
		this.ClearWeaponDataShared() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( null != m_WeaponDataShared )
		{

			UpdatePosition() ;
			CheckIsOutOfScreen() ;
			this.CheckRendererDisable() ;
		}		
	
	}
	
	void OnCollisionEnter( Collision collision )  
	{
		CheckHit( collision ) ;
	}
	
	
	protected virtual void UpdatePosition()
	{
		Vector3 ToTarget = m_WeaponDataShared.m_TargetDirection * ( m_MoveSpeed * Time.deltaTime ) ;	
		this.gameObject.transform.position = this.gameObject.transform.position + ToTarget ;
		this.gameObject.transform.rotation = Quaternion.LookRotation( m_WeaponDataShared.m_TargetDirection , 
																 new Vector3( 0 , 1 , 0 ) ) ;				
	}
	
	protected virtual void CloseFireAnimation()
	{
		Renderer renderer = this.gameObject.GetComponentInChildren<Renderer>() ;
		if( null != renderer )
			renderer.enabled = false ;
		Collider collider = this.gameObject.GetComponentInChildren<Collider>() ;
		if( null != collider )
			collider.enabled = false ;
		
		m_WeaponDataShared.m_FireState = WeaponFireState.FireCompleting ;
	}
	
	protected virtual void Hit( UnitDamageSystem _DmgSys , string hitComponentName )
	{
		// 取得攻擊者顯示字串
		string attackerUnitDisplayName = m_WeaponDataShared.UnitObjectName ;
		UnitData attackerUnitData = m_WeaponDataShared.ObjUnitData() ;		
		if( null != attackerUnitData )
			attackerUnitDisplayName = attackerUnitData.GetUnitDisplayName() ;
		
		_DmgSys.ActiveDamageEffectByTime( this.gameObject , 
										  hitComponentName , 1.0f ) ;
		
		_DmgSys.ActiveDamageNumberEffectNextTime( true , 3.0f ) ;
		_DmgSys.CauseDamageValueOut( m_WeaponDataShared.UnitObjectName ,
									 attackerUnitDisplayName ,
								  	 m_WeaponDataShared.m_CauseDamage , 
								  	 hitComponentName ) ;
		
		
		CloseFireAnimation() ;
	}

	protected virtual void CheckHit( Collision _collision )
	{
		if( null == m_WeaponDataShared )
			return ;
		string collisionComponentName = _collision.collider.name ;
		if( -1 != collisionComponentName.IndexOf( m_WeaponDataShared.UnitObjectName ) )
		{
			// do not collide with self
			return ;
		}
		// Debug.Log( "collision=" + _collision.collider.name ) ;
		
		// find parent		
		GameObject findParnetObj = _collision.collider.gameObject ;
		while( null != findParnetObj.transform.parent )
		{
			findParnetObj = findParnetObj.transform.parent.gameObject ;
		}
		GameObject unitObj = findParnetObj ;
		
		// active possible damage 
		UnitDamageSystem dmgSys = unitObj.GetComponent<UnitDamageSystem>() ;
		if( null != dmgSys )
		{
			Hit( dmgSys , collisionComponentName );
		}
		
	}
	
	protected void CheckIsOutOfScreen()
	{
		if( true == MathmaticFunc.IsTooFarFromScreen( this.gameObject ) )
		{
			// Debug.Log( "false == MathmaticFunc.IsInScreen" ) ;
			Renderer renderer = this.gameObject.GetComponentInChildren<Renderer>() ;
			if( null != renderer )
			{
				renderer.enabled = false ;
			}
		}
	}	
}
