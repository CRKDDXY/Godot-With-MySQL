using Godot;
using System;

public partial class 指令显示窗口 : Window{
	[Export] public Label 指令提示;
	public override void _Ready(){
		指令提示.Text = "上次执行的指令：\n";
		foreach (string item in 全局脚本.上次使用的指令){
			指令提示.Text += item;
			指令提示.Text += "\n";
		}
	}
}