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
@file InitializeStrsManager.cs
@author NDark

初始化語言管理器

# 掛載在各場景的主要節點上
# 如果已經初始化就不要重複初始化
# 會取得目前玩家的使用語言

@date 20130121 file started.
@date 20130122 by NDark . change default language to English.
@date 20130126 by NDark . add code of retrieve user prefer language at Start()

*/
using UnityEngine;

public class InitializeStrsManager : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		Language defaultLanguage = Language.English ;
		string LanguageStr = PlayerPrefs.GetString( "UserLanguage" ) ;
		if( LanguageStr == Language.English.ToString() )
			defaultLanguage = Language.English ;
		else if( LanguageStr == Language.TraditionalChinese.ToString() )
			defaultLanguage = Language.TraditionalChinese ;
		
		if( false == StrsManager.m_Initialized )
		{
			ResourceLoad.SetLanguageNow( defaultLanguage ) ;
			StrsManager.Setup( "Strs.txt" ) ;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
}
