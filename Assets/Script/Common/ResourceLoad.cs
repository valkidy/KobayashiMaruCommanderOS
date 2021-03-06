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
@file ResourceLoad.cs
@brief 資源讀入 
@author NDark

# 資源讀入
## GetDataPath() 取得資料(data)夾的位置，同時考慮語言。
## SetLanguageNow() 設定目前使用的語言，同時會修改路徑 CONST_LanguagePrefix
## LoadAudio()
## LoadTexture()
## LoadPrefab()
## LoadDataToTextAsset() 將Resource內的資料讀為TextAsset

@date 20121110 by NDark . add LoadTexture()
@date 20121124 by NDark . add LoadPrefabByPrefix() from PrefabInitantiate.cs
@date 20121202 by NDark 
. add LoadData() , LoadDataByPrefix()
. add class member CONST_DataPrefix
@date 20121203 by NDark . comment.
@date 20121218 by NDark . formmat.
@date 20130121 by NDark
. add class member m_LanguageNow
. add class member CONST_CommonPrefix
. add class method SetLanguageNow()
. add class method LoadDataByLanguage()
@date 20130126 by NDark
. add class method GetDataPath()
. rename class method LoadData() to LoadDataToTextAsset()
. remove class method LoadDataByLanguage()
@date 20130205 by NDark . comment.

*/
using UnityEngine;

public static class ResourceLoad 
{
	public static Language m_LanguageNow = Language.English ;
	public static string CONST_LanguagePrefix = "English/" ;
	public static string CONST_CommonPrefix = "Common/" ;
	public static string CONST_AudioPathPrefix = "Audio/" ;
	public static string CONST_TexturePathPrefix = "Textures/" ;
	public static string CONST_PrefabPrefix = "Prefabs/" ;
	public static string CONST_DataPrefix = "Data/" ;
	
	public static string GetDataPath( string _File , bool _UseLanguage )
	{
		string LanguagePrefix = ( true == _UseLanguage ) ? CONST_LanguagePrefix : CONST_CommonPrefix ;
		return LanguagePrefix + CONST_DataPrefix + _File ;
	}
	
	public static void SetLanguageNow( Language _Language )
	{
		m_LanguageNow = _Language ;
		switch( m_LanguageNow )
		{
		case Language.English :
			CONST_LanguagePrefix = Language.English.ToString() + "/" ;
			break ;
		case Language.TraditionalChinese :
			CONST_LanguagePrefix = Language.TraditionalChinese.ToString() + "/" ;
			break ;
		}		
	}
	
	public static AudioClip LoadAudio( string _Path )
	{
		AudioClip ret = null ;
		string commonpath = CONST_CommonPrefix + CONST_AudioPathPrefix + _Path ;
		ret = (AudioClip) Resources.Load( commonpath ) ;
		if( null == ret )
		{
			Debug.Log( "ResourceLoad :: LoadAudio() resource load failed. commonpath=" + commonpath ) ;
		}
		
		return ret ;
	}
	
	public static Texture LoadTexture( string _Path )
	{
		Texture ret = null ;
		string commonpath = CONST_CommonPrefix + CONST_TexturePathPrefix + _Path ;
		ret = (Texture) Resources.Load( commonpath ) ;
		if( null == ret )
		{
			Debug.Log( "ResourceLoad :: LoadTexture() resource load failed. commonpath=" + commonpath ) ;
		}
		return ret ;
	}		
	

	// 語言項未來會拉出去
	public static Object LoadPrefab( string _PrefabName )
	{
		return LoadPrefabByPrefix( CONST_CommonPrefix + CONST_PrefabPrefix , _PrefabName ) ;		
	}	
	
	public static TextAsset LoadDataToTextAsset( string _DataFilename , bool _Language )
	{
		string LanguagePrefix = ( true == _Language ) ? CONST_LanguagePrefix : CONST_CommonPrefix ;
		return LoadDataByPrefix( LanguagePrefix + CONST_DataPrefix , _DataFilename ) ;
	}

	/* load a resource with full path _PrefabName */
	private static Object LoadPrefabByPrefix( string _Prefix , string _PrefabName )
	{
		string fullpath = _Prefix + _PrefabName ;
		Object prefab = Resources.Load( fullpath ) ;
		if( null == prefab )
		{
			Debug.Log( "PrefabInstantiate:LoadPrefabByPrefix() null == prefab:" + fullpath ) ;
			return null ; 
		}
		return prefab ;
	}

	private static TextAsset LoadDataByPrefix( string _Prefix , string _DataFilename )
	{
		string fullpath = _Prefix + _DataFilename ;
#if DEBUG
		Debug.Log( "PrefabInstantiate:LoadDataByPrefix() fullpath=" + fullpath ) ;
#endif
		TextAsset textAsset = (TextAsset) Resources.Load( fullpath ) ;
		if( null == textAsset )
		{
			Debug.Log( "PrefabInstantiate:LoadDataByPrefix() null == textAsset:" + fullpath ) ;
			return null ; 
		}
		return textAsset ;
	}
	

		
}
