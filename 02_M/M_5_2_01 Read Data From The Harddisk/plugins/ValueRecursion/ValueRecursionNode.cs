#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using System.Collections.Generic;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "Recursion", Category = "Value", Help = "Basic template with one value in/out", Tags = "")]
	#endregion PluginInfo
	public class ValueRecursionNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("X", DefaultValue = 0.0, IsSingle = true)]
		ISpread<double> FX;
		[Input("Y", DefaultValue = 0.0, IsSingle = true)]
		ISpread<double> FY;
		[Input("Radius", DefaultValue = 1.0, IsSingle = true)]
		ISpread<double> FRadius;
		[Input("Level", DefaultValue = 6.0, IsSingle = true)]
		ISpread<double> FLevel;

		[Output("XY")]
		ISpread<Vector2D> FOutput1;
		[Output("Radius")]
		ISpread<double> FOutput2;
		[Output("Level")]
		ISpread<double> FOutput3;

		[Import()]
		ILogger FLogger;
		
		List<Vector2D> FArcXY =  new List<Vector2D>();
		List<double> FArcRadius =  new List<double>();
		List<double> FArcLevel =  new List<double>();
		#endregion fields & pins
		


		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FArcXY.Clear();
			FArcRadius.Clear();
			FArcLevel.Clear();
			drawBranch(FX[0], FY[0],FRadius[0], FLevel[0]);
			FOutput1.AssignFrom(FArcXY);
			FOutput2.AssignFrom(FArcRadius);
			FOutput3.AssignFrom(FArcLevel);

		}
		private void drawBranch(double x, double y, double radius, double level){
			FArcXY.Add(new Vector2D(x,y));
			FArcRadius.Add(radius);
			FArcLevel.Add(level);
			if (level > 0) {			
				drawBranch (x-radius, y-radius/2, radius/2, level-1);
				drawBranch (x+radius, y-radius/2, radius/2, level-1);
			}			
		}
	}
}
