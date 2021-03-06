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
@file ComponentDataSet.cs
@brief 部件運作的相關物件集
@author NDark

# ComponentName of this component in UnitData.
# UnitObjectName and its unit object of the component.
# Component 3D Object Name and its Object of the component under GameObject.
# Effect 3D Object Name and its Object of the component.
# m_Component3DObject.Name 與 m_Effect3DObject.Name 會自動在設定 ComponentName 時創造出來

@date 20121111 . file started.
@date 20121121 . add class method Effect3DObjectGetObj()
@date 20121123 . rename to m_UnitGameObject
@date 20121203 . comment.
@date 20121218 . comment.
@date 20130112 
. comment.
. rename class method GameObjectName to UnitObjectName
. rename class member m_UnitGameObject to m_UnitObject
. fix an error that m_UnitObject.Setup should before ComponentName.
@date 20130126 . add class method ObjUnitData()
@date 20130204 by NDark . comment.

*/
using UnityEngine;

[System.Serializable]
public class ComponentDataSet
{
	public string ComponentName
	{
		get
		{
			return m_ComponentName ;
		}
		set
		{
			m_ComponentName = value ;
			m_Component3DObject.Name = ConstName.CreateComponent3DObjectName( this.UnitObjectName , 
				m_ComponentName ) ;
			m_Effect3DObject.Name = ConstName.CreateComponentEffectObjectName( this.UnitObjectName , 
				m_ComponentName ) ;
		}
	}
	
	public string UnitObjectName
	{
		get
		{ 
			return m_UnitObject.Name ;
		}
		set
		{
			m_UnitObject.Name = value ;
		}
	}
	public GameObject UnitGameObject
	{
		get
		{
			return m_UnitObject.Obj ;
		}
	}
	public UnitData ObjUnitData()
	{
		return m_UnitObject.ObjUnitData ;
	}
	

	
	public string Component3DObjectName
	{
		get
		{
			return m_Component3DObject.Name ;
		}
	}
	public GameObject Component3DObject
	{
		set
		{
			m_Component3DObject.Obj = value ;
		}		
		get
		{
			return m_Component3DObject.Obj ;
		}
	}

	public string Effect3DObjectName
	{
		get
		{
			return m_Effect3DObject.Name ;
		}
	}	
	
	public GameObject Effect3DObjectGetObj()
	{
		return m_Effect3DObject.GetObj() ;
	}
	
	public GameObject Effect3DObject
	{
		set
		{
			m_Effect3DObject.Obj = value ;
		}
		get
		{
			return m_Effect3DObject.Obj ;
		}
	}	
	
	
	// constructor
	public ComponentDataSet()
	{
	}
	public ComponentDataSet( GameObject _UnitGameObject , 
						     string _ComponentName )
	{
		m_UnitObject.Setup( _UnitGameObject ) ; // m_UnitObject.Setup should before ComponentName.
		ComponentName = _ComponentName ;	
	}
	public ComponentDataSet( ComponentDataSet _src )
	{
		ComponentName = _src.ComponentName ;	
		m_UnitObject.Setup( _src.m_UnitObject ) ;
		m_Component3DObject.Name = _src.m_Component3DObject.Name ;
		m_Effect3DObject.Name = _src.m_Effect3DObject.Name ;
	}
	
	// member
	private string m_ComponentName = "" ;
	protected UnitObject m_UnitObject = new UnitObject();
	private NamedObject m_Component3DObject = new NamedObject() ;
	private NamedObject m_Effect3DObject = new NamedObject() ;
	

}

