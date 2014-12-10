﻿using System;
using Autofac;
using Jabberwocky.Glass.Autofac.Extensions;
using Jabberwocky.Glass.Autofac.Util;
using Sitecore.Reflection;

namespace Jabberwocky.Glass.Autofac.Pipelines.Factories
{
	/// <summary>
	/// A Sitecore Factory adapter for Autofac 
	/// </summary>
	public class AutofacProcessorFactory : IFactory
	{
		protected static IContainer Container { get { return AutofacConfig.ServiceLocator; } }

		public virtual object GetObject(string identifier)
		{
			var type = ResolveType(identifier);
			if (type == null) return null;

			// Includes Pipeline specific registrations that override existing defaults
			var scope = Container.BeginLifetimeScope(builder => builder.RegisterSitecorePipelineServices());
			try
			{
				return scope.Resolve(type);
			}
			catch
			{
				// If an error occurs during service resolution, but we have already created a lifetime scope, dispose it!
				scope.Dispose();
			}

			return null;
		}

		private static Type ResolveType(string identifier)
		{
			return identifier == null ? null : Type.GetType(identifier, false, true);
		}
	}
}