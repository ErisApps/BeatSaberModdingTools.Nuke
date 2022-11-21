using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

namespace Helpers;

public static class DotNetNugetUpdateSourceTask
{
	/// <summary>
	///   <p>Adds a NuGet source.</p>
	///   <p>For more details, visit the <a href="https://docs.microsoft.com/en-us/dotnet/core/tools/">official website</a>.</p>
	/// </summary>
	/// <remarks>
	///   <p>This is a <a href="http://www.nuke.build/docs/authoring-builds/cli-tools.html#fluent-apis">CLI wrapper with fluent API</a> that allows to modify the following arguments:</p>
	///   <ul>
	///     <li><c>&lt;name&gt;</c> via <see cref="DotNetNuGetUpdateSourceSettings.Name"/></li>
	///     <li><c>--source</c> via <see cref="DotNetNuGetUpdateSourceSettings.Source"/></li>
	///     <li><c>--password</c> via <see cref="DotNetNuGetUpdateSourceSettings.Password"/></li>
	///     <li><c>--store-password-in-clear-text</c> via <see cref="DotNetNuGetUpdateSourceSettings.StorePasswordInClearText"/></li>
	///     <li><c>--username</c> via <see cref="DotNetNuGetUpdateSourceSettings.Username"/></li>
	///     <li><c>--valid-authentication-types</c> via <see cref="DotNetNuGetUpdateSourceSettings.ValidAuthenticationTypes"/></li>
	///   </ul>
	/// </remarks>
	public static IReadOnlyCollection<Output> DotNetNuGetUpdateSource(DotNetNuGetUpdateSourceSettings toolSettings = null)
	{
		toolSettings ??= new DotNetNuGetUpdateSourceSettings();
		using var process = ProcessTasks.StartProcess(toolSettings);
		process.AssertZeroExitCode();
		return process.Output;
	}

	/// <summary>
	///   <p>Adds a NuGet source.</p>
	///   <p>For more details, visit the <a href="https://docs.microsoft.com/en-us/dotnet/core/tools/">official website</a>.</p>
	/// </summary>
	/// <remarks>
	///   <p>This is a <a href="http://www.nuke.build/docs/authoring-builds/cli-tools.html#fluent-apis">CLI wrapper with fluent API</a> that allows to modify the following arguments:</p>
	///   <ul>
	///     <li><c>&lt;name&gt;</c> via <see cref="DotNetNuGetUpdateSourceSettings.Name"/></li>
	///     <li><c>--source</c> via <see cref="DotNetNuGetUpdateSourceSettings.Source"/></li>
	///     <li><c>--password</c> via <see cref="DotNetNuGetUpdateSourceSettings.Password"/></li>
	///     <li><c>--store-password-in-clear-text</c> via <see cref="DotNetNuGetUpdateSourceSettings.StorePasswordInClearText"/></li>
	///     <li><c>--username</c> via <see cref="DotNetNuGetUpdateSourceSettings.Username"/></li>
	///     <li><c>--valid-authentication-types</c> via <see cref="DotNetNuGetUpdateSourceSettings.ValidAuthenticationTypes"/></li>
	///   </ul>
	/// </remarks>
	public static IReadOnlyCollection<Output> DotNetNuGetUpdateSource(Configure<DotNetNuGetUpdateSourceSettings> configurator)
	{
		return DotNetNuGetUpdateSource(configurator(new DotNetNuGetUpdateSourceSettings()));
	}

	/// <summary>
	///   <p>Adds a NuGet source.</p>
	///   <p>For more details, visit the <a href="https://docs.microsoft.com/en-us/dotnet/core/tools/">official website</a>.</p>
	/// </summary>
	/// <remarks>
	///   <p>This is a <a href="http://www.nuke.build/docs/authoring-builds/cli-tools.html#fluent-apis">CLI wrapper with fluent API</a> that allows to modify the following arguments:</p>
	///   <ul>
	///     <li><c>&lt;name&gt;</c> via <see cref="DotNetNuGetUpdateSourceSettings.Name"/></li>
	///     <li><c>--source</c> via <see cref="DotNetNuGetUpdateSourceSettings.Source"/></li>
	///     <li><c>--password</c> via <see cref="DotNetNuGetUpdateSourceSettings.Password"/></li>
	///     <li><c>--store-password-in-clear-text</c> via <see cref="DotNetNuGetUpdateSourceSettings.StorePasswordInClearText"/></li>
	///     <li><c>--username</c> via <see cref="DotNetNuGetUpdateSourceSettings.Username"/></li>
	///     <li><c>--valid-authentication-types</c> via <see cref="DotNetNuGetUpdateSourceSettings.ValidAuthenticationTypes"/></li>
	///   </ul>
	/// </remarks>
	public static IEnumerable<(DotNetNuGetUpdateSourceSettings Settings, IReadOnlyCollection<Output> Output)> DotNetNuGetUpdateSource(
		CombinatorialConfigure<DotNetNuGetUpdateSourceSettings> configurator, int degreeOfParallelism = 1, bool completeOnFailure = false)
	{
		return configurator.Invoke(DotNetNuGetUpdateSource, DotNetTasks.DotNetLogger, degreeOfParallelism, completeOnFailure);
	}

