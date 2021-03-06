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
@file UnitDataStruct.cs
@brif UnitData會用的基本資料
@author NDark

# StandardParameter 標準資料
# ComponentStatus 部件的狀態
# WeaponReloadStatus 武器填充的狀態
# ComponentStatusColor::GetColor() 得到狀態相對應的顏色

@date 20121111 file created.
@date 20121213 by NDark . add ComponentStatus_ForceOffline 
@date 20121218 by NDark . comment.
@date 20121225 by NDark 
. move class InterpolatePair to InterpolateTable.cs
. move class InterpolateTable to InterpolateTable.cs
@date 20130112 by NDark
. remove redundent ComponentStatus_ of enum ComponentStatus

*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic ;

/**
 * @brief 標準資料
 * 
 * 內含
 * -# now 
 * -# max maximum
 * -# min minimum , default is 0.
 * -# std standard , default is maximum.
 * -# init initialization , default is maximum.
 */
[System.Serializable]
public class StandardParameter
{
	public StandardParameter()
	{}
	public StandardParameter( StandardParameter _src )
	{
		Setup( _src.m_Now , _src.m_Max , _src.m_Min , _src.m_Std , _src.m_Init ) ;
	}	
	
	public StandardParameter( float _Now , float _Maximum )
	{
		Setup( _Now , _Maximum ) ;
	}	
	public StandardParameter( float _Now , float _Maximum , float _Initialization )
	{
		Setup( _Now , _Maximum , _Initialization ) ;
	}
	public StandardParameter( float _Now , float _Maximum , float _Minimum , float _Initialization )
	{
		Setup( _Now , _Maximum , _Minimum , _Initialization ) ;
	}	
	public StandardParameter( float _Now , float _Maximum , float _Minimum , float _Std , float _Initialization )
	{
		Setup( _Now , _Maximum , _Minimum , _Std , _Initialization ) ;
	}
	
	public void Reset()
	{
		m_Now = m_Init ;
	}
	public void Clear()
	{
		m_Now = m_Min ;
	}
	public void ToMax()
	{
		m_Now = m_Max ;
	}
	public void Setup( float _Now , 
					   float _Maximum ) 
	{
		Setup( _Now , _Maximum , 0.0f , _Maximum , _Maximum ) ;
	}			
	
	public void Setup( float _Now , 
					   float _Maximum , 
					   float _Initialization ) 
	{
		Setup( _Now , _Maximum , 0.0f , _Maximum , _Initialization ) ;
	}		
	public void Setup( float _Now , 
					   float _Maximum , 
					   float _Minimum , 
					   float _Initialization ) 
	{
		Setup( _Now , _Maximum , _Minimum , _Maximum , _Initialization ) ;
	}	
	
	public void Setup( float _Now , 
					   float _Maximum , 
					   float _Minimum , 
					   float _Std , 
					   float _Initialization ) 
	{
		m_Now = _Now ;
		m_Max = _Maximum ;
		m_Min = _Minimum ;
		m_Std = _Std ;
		m_Init = _Initialization ;
	}
	
	/// <summary>
	/// Checks the range of now from min to max
	/// </summary> 
	public void CheckRange()
	{
		if( m_Now > m_Max )
			ToMax() ;
		else if( m_Now < m_Min )
			Clear() ;				
	}
	
	/// <summary>
	/// Ratio of m_Now / m_Std
	/// </summary>
	public float Ratio()
	{
		if( 0.0f == m_Std )
			return 0.0f ;
		return m_Now / m_Std ;
	}
	
	// property
	public float now
	{
		get
		{
			return m_Now ;
		}
		set
		{
			m_Now = value ;
			CheckRange() ;
		}
	}

	public float max
	{
		get
		{
			return m_Max ;
		}
		set
		{
			m_Max = value ;
			CheckRange() ;
		}
	}
	
	public float min
	{
		get
		{
			return m_Min ;
		}
		set
		{
			m_Min = value ;
			CheckRange() ;
		}
	}	
	
	public float m_Now = 0.0f ;
	public float m_Max = 0.0f ;
	public float m_Min = 0.0f ;
	public float m_Std = 0.0f ;
	public float m_Init = 0.0f ;
}

[System.Serializable]
public enum ComponentStatus
{
	Normal ,
	Holding ,
	Danger ,
	Offline ,
	OfflineRepair ,
	OfflineAble ,
	ForceOffline ,
	Online ,
}

[System.Serializable]
public enum WeaponReloadStatus
{
	WeaponReloadStatus_Full ,
	WeaponReloadStatus_Empty ,
	WeaponReloadStatus_Reload ,
}



[System.Serializable]
public class StatusDescription
{
	public float high ;
	public float low ;
	public float zero ;
	public StatusDescription() 
	{}
	public StatusDescription( StatusDescription _src )
	{
		high = _src.high ;
		low = _src.low ;
		zero = _src.zero ;
	}
}


[System.Serializable]
public class ComponentStatusColor
{
	public static Color GetColor( ComponentStatus _status )
	{
		Color retColor = Color.white ;
		switch( _status )
		{
		case ComponentStatus.Offline :
		case ComponentStatus.OfflineAble :
		case ComponentStatus.OfflineRepair :
		case ComponentStatus.ForceOffline :			
			retColor = Color.gray ;
			break ;
		case ComponentStatus.Normal :
			retColor = new Color( 0.5f , 1.0f , 0.5f , 1 ) ;
			break ;
		case ComponentStatus.Holding :
			retColor = Color.yellow ;
			break ;
		case ComponentStatus.Danger :
			retColor = new Color( 1.0f , 0.5f , 0 , 1 ) ;
			break ;
		}
			
		return retColor ;
	}
}
