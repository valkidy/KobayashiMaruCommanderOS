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
@file WhenCollideOnCollider.cs
@brief 船隻碰撞處理
@author NDark

船隻碰撞處理

# 只紀錄碰撞的對象不做處理
# 目前綁在導彈的CollideCube上
# 目前只有導彈的AI會去檢查

@date 20121228 by NDark . file started.
@date 20130106 by NDark . add class member m_CollideUnitObject

*/
using UnityEngine;

public class WhenCollideOnCollider : MonoBehaviour {
	
	public bool m_IsCollide = false ;
	public string m_CollideUnitName = "" ;
	public GameObject m_CollideUnitObject = null ;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	void OnCollisionEnter( Collision collision )  
	{
		if( null != gameObject.transform.parent &&
			null != collision.gameObject.transform.parent )
		{
			GameObject targetObj = collision.gameObject.transform.parent.gameObject ;
			string TargetName = targetObj.name ;
			string ThisName = gameObject.transform.parent.gameObject.name ;
			if( ThisName == TargetName )// do not collide with my self
				return ;
			m_IsCollide = true ;
			m_CollideUnitName = TargetName ;
			m_CollideUnitObject = targetObj ;
			// Debug.Log( "ThisName=" + ThisName + " TargetName=" + TargetName ) ;
		}	
	}
}
