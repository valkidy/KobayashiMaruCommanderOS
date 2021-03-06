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
@file ClickOnMessageCard_BackToSelectScene.cs
@brief 點選時回到選擇關卡
@author NDark


# 此功能是當遊戲中途中斷，要返回的目標場景
# 目前掛載在主選單的離開戰鬥按鈕上
# 會依照目前是否有設定 GlobalSingleton.m_InformationSceneEnd 來決定返回的場景名稱
# 若沒有設定就返回 警告頁面 Scene_Warning 重新開始


@date 20121204 by NDark . comment.
@date 20130126 by NDark . add code of GlobalSingleton.m_CurrentModeSelectSceneString at OnMouseDown()
@date 20130205 by NDark . comment.
@date 20130206 by NDark . replace m_CurrentModeSelectSceneString by m_InformationSceneEnd at OnMouseDown()
*/
using UnityEngine;

public class ClickOnMessageCard_BackToSelectScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown()
	{
		// 注意會依照GlobalSingleton的設定的場景來跳躍.
		if( 0 == GlobalSingleton.m_InformationSceneEnd.Length )
			Application.LoadLevel( "Scene_Warning" ) ;
		else
			Application.LoadLevel( GlobalSingleton.m_InformationSceneEnd ) ;
		
	}
}
