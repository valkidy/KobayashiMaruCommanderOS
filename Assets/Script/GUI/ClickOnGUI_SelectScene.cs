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
@file ClickOnGUI_SelectScene.cs
@brief 按下後開啟關卡
@author NDark

# 按下後開啟關卡(目前進入感謝場景)
# 依照物件名稱來決定關卡字串
設定在 GlobalSingleton.m_LevelString 上
# 掛載在選擇關卡場景的關卡圖片上
# 有緩衝時間,當物件顯示後一段時間才可以按.
# 寫入關卡已被遊玩的設定
# 注意 寫入字串為 物件名稱的 第二段 也就是 
GUI_SelectScene:Level01_Tutorial 的 Level01_Tutorial
# m_SetAcknoledgeGUIObjeName 是否要啟動感謝GUIObject
會寫入 GlobalSingleton.m_AcknowledgementGUIOBjectName
# 目前進入感謝場景(如沒有感謝物則繼續到戰場)

@date 20121127 by NDark . modify to load scene Scene_BattleLevel
@date 20121202 by NDark . add write played level.
@date 20121204 by NDark . commnet.
@date 20121219 by NDark . replace by ConstName.GetSplitVec() at LevelString()
@date 20121226 by NDark
. add class member m_SetAcknoledgeGUIObjeName
. modify load level from battle scene to Scene_Acknowledge
@date 20130112 by NDark 
. comment.
. remove class method LevelString()
@date 20130125 by NDark . add code of GUIText at Update()

*/
using UnityEngine;

public class ClickOnGUI_SelectScene : MonoBehaviour 
{
	public string m_SetAcknoledgeGUIObjeName = "" ;// 是否要啟動感謝GUIObject
	
	private TimeTrigger m_WaitTimer = new TimeTrigger( 0 , BaseDefine.WAIT_SEC_AFTER_SHOW ) ;// 有緩衝時間,當物件顯示後一段時間才可以按.
	
	// Use this for initialization
	void Start () 
	{
		m_WaitTimer.Initialize() ;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( 
			(
			( null != this.gameObject.guiTexture && true == this.gameObject.guiTexture.enabled ) ||
			( null != this.gameObject.guiText && true == this.gameObject.guiText.enabled ) 
			)
		    &&
			true == m_WaitTimer.IsAboutToStart( true ) )
		{
		}
	}

	void OnMouseDown()
	{
		if( true == m_WaitTimer.IsAboutToClose( true ) ) 
		{
			if( 0 != m_SetAcknoledgeGUIObjeName.Length )
				GlobalSingleton.m_AcknowledgementGUIOBjectName = m_SetAcknoledgeGUIObjeName ;
			
			string levelString = ConstName.GetSplitVecConetent( this.gameObject.name , 1 ) ;
			if( 0 != levelString.Length )
			{
				// Debug.Log( "levelString=" + levelString ) ;
				// 設定關卡名稱
				GlobalSingleton.m_LevelString = levelString ;
				
				// 寫入關卡已被遊玩的設定
				PlayerPrefs.SetString( levelString + "IsPlayed" , "true" ) ;
				
				// 目前進入感謝場景
				Application.LoadLevel( ConstName.Scene_Acknowledge ) ;
			}
		}
	}
}
