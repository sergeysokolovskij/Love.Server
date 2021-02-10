using Api.Services.Cache;
using Api.Services.Email;
using Api.Services.Email.Templates;
using Api.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared;
using ShopPlatform.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ShopPlatforms.Core.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class RequestController : BaseController
	{
		private readonly IEmailSenderService emailSenderService;
		private readonly IConfiguration configuration;
		public RequestController(IServiceProvider serviceProvider, 
			IEmailSenderService emailSenderService,
			IConfiguration configuration) : base(serviceProvider)
		{
			this.emailSenderService = emailSenderService;
			this.configuration = configuration;
		}
		
		[HttpPost("sendrequest")]
		public async Task<IActionResult> SendRequestAsync([FromForm]RequestModel requestModel)
		{
			var requiredFields = new List<string> {"files", "email" };
			var dictionary = new Dictionary<string, string>();
			foreach (var field in Request.Form)
			{
				if (requiredFields.Contains(field.Key))
					continue;
				dictionary.Add(field.Key, field.Value);
			}
			requestModel.CustomFields = dictionary;

			if (requestModel.Files.IsListNotNull())
			{
				var directoryPath = Path.Combine(Environment.CurrentDirectory, "TempFiles");
				int maxFileSizeInBytes = Convert.ToInt32(configuration["maxFileSize"]);

				int count = 1;

				var pathes = new List<string>();

				foreach (var file in requestModel.Files)
				{
					var photoType = file.FileName.Contains("png") ? ".png" : ".jpg";

					if (file.Length > maxFileSizeInBytes)
						continue;

					var fullPath = Path.Combine(directoryPath, string.Format("temp{0}{1}", count, photoType));
					if (System.IO.File.Exists(fullPath))
						System.IO.File.Delete(fullPath);
					await using (var fileStream = new FileStream(fullPath, FileMode.Create))
					{
						await file.CopyToAsync(fileStream);
					}

					pathes.Add(fullPath);
					count++;
				}
				await emailSenderService.SendEmailAsync(requestModel, requestModel.Email, MailTempleteNames.RequestMailTemplateName, pathes);
			}
			else
				await emailSenderService.SendEmailAsync(requestModel, requestModel.Email, MailTempleteNames.RequestMailTemplateName);
			return Ok();
		}
	}
}
