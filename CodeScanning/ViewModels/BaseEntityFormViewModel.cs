using CodeScanning.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using CodeScanning.Controllers;

namespace CodeScanning.ViewModels
{
	public class BaseEntityFormViewModel<T> where T : IEntityWithId
	{
		protected T Entity { get; set; }

		public bool IsUpdate => Entity.Id > 0;


    }
}
