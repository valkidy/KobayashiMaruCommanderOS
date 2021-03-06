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
@file ActiveCountDownConditionEvent.cs
@author NDark

啟動倒數計時事件

# 總時間

@date 20121224 file started.
@date 20130117 by NDark . rename file from ActiveCountDownDisplayEvent to ActiveCountDownConditionEvent
*/
// #define DEBUG

using UnityEngine;
using System.Xml;


public class ActiveCountDownConditionEvent : ConditionEvent 
{
	private float m_TotalSec = 0.0f ;
	public ActiveCountDownConditionEvent()
	{
		
	}
	
	public ActiveCountDownConditionEvent( ActiveCountDownConditionEvent _src )
	{
		m_TotalSec = _src.m_TotalSec ;
	}
	/*
	<AddScriptOnObject objectName="GlobalSingleton" scriptName="CountDownTimeManager" />	
	
	<UsualEvent EventName="ActiveCountDownConditionEvent"
			TotalSec="180.0" >
		<Condition ConditionName="Condition_Time" 
			StartTime="6.0" />			
	</UsualEvent>
	*/
	public override bool ParseXML( XmlNode _Node )
	{
#if DEBUG				
		Debug.Log( "ActiveCountDownConditionEvent::ParseXML()" ) ;
#endif
		ParseForChildren( _Node ) ;
		
		if( null == _Node.Attributes["TotalSec"] )
		{
			return false ;
		}
	
		string totalSecStr = _Node.Attributes["TotalSec"].Value ;
		float.TryParse( totalSecStr , out m_TotalSec ) ; 		
		
		return true ;
	}
	
	
	public override void DoEvent()
	{
#if DEBUG				
		Debug.Log( "ActiveCountDownConditionEvent::DoEvent()" ) ;
#endif		
		ActiveCountDownManager() ;
	}	
	
	private void ActiveCountDownManager()
	{
		GameObject globalObj = GlobalSingleton.GetGlobalSingletonObj() ;
		if( null != globalObj )
		{
			CountDownTimeManager manager = globalObj.GetComponent< CountDownTimeManager >() ;
			if( null != manager )
				manager.Setup( m_TotalSec ) ;
		}
	}
		
}
