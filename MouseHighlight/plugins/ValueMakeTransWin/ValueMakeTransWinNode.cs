#region usings
using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Drawing;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "MakeTransWin", Category = "Value", Help = "Basic template with one value in/out", Tags = "")]
	#endregion PluginInfo
	public class ValueMakeTransWinNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Position", DefaultValue = 1.0)]
		public ISpread<Vector2D> FPos;

		[Input("Size", DefaultValue = 1.0)]
		public ISpread<double> FSize;
		
		[Input("Enable", DefaultValue = 1.0)]
		public ISpread<bool> FEnable;
		
		[Input("Timing", DefaultValue = 0.0)]
		public ISpread<double> FTim;
		
		[Output("Output")]
		public ISpread<double> FOutput;
		
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins
		
		bool flag = false;
		Form Main = new Form();
		Form Main2 = new Form();
		Graphics g;
		Pen redPen = new Pen(Color.Red, 4);
		Pen yelPen = new Pen(Color.Yellow, 10);
		Pen whitePen = new Pen(Color.White, 2);
		Rectangle rect1 = new Rectangle(30, 30, 60, 60);
		
		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if (!flag){
				Main.FormBorderStyle = FormBorderStyle.None;
				Main.TopMost = true;
				Main.Show();
				Main.Text = "Main";
				flag = true;
				FLogger.Log(LogType.Debug, "hi tty!");
				Main.BackColor =  Color.White;
				Main.TransparencyKey = Color.White;
				g = Main.CreateGraphics();
			}
			if(FEnable[0]){
				Main.Show();
				Main.Location = new Point((int)FPos[0].x-60,(int)FPos[0].y-60 );
				Main.Size = new Size(100, 100);
			}else{
				Main.Hide();
			}
			FOutput.SliceCount = SpreadMax;			
			g.FillRectangle(Brushes.White, 0,0,120,120);
		
			if(FTim[0] == 1){
				g.DrawEllipse(yelPen, rect1);
			}else{
				g.DrawEllipse(redPen, rect1);
			}
		}
	}
}
