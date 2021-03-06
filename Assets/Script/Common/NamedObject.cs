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
@file NamedObject.cs
@brief 名稱與物件取得器 
@author NDark 
 
@date 20121108 . file started.
@date 20121109 by NDark . add class method GetObj()
@date 20121218 by NDark 
. refactor
. add class method Setup()
@date 20130109 by NDark . add class method Clear()
*/
// #define DEBUG
using UnityEngine;

[System.Serializable]
public class NamedObject 
{
	public virtual void Clear()
	{
		m_Name = "" ;
		m_GameObject = null ;
	}
	
	public void Setup( string _Name , GameObject _GameObject )
	{
		Clear() ;
		m_Name = _Name ;
		m_GameObject = _GameObject ;
	}
	
	public void Setup( GameObject _GameObject )
	{
		if( null == _GameObject )
			return ;
		Clear() ;
		m_Name = _GameObject.name ;
		m_GameObject = _GameObject ;
	}

	public void Setup( NamedObject _Obj )
	{
		Clear() ;
		Setup( _Obj.m_GameObject ) ;
	}
	
	public string Name
	{
		get 
		{
			return m_Name ;
		}
		set
		{
			if( value != m_Name )
			{
				Setup( value , null ) ;
			}
		}
	}
	
	public GameObject GetObj()
	{
		return m_GameObject ;
	}
	
	public GameObject Obj
	{
		get
		{
			if( null == m_GameObject )
			{
				if( 0 == m_Name.Length )
				{
#if DEBUG
					Debug.Log( "0 == m_Name.Length" ) ;
#endif					
					return m_GameObject ;
				}
				
				m_GameObject = GameObject.Find( m_Name ) ;
				if( null == m_GameObject )
				{
#if DEBUG					
					Debug.Log( "null == m_GameObject=" + m_Name ) ;
#endif					
				}
			}
			return m_GameObject ;
		}
		set
		{
			Setup( value ) ;
		}
	}
	
	public GameObject ForceObj()
	{
		m_GameObject = GameObject.Find( m_Name ) ;
		if( null == m_GameObject )
		{
			Debug.Log( "null == m_GameObject=" + m_Name ) ;
		}
		return m_GameObject ;
	}
	

	// constructor
	public NamedObject()
	{
	}
	
	public NamedObject( string _Name )
	{		
		Name = _Name ;
	}
	
	public NamedObject( GameObject _GameObject )
	{
		m_Name = _GameObject.name ;
		m_GameObject = _GameObject ;
	}

	public NamedObject( NamedObject _src )
	{
		m_Name = _src.m_Name ;
		m_GameObject = _src.m_GameObject ;
	}
	
	// member
	private string m_Name = "" ;
	private GameObject m_GameObject = null ;	
	
}
