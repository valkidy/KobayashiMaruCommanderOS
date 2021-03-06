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
@file ReplaceAIConditionEvent.cs
@author NDark

切換AI的事件

參數
# ObjectName{0} 目標群組
# RemoveAIName 移除AI名稱
# AddAIName 新增AI名稱



@date 20130116 file started and copy from ReplaceAIWhenEnterEvent
*/
// #define DEBUG

using UnityEngine;
using System.Collections.Generic;
using System.Xml;

/*
切換AI的事件
*/
[System.Serializable]
public class ReplaceAIConditionEvent : ConditionEvent
{
	public List<NamedObject> m_PossibleTargets = new List<NamedObject>() ;
	public string m_RemoveAIName = "" ;
	public string m_AddAIName = "" ;
	
	public ReplaceAIConditionEvent()
	{
	}
	
	public ReplaceAIConditionEvent( ReplaceAIConditionEvent _src )
	{
		m_PossibleTargets = new List<NamedObject>( _src.m_PossibleTargets ) ;
		m_RemoveAIName = _src.m_RemoveAIName ;
		m_AddAIName = _src.m_AddAIName ;
	}
/*
	<UsualEvent EventName="ReplaceAIConditionEvent" 
			ObjectName0="Unit_AmbushUnit011" 
			ObjectName1="Unit_AmbushUnit012" 
			ObjectName2="Unit_AmbushUnit013" 
			RemoveAIName = ""
			AddAIName="AI_PowerUpAndReplaceAI" >
		
		<Condition ConditionName="Condition_Collision" 
			TestObjectName="MainCharacter" 
			PosAnchor="RescureUnitPosition2"
			JudgeDistance="30" />
		
	</UsualEvent>
 */
	public override bool ParseXML( XmlNode _Node )
	{
#if DEBUG		
		Debug.Log( "ReplaceAIConditionEvent::ParseXML()" ) ;
#endif
		ParseForChildren( _Node ) ;
		
		for( int i = 0 ; i < 10 ; ++i )
		{
			string key = string.Format( "ObjectName{0}" , i ) ;
			if( null == _Node.Attributes[ key ] )
				break ;
			m_PossibleTargets.Add( new NamedObject( _Node.Attributes[ key ].Value ) ) ;
		}
		
		if( null != _Node.Attributes["RemoveAIName"] )
			m_RemoveAIName = _Node.Attributes["RemoveAIName"].Value ;
		
		if( null != _Node.Attributes["AddAIName"] )
			m_AddAIName = _Node.Attributes["AddAIName"].Value ;

		return ( m_PossibleTargets.Count > 0 ) ;		
	}
	
	public override void DoEvent()
	{
#if DEBUG				
		Debug.Log( "ReplaceAIConditionEvent::DoEvent()" ) ;
#endif		
		ReplaceAI() ;
	}	
	
	private void ReplaceAI()
	{
		GameObject affectUnit = RetrieveAffectObject() ;
		if( null == affectUnit )
			return ;
		if( 0 != m_RemoveAIName.Length )
		{
			Component removeAI = affectUnit.GetComponent( m_RemoveAIName ) ;
			Component.Destroy( removeAI ) ;
		}
		
		if( 0 != m_AddAIName.Length )
		{
			affectUnit.AddComponent( m_AddAIName ) ;			
		}		
	}
	
	private GameObject RetrieveAffectObject()
	{
		GameObject ret = null ;
		if( 0 == m_PossibleTargets.Count )
			return ret ;
			
		int index = Random.Range( 0 , m_PossibleTargets.Count ) ;
		// Debug.Log( index ) ;
		ret = m_PossibleTargets[ index ].Obj ;
		return ret ;
	}
}
