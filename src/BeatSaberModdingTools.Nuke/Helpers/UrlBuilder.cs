using System.Text;

namespace BeatSaberModdingTools.Nuke.Helpers
{
	public sealed class UrlBuilder
	{
		private readonly StringBuilder _url;

		private bool _firstQueryParamSet;

		public UrlBuilder(string baseUrl)
		{
			_url = new StringBuilder(baseUrl);
		}

		public UrlBuilder AppendQueryParam(string key, string value)
		{
			_url.Append(QueryParamSeparator()).Append(key).Append('=').Append(value);
			return this;
		}

		public override string ToString()
		{
			return _url.ToString();
		}

		public static implicit operator string(UrlBuilder urlBuilder) => urlBuilder.ToString();

		private char QueryParamSeparator()
		{
			if (_firstQueryParamSet)
			{
				return '&';
			}

			_firstQueryParamSet = true;
			return '?';
		}
	}
}