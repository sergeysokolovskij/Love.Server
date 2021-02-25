using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Services.Reports.Base
{
	public interface IReportBuilder
	{
		Task BuildReportZip();
	}
	public class ReportBuilder : IReportBuilder
	{
		public async Task BuildReportZip()
		{

		}
	}
}
