/*
@file BackgroundObjInitialization.cs
@brief 背景初始化 
@author NDark

# 裝載在關卡場景物件上，用來定義場景的大小。
# 場景是用 plane 製作的 所以參數是以plane為客製化。
# 基本大小 : plane未縮放時的大小 5*5
# 基本縮放 : 我們縮放在xz軸向
# 寬度 = 基本大小 * 基本縮放
# 邊界大小 : 在xz軸向的邊界寬度
# 可移動範圍 = 寬度 - 邊界大小
## 以v0.9為標準，場景的可移動範圍是120*120
# IsOutofLevel() 判斷一個座標點是否超過邊界範圍
# 在 MainUpdate::UpdatePosition() 中 會依此 判斷船隻是否有超過可移動範圍

@date 20121108 by NDark . add comment.
@date 20121218 by NDark . add class method IsOutofLevel()
@date 20130111 by NDark . refactor and comment.

*/
using UnityEngine;

public class BackgroundObjInitialization : MonoBehaviour 
{
	public Vector3 ScaleInSize = new Vector3( 30 , 1 , 30 ) ;	// 放大這個plane
	public Vector3 BorderInSize = new Vector3( 30 , 1 , 30 ) ;	// 邊界的大小(縮放後)
	
	private Vector3 BasicSizeInIdentityScale = new Vector3( 5 , 1 , 5 ) ;// scale 1 時的plane大小
	private Vector3 WidthInSize = Vector3.zero ;	// 實際大小
	private Vector3 MovableSize = Vector3.zero ; // 最終可移動範圍 
	
	// 判斷一個座標點是否超過邊界範圍
	public bool IsOutofLevel( Vector3 _TestPos )
	{
		Vector3 objCenter = this.gameObject.transform.position ;
		return ( _TestPos.x > objCenter.x + this.MovableSize.x ||
				 _TestPos.x < objCenter.x - this.MovableSize.x ||
				 _TestPos.z > objCenter.z + this.MovableSize.z ||
				 _TestPos.z < objCenter.z - this.MovableSize.z ) ;
	}
	
	// Use this for initialization
	void Start () 
	{
		this.gameObject.transform.localScale = ScaleInSize ;
		
		WidthInSize.Set( BasicSizeInIdentityScale.x * ScaleInSize.x , 
						 BasicSizeInIdentityScale.y * ScaleInSize.y ,
						 BasicSizeInIdentityScale.z * ScaleInSize.z ) ;
		
		MovableSize = WidthInSize - BorderInSize ;
	}
	

	// Update is called once per frame
	void Update () 
	{
	}
}
