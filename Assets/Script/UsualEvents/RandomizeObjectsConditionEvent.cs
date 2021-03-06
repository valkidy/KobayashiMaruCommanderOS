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
@file RandomizeObjectsConditionEvent.cs
@author NDark

將指定物件隨機排序時間事件

# ObjectMax 有幾個物件 
# ObjectName{0} 物件串列 從零開始

@date 20121229 by NDark
@date 20130117 by NDark . rename RandomizeObjectsTimeEvent to RandomizeObjectsConditionEvent
@date 20130202 by NDark . add class method AddObject()

*/
// #define DEBUG

using UnityEngine;
using System.Collections.Generic ;
using System.Xml;

public class RandomizeObjectsConditionEvent : ConditionEvent 
{
	private List<NamedObject> m_Objects = new List<NamedObject>() ;
	
	public void AddObject( NamedObject _Obj )
	{
		m_Objects.Add( _Obj ) ;
	}
	
	public override bool ParseXML( XmlNode _Node )
	{
#if DEBUG				
		Debug.Log( "RandomizeObjectsConditionEvent::ParseXML()" ) ;
#endif
		ParseForChildren( _Node ) ;
		
		int objectMax = 10 ;
		if( null != _Node.Attributes["ObjectMax"] )
		{
			string ObjectMaxStr = _Node.Attributes["ObjectMax"].Value ;
			int.TryParse( ObjectMaxStr , out objectMax );
		}
		
		for( int i = 0 ; i < objectMax ; ++i )
		{
			string format = string.Format( "ObjectName{0}" , i ) ;
			if( null == _Node.Attributes[format] )
				break ;
			
			string Name = _Node.Attributes[format].Value ;
#if DEBUG				
			Debug.Log( "RandomizeObjectsConditionEvent::ParseXML() ObjectName="  + Name ) ;
#endif			
			this.AddObject( new NamedObject( Name ) ) ;
		}
		
		return true ;
	}
	
	public RandomizeObjectsConditionEvent()
	{
	}
	
	public RandomizeObjectsConditionEvent( RandomizeObjectsConditionEvent _src )
	{
		m_Objects = _src.m_Objects ;
	}
		
	
	public override void DoEvent()
	{
#if DEBUG				
		Debug.Log( "RandomizeObjectsConditionEvent::DoEvent()" ) ;
#endif		
		RandomizeObjects() ;
	}	
	
	private void RandomizeObjects()
	{
		List<Vector3> positions = new List<Vector3>() ;
		foreach( NamedObject obj in m_Objects )
		{
			if( null != obj.Obj )
			{
				positions.Add( obj.Obj.transform.position ) ;
				// Debug.Log( obj.Obj.transform.position ) ;
			}
			else
				positions.Add( Vector3.zero ) ;
			
		}
		
		List<Vector3> newPositions = new List<Vector3>() ;
		
		while( positions.Count> 0 )
		{
			int index = Random.Range( 0 , positions.Count ) ;
			newPositions.Add( positions[ index ] ) ;
#if DEBUG				
			Debug.Log( "RandomizeObjectsConditionEvent::DoEvent() " + index + " " + positions[ index ]  ) ;
#endif				
			positions.RemoveAt( index ) ;
		}
		
		
		List<NamedObject>.Enumerator e = m_Objects.GetEnumerator() ;
		int i = 0 ; 
		while( e.MoveNext() )
		{
			if( null != e.Current.Obj && i < newPositions.Count )
				e.Current.Obj.transform.position = newPositions[ i++ ] ;
		}
	}	
}
