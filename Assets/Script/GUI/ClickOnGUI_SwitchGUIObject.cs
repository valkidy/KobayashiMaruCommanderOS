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
@file ClickOnGUI_SwitchGUIObject.cs
@author NDark

按下時切換另一個GUI物件。
# m_ControlGUIObjectName 控制的另一個GUI物件名稱

@date 20121210 file started.
@date 20121210 by NDark . add class member m_ControlGUIObjectName
@date 20121213 by NDark . add enable checking at OnMouseDown()
@date 20121219 by NDark . add class method SetNotToMoveOfMainCharacter()
@date 20121225 by NDark . replace switch by SwitchAndItsAllChildren() at OnMouseDown()
@date 20130112 by NDark . remove class method SetNotToMoveOfMainCharacter()
@date 20130114 by NDark . fix a checking error at OnMouseDown()

*/
// #define DEBUG

using UnityEngine;

public class ClickOnGUI_SwitchGUIObject : MonoBehaviour 
{
	public string m_ControlGUIObjectName = "" ;// 控制的GUI物件名稱
	
	private NamedObject m_ControlGUIObject = new NamedObject() ;
	
	public void Setup( string _ControlGUIObjectName ) 
	{

		m_ControlGUIObject.Name = _ControlGUIObjectName ;
	}
	
	// Use this for initialization
	void Start () 
	{
		if( 0 != m_ControlGUIObjectName.Length )
			Setup( m_ControlGUIObjectName ) ;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	

	
	void OnMouseDown()
	{

		if( false == this.enabled )
			return ;
		
		if( 0 == m_ControlGUIObject.Name.Length )
			return ;
	
		GlobalSingleton.TellMainCharacterNotToTriggerOtherClick() ;
		
		if( null != m_ControlGUIObject.Obj )
		{
#if DEBUG			
			Debug.Log( "ClickOnGUI_SwitchGUIObject::OnMouseDown() m_ControlGUIObject.Name" + m_ControlGUIObject.Name ) ;
#endif
			ShowGUITexture.Switch( m_ControlGUIObject.Obj , true , true ) ;			
		}
	}
}
