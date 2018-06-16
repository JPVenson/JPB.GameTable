using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPB.GameTable.UI.ViewModel.SubSpaces;
using JPB.WPFBase.MVVM.ViewModel;

namespace JPB.GameTable.Ui.Contracts.GameArea
{
	public interface IGameArea
	{
		int AppUserId { get; }
		ThreadSaveObservableCollection<SubSpaceBase> OpenSubSpaces { get; }
	}
}
