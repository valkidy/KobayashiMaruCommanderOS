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
@file XMLParsePosRoute.cs
@author NDark
@date 20121224 file started.
@date 20121224 by NDark . add DetectGUIObject
@date 20121225 by NDark . add parse of WaitDetectGUIObject

*/
using UnityEngine;
using System.Xml;

/* 
		<PosRoute Destination="MainCharacter" 
				MoveTime="0.3" 
				WaitTime="0.1" 
				MoveDetectGUIObject="MessageCard_ObjectiveLevel06" 
				WaitDetectGUIObject="MessageCard_ObjectiveLevel06" 												
				/>
	
*/
public static class XMLParsePosRoute 
{
	public static bool Parse( XmlNode _PosRouteNode , 
							  out PosRoute _Result )
	{
		_Result = new PosRoute() ;
		if( null != _PosRouteNode.Attributes[ "Destination" ] &&
			null != _PosRouteNode.Attributes[ "MoveTime" ] &&
			null != _PosRouteNode.Attributes[ "WaitTime" ] )
		{
			string DestinationStr = _PosRouteNode.Attributes[ "Destination" ].Value  ;
			XMLParseLevelUtility.ParseAnchor( DestinationStr , ref _Result.m_Destination ) ;
			
			string MoveTimeStr = _PosRouteNode.Attributes[ "MoveTime" ].Value  ;
			float.TryParse( MoveTimeStr  , out _Result.m_MoveTime ) ;

			string WaitTimeStr = _PosRouteNode.Attributes[ "WaitTime" ].Value  ;
			float.TryParse( WaitTimeStr  , out _Result.m_WaitTime ) ;			
			
			if( null != _PosRouteNode.Attributes[ "MoveDetectGUIObject" ] )
			{
				/*Debug.Log( "_PosRouteNode.Attributes[ DetectGUIObject ]" + 
					_PosRouteNode.Attributes[ "DetectGUIObject" ].Value ) ;
					*/
				_Result.m_MoveDetectGUIObject = new NamedObject( _PosRouteNode.Attributes[ "MoveDetectGUIObject" ].Value ) ;
			}
			
			if( null != _PosRouteNode.Attributes[ "WaitDetectGUIObject" ] )
			{
				/*Debug.Log( "_PosRouteNode.Attributes[ DetectGUIObject ]" + 
					_PosRouteNode.Attributes[ "DetectGUIObject" ].Value ) ;
					*/
				_Result.m_WaitDetectGUIObject = new NamedObject( _PosRouteNode.Attributes[ "WaitDetectGUIObject" ].Value ) ;
			}			
				
			return true ;
		}
		return false ;
	}

}
