#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

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
		[Input("X", DefaultValue = 0.0)]
		ISpread<float> FX;
		[Input("Y", DefaultValue = 0.0)]
		ISpread<float> FY;
		[Input("Radius", DefaultValue = 1.0)]
		ISpread<float> FRadius;
		[Input("Level", DefaultValue = 6.0)]
		ISpread<float> FLevel;

		[Output("Output")]
		ISpread<double> FOutput;

		[Import()]
		ILogger FLogger;
		#endregion fields & pins


		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = SpreadMax;

			for (int i = 0; i < SpreadMax; i++)
			{
				FOutput[i] = drawBranch(FX[i], FY[i],FRadius[i], FLevel[i]);
			}

		}
		private float drawBranch(ISpread<float> x, ISpread<float> y, ISpread<float> radius, ISpread<float> level){
			ISpread<float> answer = x+y+radius+level;	
			//if (level > 0) {			
				//drawBranch (x-radius, y-radius/2, radius/2, level-1);
				//drawBranch (x+radius, y-radius/2, radius/2, level-1);
			//}			
			return answer;
		}
	}
}
