using MagicVilla_WebApp.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MagicVilla_WebApp.Filters
{
	public class TokenExceptionRedirection() : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			if (context.Exception is TokenException)
			{
				context.Result = new RedirectToActionResult("LogIn", "Authentication", null);
			}
		}
	}
}
