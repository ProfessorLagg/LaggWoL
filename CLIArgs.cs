using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace LaggWoL {
		public sealed class CLIArgs {
				public PhysicalAddress MAC { get; private set; }

				public CLIArgs(IList<string> args) {

				}
		}
}
