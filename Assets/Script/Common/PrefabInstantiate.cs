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
@file PrefabInstantiate.cs
@brief prefab讀入與具現化 
@author NDark
 
# Create() 具現化物件
# CreateByInit() 依照座標具現化物件

@date 20121109 by NDark . refine code.
@date 20121121 by NDark 
. add class method ResourceLoadPrefab()
. add class method CreatePrefab()
@date 20121124
. add class method CreateByInitPrefab()
. add class method ResourceLoadByPrefix()
. refactor PrefabInstantiate
@date 20121203 by NDark . comment.

*/
using UnityEngine;

public static class PrefabInstantiate
{
	static public GameObject Create( string _PrefabName , 
									 string _ObjectName )
	{
		GameObject retObj = null ;
		Object prefab = ResourceLoad.LoadPrefab( _PrefabName ) ;
		if( null == prefab )
		{
			Debug.Log( "PrefabInstantiate:Create() null == prefab:" + _PrefabName ) ;
			return retObj ;
		}
		retObj = (GameObject) MonoBehaviour.Instantiate( prefab ) ; 
		if( null == retObj )
		{
			Debug.Log( "PrefabInstantiate:Create() null == obj"  ) ;
			return retObj ; 
		}
		retObj.name = _ObjectName ;
		return retObj ;
	}
	
	static public GameObject CreateByInit( string _PrefabName , 
										   string _ObjectName ,
										   Vector3 _InitPos ,
										   Quaternion _InitQuaternion )
	{
		Object prefab = ResourceLoad.LoadPrefab( _PrefabName ) ;
		if( null == prefab )
		{
			Debug.Log( "PrefabInstantiate:CreateByInit() null == prefab:" + _PrefabName ) ;
			return null ;
		}
		GameObject obj = (GameObject) MonoBehaviour.Instantiate( prefab , _InitPos , _InitQuaternion ) ; 
		if( null == obj )
		{
			Debug.Log( "PrefabInstantiate:CreateByInit() null == obj" ) ;
			return null ; 
		}		
		obj.name = _ObjectName ;
		return obj ;
	}	
}