	public partial class DotNetNuGetUpdateSourceSettings : ToolSettings
	{
		/// <summary>
		///   Path to the DotNet executable.
		/// </summary>
		public override string ProcessToolPath => DotNetTasks.DotNetPath;

		public override Action<OutputType, string> ProcessCustomLogger => DotNetTasks.DotNetLogger;

		/// <summary>
		///   Name of the source.
		/// </summary>
		public virtual string Name { get; internal set; }

		/// <summary>
		///   URL of the source.
		/// </summary>
		public virtual string Source { get; internal set; }

		/// <summary>
		///   Username to be used when connecting to an authenticated source.
		/// </summary>
		public virtual string Username { get; internal set; }

		/// <summary>
		///   Password to be used when connecting to an authenticated source.
		/// </summary>
		public virtual string Password { get; internal set; }

		/// <summary>
		///   Enables storing portable package source credentials by disabling password encryption.
		/// </summary>
		public virtual bool? StorePasswordInClearText { get; internal set; }

		/// <summary>
		///   List of valid authentication types for this source. Set this to <c>basic</c> if the server advertises NTLM or Negotiate and your credentials must be sent using the Basic mechanism, for instance when using a PAT with on-premises Azure DevOps Server. Other valid values include <c>negotiate</c>, <c>kerberos</c>, <c>ntlm</c>, and <c>digest</c>, but these values are unlikely to be useful.
		/// </summary>
		public virtual IReadOnlyList<DotNetNuGetAuthentication> ValidAuthenticationTypes => ValidAuthenticationTypesInternal.AsReadOnly();

		internal List<DotNetNuGetAuthentication> ValidAuthenticationTypesInternal { get; set; } = new List<DotNetNuGetAuthentication>();

		protected override Arguments ConfigureProcessArguments(Arguments arguments)
		{
			arguments
				.Add("nuget update source")
				.Add("{value}", Name)
				.Add("--source {value}", Source)
				.Add("--username {value}", Username)
				.Add("--password {value}", Password, secret: true)
				.Add("--store-password-in-clear-text", StorePasswordInClearText)
				.Add("--valid-authentication-types", ValidAuthenticationTypes, separator: ',');
			return base.ConfigureProcessArguments(arguments);
		}
	}
}

#region DotNetNuGetUpdateSourceSettingsExtensions

/// <summary>
///   Used within <see cref="DotNetNugetUpdateSourceTask"/>.
/// </summary>
[PublicAPI]
[ExcludeFromCodeCoverage]
public static partial class DotNetNuGetUpdateSourceSettingsExtensions
{
	#region Name

