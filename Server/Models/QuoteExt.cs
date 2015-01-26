using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Models
{
	public static class QuoteExt
	{
		public static string Serialize(this Quote quote)
		{
			return string.Format("{0}={1}", quote.Symbol, quote.Price);
		}

		public static string Serialize(this IEnumerable<Quote> quotes, bool isColumnStyle = false)
		{
			var builder = new StringBuilder();
			var separator = isColumnStyle ? Environment.NewLine : ";";

			foreach (var quote in quotes)
			{
				builder.Append(quote.Serialize());
				builder.Append(separator);
			}
			return builder.ToString();
		}
	}
}
