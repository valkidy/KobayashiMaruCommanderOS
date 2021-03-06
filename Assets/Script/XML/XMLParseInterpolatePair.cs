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
@file XMLParseInterpolatePair.cs
@brief 分析XML傳出 InterpolatePair 資料
@author NDark 
@date 20121225 . file started.
*/

using UnityEngine;
using System.Collections.Generic;
using System.Xml ;

public static class XMLParseInterpolatePair 
{
	public static bool Parse( /*in*/ XmlNode _PairNode ,
							  out InterpolatePair _Result )
	{
		_Result = new InterpolatePair() ;
		if( null != _PairNode.Attributes[ "index" ] &&
			null != _PairNode.Attributes[ "value" ] )
		{
			string indexStr = _PairNode.Attributes[ "index" ].Value ;
			string valueStr = _PairNode.Attributes[ "value" ].Value ;
			float index = 0 ;
			float Value = 0 ;
			float.TryParse( indexStr , out index ) ;
			float.TryParse( valueStr , out Value ) ;
			_Result.m_Pair = new KeyValuePair<float, float>( index , Value ) ;
			return true ;
		}
		return false ;
	}
	
}
