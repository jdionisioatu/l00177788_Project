﻿using CodeScanning.Controllers;
using CodeScanning.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CodeScanning.ViewModels
{
	public class SettingsFormViewModel : BaseEntityFormViewModel<Settings>
	{
		public Settings Settings
		{
			get { return Entity;  }
			set { Entity = value; }
		}

		public string ActionName => IsUpdate ? nameof(SettingsController.Update) : nameof(SettingsController.Create);

        public SettingsFormViewModel()
		{
			Settings = new Settings();

		}
	}
}