	/// <summary>
	///   <p><em>Sets <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.Name"/></em></p>
	///   <p>Name of the source.</p>
	/// </summary>
	[Pure]
	public static T SetName<T>(this T toolSettings, string name) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.Name = name;
		return toolSettings;
	}

	/// <summary>
	///   <p><em>Resets <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.Name"/></em></p>
	///   <p>Name of the source.</p>
	/// </summary>
	[Pure]
	public static T ResetName<T>(this T toolSettings) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.Name = null;
		return toolSettings;
	}

	#endregion

	#region Source

	/// <summary>
	///   <p><em>Sets <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.Source"/></em></p>
	///   <p>URL of the source.</p>
	/// </summary>
	[Pure]
	public static T SetSource<T>(this T toolSettings, string source) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.Source = source;
		return toolSettings;
	}

	/// <summary>
	///   <p><em>Resets <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.Source"/></em></p>
	///   <p>URL of the source.</p>
	/// </summary>
	[Pure]
	public static T ResetSource<T>(this T toolSettings) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.Source = null;
		return toolSettings;
	}

	#endregion

	#region Username

	/// <summary>
	///   <p><em>Sets <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.Username"/></em></p>
	///   <p>Username to be used when connecting to an authenticated source.</p>
	/// </summary>
	[Pure]
	public static T SetUsername<T>(this T toolSettings, string username) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.Username = username;
		return toolSettings;
	}

	/// <summary>
	///   <p><em>Resets <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.Username"/></em></p>
	///   <p>Username to be used when connecting to an authenticated source.</p>
	/// </summary>
	[Pure]
	public static T ResetUsername<T>(this T toolSettings) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.Username = null;
		return toolSettings;
	}

	#endregion

	#region Password

	/// <summary>
	///   <p><em>Sets <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.Password"/></em></p>
	///   <p>Password to be used when connecting to an authenticated source.</p>
	/// </summary>
	[Pure]
	public static T SetPassword<T>(this T toolSettings, [Secret] string password) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.Password = password;
		return toolSettings;
	}

	/// <summary>
	///   <p><em>Resets <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.Password"/></em></p>
	///   <p>Password to be used when connecting to an authenticated source.</p>
	/// </summary>
	[Pure]
	public static T ResetPassword<T>(this T toolSettings) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.Password = null;
		return toolSettings;
	}

	#endregion

	#region StorePasswordInClearText

	/// <summary>
	///   <p><em>Sets <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.StorePasswordInClearText"/></em></p>
	///   <p>Enables storing portable package source credentials by disabling password encryption.</p>
	/// </summary>
	[Pure]
	public static T SetStorePasswordInClearText<T>(this T toolSettings, bool? storePasswordInClearText) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.StorePasswordInClearText = storePasswordInClearText;
		return toolSettings;
	}

	/// <summary>
	///   <p><em>Resets <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.StorePasswordInClearText"/></em></p>
	///   <p>Enables storing portable package source credentials by disabling password encryption.</p>
	/// </summary>
	[Pure]
	public static T ResetStorePasswordInClearText<T>(this T toolSettings) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.StorePasswordInClearText = null;
		return toolSettings;
	}

	/// <summary>
	///   <p><em>Enables <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.StorePasswordInClearText"/></em></p>
	///   <p>Enables storing portable package source credentials by disabling password encryption.</p>
	/// </summary>
	[Pure]
	public static T EnableStorePasswordInClearText<T>(this T toolSettings) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.StorePasswordInClearText = true;
		return toolSettings;
	}

	/// <summary>
	///   <p><em>Disables <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.StorePasswordInClearText"/></em></p>
	///   <p>Enables storing portable package source credentials by disabling password encryption.</p>
	/// </summary>
	[Pure]
	public static T DisableStorePasswordInClearText<T>(this T toolSettings) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.StorePasswordInClearText = false;
		return toolSettings;
	}

	/// <summary>
	///   <p><em>Toggles <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.StorePasswordInClearText"/></em></p>
	///   <p>Enables storing portable package source credentials by disabling password encryption.</p>
	/// </summary>
	[Pure]
	public static T ToggleStorePasswordInClearText<T>(this T toolSettings) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.StorePasswordInClearText = !toolSettings.StorePasswordInClearText;
		return toolSettings;
	}

	#endregion

	#region ValidAuthenticationTypes

	/// <summary>
	///   <p><em>Sets <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.ValidAuthenticationTypes"/> to a new list</em></p>
	///   <p>List of valid authentication types for this source. Set this to <c>basic</c> if the server advertises NTLM or Negotiate and your credentials must be sent using the Basic mechanism, for instance when using a PAT with on-premises Azure DevOps Server. Other valid values include <c>negotiate</c>, <c>kerberos</c>, <c>ntlm</c>, and <c>digest</c>, but these values are unlikely to be useful.</p>
	/// </summary>
	[Pure]
	public static T SetValidAuthenticationTypes<T>(this T toolSettings, params DotNetNuGetAuthentication[] validAuthenticationTypes) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.ValidAuthenticationTypesInternal = validAuthenticationTypes.ToList();
		return toolSettings;
	}

	/// <summary>
	///   <p><em>Sets <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.ValidAuthenticationTypes"/> to a new list</em></p>
	///   <p>List of valid authentication types for this source. Set this to <c>basic</c> if the server advertises NTLM or Negotiate and your credentials must be sent using the Basic mechanism, for instance when using a PAT with on-premises Azure DevOps Server. Other valid values include <c>negotiate</c>, <c>kerberos</c>, <c>ntlm</c>, and <c>digest</c>, but these values are unlikely to be useful.</p>
	/// </summary>
	[Pure]
	public static T SetValidAuthenticationTypes<T>(this T toolSettings, IEnumerable<DotNetNuGetAuthentication> validAuthenticationTypes) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.ValidAuthenticationTypesInternal = validAuthenticationTypes.ToList();
		return toolSettings;
	}

	/// <summary>
	///   <p><em>Adds values to <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.ValidAuthenticationTypes"/></em></p>
	///   <p>List of valid authentication types for this source. Set this to <c>basic</c> if the server advertises NTLM or Negotiate and your credentials must be sent using the Basic mechanism, for instance when using a PAT with on-premises Azure DevOps Server. Other valid values include <c>negotiate</c>, <c>kerberos</c>, <c>ntlm</c>, and <c>digest</c>, but these values are unlikely to be useful.</p>
	/// </summary>
	[Pure]
	public static T AddValidAuthenticationTypes<T>(this T toolSettings, params DotNetNuGetAuthentication[] validAuthenticationTypes) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.ValidAuthenticationTypesInternal.AddRange(validAuthenticationTypes);
		return toolSettings;
	}

	/// <summary>
	///   <p><em>Adds values to <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.ValidAuthenticationTypes"/></em></p>
	///   <p>List of valid authentication types for this source. Set this to <c>basic</c> if the server advertises NTLM or Negotiate and your credentials must be sent using the Basic mechanism, for instance when using a PAT with on-premises Azure DevOps Server. Other valid values include <c>negotiate</c>, <c>kerberos</c>, <c>ntlm</c>, and <c>digest</c>, but these values are unlikely to be useful.</p>
	/// </summary>
	[Pure]
	public static T AddValidAuthenticationTypes<T>(this T toolSettings, IEnumerable<DotNetNuGetAuthentication> validAuthenticationTypes) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.ValidAuthenticationTypesInternal.AddRange(validAuthenticationTypes);
		return toolSettings;
	}

	/// <summary>
	///   <p><em>Clears <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.ValidAuthenticationTypes"/></em></p>
	///   <p>List of valid authentication types for this source. Set this to <c>basic</c> if the server advertises NTLM or Negotiate and your credentials must be sent using the Basic mechanism, for instance when using a PAT with on-premises Azure DevOps Server. Other valid values include <c>negotiate</c>, <c>kerberos</c>, <c>ntlm</c>, and <c>digest</c>, but these values are unlikely to be useful.</p>
	/// </summary>
	[Pure]
	public static T ClearValidAuthenticationTypes<T>(this T toolSettings) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		toolSettings.ValidAuthenticationTypesInternal.Clear();
		return toolSettings;
	}

	/// <summary>
	///   <p><em>Removes values from <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.ValidAuthenticationTypes"/></em></p>
	///   <p>List of valid authentication types for this source. Set this to <c>basic</c> if the server advertises NTLM or Negotiate and your credentials must be sent using the Basic mechanism, for instance when using a PAT with on-premises Azure DevOps Server. Other valid values include <c>negotiate</c>, <c>kerberos</c>, <c>ntlm</c>, and <c>digest</c>, but these values are unlikely to be useful.</p>
	/// </summary>
	[Pure]
	public static T RemoveValidAuthenticationTypes<T>(this T toolSettings, params DotNetNuGetAuthentication[] validAuthenticationTypes) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		var hashSet = new HashSet<DotNetNuGetAuthentication>(validAuthenticationTypes);
		toolSettings.ValidAuthenticationTypesInternal.RemoveAll(x => hashSet.Contains(x));
		return toolSettings;
	}

	/// <summary>
	///   <p><em>Removes values from <see cref="DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings.ValidAuthenticationTypes"/></em></p>
	///   <p>List of valid authentication types for this source. Set this to <c>basic</c> if the server advertises NTLM or Negotiate and your credentials must be sent using the Basic mechanism, for instance when using a PAT with on-premises Azure DevOps Server. Other valid values include <c>negotiate</c>, <c>kerberos</c>, <c>ntlm</c>, and <c>digest</c>, but these values are unlikely to be useful.</p>
	/// </summary>
	[Pure]
	public static T RemoveValidAuthenticationTypes<T>(this T toolSettings, IEnumerable<DotNetNuGetAuthentication> validAuthenticationTypes) where T : DotNetNugetUpdateSourceTask.DotNetNuGetUpdateSourceSettings
	{
		toolSettings = toolSettings.NewInstance();
		var hashSet = new HashSet<DotNetNuGetAuthentication>(validAuthenticationTypes);
		toolSettings.ValidAuthenticationTypesInternal.RemoveAll(x => hashSet.Contains(x));
		return toolSettings;
	}

	#endregion
}

#endregion