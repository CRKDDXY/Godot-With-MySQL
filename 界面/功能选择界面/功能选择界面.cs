using Godot;
using System;
using System.Collections.Generic;

public partial class 功能选择界面 : Node2D{
	[Export] public Button 数据库账号管理;
	[Export] public Button 数据库社团管理;
	[Export] public Button 活动审核;
	[Export] public Button 申请活动;
	[Export] public Button 管理社团成员;
	[Export] public Button 创建社团;
	[Export] public Button 参报活动;
	[Export] public Button 个人社团管理;
	[Export] public Button 切换账号;
	[Export] public Button 操作记录查看;
	public int 权限等级; 
	public override void _Ready(){
		数据库账号管理.Visible = true;
		数据库社团管理.Visible = true;
		活动审核.Visible = true;
		申请活动.Visible = false;
		管理社团成员.Visible = true;
		创建社团.Visible = true;
		参报活动.Visible = false;
		个人社团管理.Visible = false;	//管理员账号固定每个社团都有最高权限
		切换账号.Visible = true;
		操作记录查看.Visible = true;
		if(全局脚本.level == "2"){
			数据库账号管理.Visible = false;
			数据库社团管理.Visible = false;
			活动审核.Visible = false;
			申请活动.Visible = false;
			管理社团成员.Visible = false;
			创建社团.Visible = false;
			个人社团管理.Visible = true;
			参报活动.Visible = true;
			操作记录查看.Visible = false;
		}
		if(全局脚本.level == "1"){
			管理社团成员.Visible = false;
			数据库账号管理.Visible = false;
			数据库社团管理.Visible = false;
			活动审核.Visible = false;
			创建社团.Visible = false;
			个人社团管理.Visible = true;
			参报活动.Visible = true;
			申请活动.Visible = true;
			操作记录查看.Visible = false;
		}
		GetWindow().SetFlag(Godot.Window.Flags.ResizeDisabled,true);    //设置窗口类型（不可缩放）
		//设置权限
		切换账号.Pressed +=()=> 返回登录页面();
		数据库账号管理.Pressed +=()=> 查看_数据库账号管理();
		创建社团.Pressed +=()=> 跳转_创建社团();
		数据库社团管理.Pressed +=()=> 跳转_数据库社团信息管理();
		个人社团管理.Pressed +=()=> 跳转_个人社团管理();
		管理社团成员.Pressed +=()=> 跳转_管理社团成员();
		申请活动.Pressed +=()=> 跳转_申请活动();
		活动审核.Pressed +=()=> 跳转_活动审核();
		参报活动.Pressed +=()=> 跳转_参加活动();
		操作记录查看.Pressed +=()=> 跳转_操作记录();
	}
	private void 查看_数据库账号管理(){
		GetTree().ChangeSceneToFile("res://界面/数据库账号管理/数据库账号管理.tscn");
	}
	private void 返回登录页面(){
		GetTree().ChangeSceneToFile("res://界面/登录界面/登录界面.tscn");
	}
	private void 跳转_数据库社团信息管理(){
		GetTree().ChangeSceneToFile("res://界面/数据库社团管理/数据库社团管理.tscn");
	}
	private void 跳转_创建社团(){
		GetTree().ChangeSceneToFile("res://界面/创建社团/创建社团.tscn");
	}
	private void 跳转_个人社团管理(){
		GetTree().ChangeSceneToFile("res://界面/个人社团管理/个人社团管理.tscn");
	}
	private void 跳转_管理社团成员(){
		GetTree().ChangeSceneToFile("res://界面/管理社团成员/管理社团成员.tscn");
	}
	private void 跳转_申请活动(){
		GetTree().ChangeSceneToFile("res://界面/申请活动/申请活动.tscn");
	}
	private void 跳转_活动审核(){
		GetTree().ChangeSceneToFile("res://界面/活动审核/活动审核.tscn");
	}
	private void 跳转_参加活动(){
		GetTree().ChangeSceneToFile("res://界面/参加活动/参加活动.tscn");
	}
	private void 跳转_操作记录(){
		GetTree().ChangeSceneToFile("res://界面/操作记录查看/操作记录查看.tscn");
	}
}