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
@file AI_CreateSelf2.cs
@author NDark
 
# 分裂版本的 AI_CreateSelf (繼承)
# 產生兩個
# 產生時分別朝向相反方向飛去
# 覆寫函式 CreateSelf()
 
@date 20121125 file started.
@date 20121203 by NDark . comment.
@date 20121204 by NDark 
. 增加檢查當勝利已分時不再分裂,直接結束
. 增加檢查當此物件已經分裂兩次,就不再分裂第三次,直接結束.
@date 20121218 by NDark . comment.

*/
using UnityEngine;

public class AI_CreateSelf2 : AI_CreateSelf 
{
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
	
	protected override void CreateSelf()
	{
		// when duplicat is too musch , do not create.
		string[] stringSeparators = new string[] {"clone"};
		string []strVec  = m_SelfUnitName.Split( stringSeparators , System.StringSplitOptions.None ) ;
		if( strVec.Length > 2 )// 第三次就不要再繁殖了
			return ; 
		
		Vector3 initPos = this.gameObject.transform.position ;
		Quaternion initQuat = this.gameObject.transform.rotation ;
		string NewName = m_SelfUnitName+"_clone1" ;
		
		Vector3 pushVec = Random.insideUnitSphere ;
		pushVec.Normalize() ;
		
		pushVec.y = 0 ;// clear y direction random
		
		pushVec *= m_ShiftScale ;
		
		Vector3 shiftVec = pushVec * m_ShiftScale ;
		
		// 產生第一個
		CreateASelfObject( NewName , initPos + shiftVec , initQuat , pushVec ) ;
		
		// 推動方向相反
		pushVec *= -1.0f ;	
		shiftVec = pushVec * m_ShiftScale ;
		NewName = m_SelfUnitName+"_clone2" ;
		
		// 產生第二個
		CreateASelfObject( NewName , initPos + shiftVec , initQuat , pushVec ) ;	
	}	
}
