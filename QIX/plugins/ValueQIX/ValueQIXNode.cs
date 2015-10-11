#region usings
using System;
using System.ComponentModel.Composition;
using System.Linq;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "QIX", Category = "Value", Help = "Basic template with one value in/out", Tags = "")]
	#endregion PluginInfo
	public class ValueQIXNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Keyboard", DefaultValue = 1.0)]
		public ISpread<bool> FKey;
		
		[Input("doCut", DefaultValue = 1.0)]
		public ISpread<bool> FCut;
		
		[Input("FieldSizeX", DefaultValue = 1.0)]
		public ISpread<int> FFieldX;
		
		[Input("FieldSizeY", DefaultValue = 1.0)]
		public ISpread<int> FFieldY;
		
		[Input("Reset", DefaultValue = 1.0)]
		public ISpread<bool> FReset;
		
		
		[Output("Count")]
		public ISpread<double> FOutput;
		
		[Output("isCut")]
		public ISpread<bool> FisCut;
		
		[Output("Field")]
		public ISpread<int> FFdeb;
		
		[Output("Position")]
		public ISpread<Vector2D> FPos;
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins
		
		Vector2D nowPos;
		int[] field;
		int[] calcfield;
		int sizeX,sizeY;
		bool doCut;
		
		private int p2fi(Vector2D p){
			return (int)p.x + (int)p.y*sizeY;
		}
		
		private void reCalc(int i,int num){
			if(calcfield[i+1] == -1 || calcfield[i+1] == num){
				
			}
			else{
				calcfield[i+1] = num;
				reCalc(i+1,num);
			}
			
			if(calcfield[i-1] == -1 || calcfield[i-1] == num){
				
			}
			else{
				calcfield[i-1] = num;
				reCalc(i-1,num);
			}
			if(calcfield[i+sizeY] == -1 || calcfield[i+sizeY] == num){
				
			}
			else{
				calcfield[i+sizeY] = num;
				reCalc(i+sizeY,num);
			}
			
			if(calcfield[i-sizeY] == -1 || calcfield[i-sizeY] == num){
				
			}
			else{
				calcfield[i-sizeY] = num;
				reCalc(i-sizeY,num);
			}
			return;
		}
		
		private void checkCalcField(){
			int num = 1;
			int maxnum = 1;
			for(int j = 1; j <= sizeY-1; j++){
				num = 1;
				for(int i = 1; i < sizeX-1; i++){
					if(calcfield[i+j*sizeY] == -1){
						num++;
					}
					else{
						calcfield[i+j*sizeY] += num;
						if(calcfield[i+j*sizeY] > maxnum){maxnum = calcfield[i+j*sizeY];}
					}
				}
			}
			for(int i = 1; i < sizeX-1; i++){
				num = 1;
				for(int j = 1; j <= sizeY-1; j++){
					if(calcfield[i+j*sizeY] == -1){
						num++;
					}
					else{
						calcfield[i+j*sizeY] += num;
						if(calcfield[i+j*sizeY] > maxnum){maxnum = calcfield[i+j*sizeY];}
					}
				}
			}
			for(int i = 0; i < sizeX*sizeY; i++){
				if(calcfield[i] == maxnum){
					reCalc(i,maxnum);
				}
			}
			FLogger.Log(LogType.Debug, "CalcNow");
			int countA = 0;
			int countB = 0;
			for(int i = 0; i < sizeX*sizeY; i++){
				if(calcfield[i] == -1){continue;}
				if(calcfield[i] == maxnum){
					countB++;
				}
				else{
					countA++;
				}
			}
			if(countA <= countB){
				for(int i = 0; i < sizeX*sizeY; i++){
					if(calcfield[i] == maxnum){
						if(calcfield[i] != -1){
							calcfield[i] = 0;
						}
					}
					else{
						if(calcfield[i]!=-1){
							field[i] = 5;
						}
						calcfield[i] = -1;
					}
				}
			}
			else{
				for(int i = 0; i < sizeX*sizeY; i++){
					if(calcfield[i] == maxnum){
						if(calcfield[i] != -1){
							field[i] = 5;
						}
						calcfield[i] = -1;
					}
					else{
						if(calcfield[i] != -1){
							calcfield[i] = 0;
						}
					}
				}
			}
			for(int i = 0; i < sizeX*sizeY; i++){
				if(field[i] == 2){
					field[i] = 1;
				}
			}
		}
		
		
		private bool cutCalc(int px, int py){
			Vector2D tPos = nowPos;
			tPos.x += px;
			tPos.y += py;
			
			if(tPos.x < 0 || tPos.x >= sizeX || tPos.y < 0 || tPos.y >= sizeY){
				return false;
			}
			
			if(FCut[0]){
				doCut = true;
				if(field[p2fi(tPos)] == 0){
					return true;
				}
				if(doCut && field[p2fi(tPos)] == 1){
					checkCalcField();
					
					doCut = false;
					FLogger.Log(LogType.Debug, "CuttingEnd");
					return true;
				}
				if(field[p2fi(tPos)] == 2){
					return false;
				}
				return false;
			}
			
			else{
				if(field[p2fi(tPos)] == 1){
					return true;
				}
				else{
					return false;
				}
			}
			
		}
		
		private void KeyCtrl(){
			// Left
			if(FKey[0]){
				if(cutCalc(-1,0)){
					nowPos.x -= 1;
				}
			}
			// Down
			else if(FKey[1]){
				if(cutCalc(0,1)){
					nowPos.y += 1;
				}
			}
			// Up
			else if(FKey[2]){
				if(cutCalc(0,-1)){
					nowPos.y -= 1;
				}
			}
			// Right
			else if(FKey[3]){
				if(cutCalc(1,0)){
					nowPos.x += 1;
				}
			}
		}
		
		public void init(int x, int y){
			sizeX = x;
			sizeY = y;
			nowPos = new Vector2D(0,0);
			field = new int[x*y];
			calcfield = new int[x*y];
			for(int i = 0; i < x*y; i++){
				field[i] = 0;
			}
			for(int i = 0; i < x; i++){
				field[i] = 1;
				field[i*y] = 1;
				field[i*y+x-1] = 1;
				field[field.Length-1-i] = 1;
				
				calcfield[i]=-1;
				calcfield[i*y] = -1;
				calcfield[i*y+x-1] = -1;
				calcfield[field.Length-1-i] = -1;
			}
		}
		
		// field[] の番号
		// 1: 壁（あとから追加されたものもこれに）
		// 2: カット中
		// 3: 現在の位置
		// 4: （未使用）
		// 5: カット済み
		// =====================================================
		public void Evaluate(int SpreadMax)
		{
			if(FReset[0]){
				init(FFieldX[0],FFieldY[0]);
			}
			FisCut.SliceCount = 1;
			FisCut[0] = false;			
			
			if(doCut && FCut[0]){
				FisCut[0] = true;
				if(field[p2fi(nowPos)] != 1){
					field[p2fi(nowPos)] = 2;
					calcfield[p2fi(nowPos)] = -1;
				}
					KeyCtrl();					
			}
			else{
				KeyCtrl();
			}
			
			FOutput.SliceCount = 2;
			
			// 現在の位置は2を入れておく
			// field[p2fi()] = 2;
			int Ccount = 0;
			int Kabecount = 0;
			FFdeb.SliceCount = field.Length;
			FPos.SliceCount = 1;
			for(int i = 0; i < field.Length; i++){
				if(p2fi(nowPos) == i){
					FFdeb[i] = 3;
				}
				else{
					FFdeb[i] = field[i];
				}
				if(field[i] == 5){
					Ccount++;
				}
				if(field[i] == 1){
					Kabecount++;
				}
			}
			FOutput[0] = Ccount;
			FOutput[1] = sizeX*sizeY - Kabecount;
			FPos[0] = nowPos;
			
			//FLogger.Log(LogType.Debug, "hi tty!");
		}
	}
}
