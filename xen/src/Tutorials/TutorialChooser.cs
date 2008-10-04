using System;
using System.Collections.Generic;

using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tutorials
{
	public partial class TutorialChooser : Form
	{
		bool canceled = true;

		public string SelectedTutorial
		{
			get { return (tutorialList.SelectedItem ?? "").ToString(); }
		}

		public bool Canceled
		{
			get { return canceled; }
		}

		public TutorialChooser(IEnumerable<string> tutorialNames, string startName)
		{
			Cursor.Show();

			InitializeComponent();

			int selectIndex = 0;
			int i = 0;

			foreach (string str in tutorialNames)
			{
				i++;
				if (startName == str)
					selectIndex = i;

				tutorialList.Items.Add(str);
			}

			if (tutorialList.Items.Count > 0)
				tutorialList.SelectedIndex = Math.Min(tutorialList.Items.Count-1,selectIndex);
		}

		private void OK(object sender, EventArgs e)
		{
			Cursor.Hide();

			canceled = false;
			Close();
		}

		private void Cancel(object sender, EventArgs e)
		{
			Close();
		}

		public static Type ChooseTutorial()
		{
			SortedList<string, Type> tutorials = new SortedList<string, Type>();
			FindTutorials(tutorials);

			TutorialChooser chooser = new TutorialChooser(tutorials.Keys, chosenTutorial);
			chooser.ShowDialog();

			if (chooser.Canceled)
				return null;

			Type type;
			tutorials.TryGetValue(chooser.SelectedTutorial, out type);

			chosenTutorial = chooser.SelectedTutorial;
			return type;
		}

		static string chosenTutorial;

		static void FindTutorials(SortedList<string, Type> types)
		{
			foreach (Type type in typeof(Program).Assembly.GetExportedTypes())
			{
				if (typeof(Xen.Application).IsAssignableFrom(type))
				{
					string name = type.FullName;
					foreach (object attribute in type.GetCustomAttributes(typeof(DisplayNameAttribute), true))
						name = ((DisplayNameAttribute)attribute).Name;

					types.Add(name, type);
				}
			}
		}
	}


}