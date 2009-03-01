
#if XBOX360

//On XBOX, set this using statement to the tutorial you would like to run:

//Modify the next lines to set the example that will run
//Tutorial_01 to Tutorial_24

//this next line can be commented out:
#warning Please select a default tutorial for the xbox project (set on the next line):

using Tutorials.Tutorial_XX;

#endif

using System;
using System.Collections.Generic;
using Xen;



namespace Tutorials
{
	static class Program
	{
		static void Main(string[] args)
		{
#if XBOX360
			using (Tutorial game = new Tutorial())
			    game.Run();
#else
			while (true)
			{
				bool runInWinForms;

				Type type = TutorialChooser.ChooseTutorial(out runInWinForms);
				if (type == null)
					break;

				using (Application tutorial = Activator.CreateInstance(type) as Application)
				{
					if (runInWinForms)
					{
						WinFormsExample form = new WinFormsExample();
						tutorial.Run(form.XenWinFormsHostControl);

						System.Windows.Forms.Application.Run(form);
					}
					else
						tutorial.Run();
				}
			}
#endif
		}
	}


	public class DisplayNameAttribute : Attribute
	{
		private string name;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}

