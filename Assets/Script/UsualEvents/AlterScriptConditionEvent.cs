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
0@file AlterScriptConditionEvent.cs
@author NDark

調整script時間事件

# 調整的script名稱
# 調整的物件名稱
# 調整的內容

@date 20130104 file started.
@date 20130117 by NDark . rename file from AlterScriptTimeEvent to AlterScriptConditionEvent 
*/
// #define DEBUG

using UnityEngine;
using System.Xml ;

public class AlterScriptConditionEvent : ConditionEvent 
{
	private NamedObject m_TargetObject = new NamedObject() ;
	private string m_ScriptName = "" ;
	private string m_AlterCase = "" ;
	
/*
	<UsualEvent EventName="AlterScriptConditionEvent" 
			ObjectName="MainCharacter" 
			ScriptName="MainCharacterController" 
			AlterCase="Remove" 
			StartSec="4.5" 
			/>	
	<UsualEvent EventName="AlterScriptConditionEvent" 
			ObjectName="MainCharacter" 
			ScriptName="MainCharacterController" 
			AlterCase="Add" 
			StartSec="5" 
			/>	

	<UsualEvent EventName="AlterScriptConditionEvent" 
			ObjectName="MainCharacter" 
			ScriptName="MainCharacterController" 
			AlterCase="Disable" 
			StartSec="6" 
			/>
*/
	public override bool ParseXML( XmlNode _Node )
	{
#if DEBUG
		Debug.Log( "AlterScriptConditionEvent::ParseXML()" ) ;
#endif
		ParseForChildren( _Node ) ;
		
		if( null == _Node.Attributes["ObjectName"] ||
			null == _Node.Attributes["ScriptName"] ||
			null == _Node.Attributes["AlterCase"] )
		{
			return false ;
		}
		
		m_TargetObject.Setup( _Node.Attributes["ObjectName"].Value , null ) ;
		m_ScriptName = _Node.Attributes["ScriptName"].Value ;
		m_AlterCase = _Node.Attributes["AlterCase"].Value ;
		return true ;		
	}
	
	public AlterScriptConditionEvent()
	{
	}
	
	public AlterScriptConditionEvent( AlterScriptConditionEvent _src )
	{
		m_TargetObject.Setup( _src.m_TargetObject ) ;
		m_ScriptName = _src.m_ScriptName ;
		m_AlterCase = _src.m_AlterCase ;
	}
	
	public override void DoEvent()
	{
#if DEBUG				
		Debug.Log( "AlterScriptConditionEvent::DoEvent()" ) ;
#endif		
		AlterScript() ;
	}	
	
	private void AlterScript()
	{
		if( null != m_TargetObject.Obj )
		{
			if( m_AlterCase == "Remove" )
			{
				Component component = m_TargetObject.Obj.GetComponent( m_ScriptName ) ;	
				if( null != component )
				{
					Component.Destroy( component ) ;
				}
			}
			else if( m_AlterCase == "Enable" )
			{
				MonoBehaviour component = (MonoBehaviour) m_TargetObject.Obj.GetComponent( m_ScriptName ) ;
				if( null != component )
				{
					component.enabled = true ;
				}
			}
			else if( m_AlterCase == "Disable" )
			{
				MonoBehaviour component = (MonoBehaviour) m_TargetObject.Obj.GetComponent( m_ScriptName ) ;
				if( null != component )
				{
					component.enabled = false ;
				}
			}	
			else if( m_AlterCase == "Add" )
			{
				Component component = m_TargetObject.Obj.GetComponent( m_ScriptName ) ;	
				if( null == component )
				{
					m_TargetObject.Obj.AddComponent( m_ScriptName ) ;
				}
			}
			
		}
	}	
}
