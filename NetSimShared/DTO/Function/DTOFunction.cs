using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shared.DTO.Function
{
	public class DTOFunction
	{
		
		public static string Serializer<T>(T dto) where T : struct
		{
			return JsonSerializer.Serialize(dto);
		}
		public static T Deerializer<T>(string jsonString) where T : struct
		{
			return JsonSerializer.Deserialize<T>(jsonString);
		}
		

	}
}
