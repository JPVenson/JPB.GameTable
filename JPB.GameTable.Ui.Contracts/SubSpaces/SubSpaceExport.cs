using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPB.GameTable.Ui.Contracts.SubSpaces
{
	public class SubSpaceExportAttribute : Attribute
	{
	}

	public interface ISubSpaceExport
	{
		void OnStart();
	}
}
