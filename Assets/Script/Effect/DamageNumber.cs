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
@file DamageNumber.cs
@brief 傷害文字特效
@author NDark

# 目前沒有跟其他特效分類
# 由 UnitDamageSystem 所啟動 掛載在GUIText物件
# 但不掛在受損單位下
# UnitDamageSystem 消失時會連帶被摧毀
# Setup() 設定目標物件與顯示文字
# StartByTime() 設定經過時間
# m_Shift_Speed 移動速度
# m_TargetObj 附著在的目標物件

@date 20121111 .file created.
@date 20121204 . comment
@date 20121218 by NDark 
. comment.
. add class method VisibleGUIText()
. remove m_Shift_Vector
@date 20130112 by NDark . comment.
@date 20130205 by NDark . comment.

*/
using UnityEngine;

public class DamageNumber : MonoBehaviour 
{

	public void Setup( GameObject _TargetObj , string _DisplayText )
	{
		if( null == _TargetObj )
			return ;
		
		UpdatePos( _TargetObj ) ;
		m_TargetObj = _TargetObj ;
		
		if( null != this.gameObject.guiText )
		{
			GUIText guiText = this.gameObject.guiText ;
			guiText.text = _DisplayText ;
			guiText.pixelOffset = Vector2.zero ;
		}
	}

	public void StartByTime( float _ElapsedSec ) 
	{
		m_CountDownTrigger.Setup( _ElapsedSec ) ;
		m_CountDownTrigger.Rewind() ;
		IsActive = true ;
		
		VisibleGUIText( true ) ;
	}
		
	// Use this for initialization
	void Start () 
	{
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		if( true == IsActive )
		{
			UpdatePos( m_TargetObj ) ;
			
			if( true == m_CountDownTrigger.IsCountDownToZero() )
			{
				VisibleGUIText( false ) ;
				IsActive = false ;
			}
		}
	}
	
	private void VisibleGUIText( bool _Visible )
	{
		if( null != this.gameObject.guiText )
		{
			this.gameObject.guiText.enabled = _Visible ;
		}
	}

	private void UpdatePos( GameObject _TargetObj )
	{
		if( null != _TargetObj )
		{
			Vector3 viewportPosition = Camera.mainCamera.WorldToViewportPoint( _TargetObj.transform.position ) ;
			this.transform.position = viewportPosition ;
		}

		GUIText guiText = this.gameObject.guiText ;
		if( null != guiText )
		{
			Vector2 shiftVectorNow = m_Shift_Speed * Time.deltaTime ;
			guiText.pixelOffset = guiText.pixelOffset + shiftVectorNow ;
		}					
	}
	
	private bool IsActive = false ;
	private CountDownTrigger m_CountDownTrigger = new CountDownTrigger() ;
	private GameObject m_TargetObj = null ;// 附著在的目標物件
	private Vector2 m_Shift_Speed = new Vector2( 0.0f , 10.0f ) ;// 移動速度
	
}
