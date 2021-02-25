using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Dal
{
	public interface IBaseEntity
	{
		DateTime Created { get; set; }
		DateTime Updated { get; set; }
	}
}
