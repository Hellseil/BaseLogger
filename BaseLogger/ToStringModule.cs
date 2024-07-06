using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LoggerLibrary
{
	internal static class ToStringModule
	{
		public static string ListToString(IEnumerable values,ref List<int> hashs)
		{
			var ret="";
			var sb = new StringBuilder();
			bool fast=true;
			var baseHash=hashs.GetHashCode();
			hashs.Add(baseHash);
			if(values !=null)
			{
				foreach(var item in values)
				{
					
					if (!fast) 
					sb.Append(",");
					else
					fast = false;
					if(item==null)
					{
						sb.Append("null");
					}
					else if (!hashs.Contains(item.GetHashCode()))
					{
						
						if(item is string)
						{
							sb.Append($"{item}");
						}
						else if(item is Enum @enum)
						{
							sb.Append($"{@enum.ToString()}");
						}
						else if(item is IEnumerable arr)
						{
							sb.Append(ListToString(arr, ref hashs));
						}
						else if(item.GetType().IsClass)
						{
							sb.Append(ClassToString(item, ref hashs));
						}
						else
						{
							sb.Append(item.ToString());
						}
					}
				}
				ret= $"[{sb.ToString()}]";
			}
			hashs.Remove(baseHash);
			return ret;
		}
		public	static string ClassToString(object clazz,ref List<int> hashs)
		{
			var ret = new StringBuilder();
			bool fast=true;
			var baseHash=clazz.GetHashCode();
			hashs.Add(baseHash);
			foreach(var item in clazz.GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance))
			{
				var name=item.Name; ;
				var value=item.GetValue(clazz);
				if (!fast) 
				ret.Append(",");
				else
				fast = false;
				if(value==null)
				{
					ret.Append($"{name}:null");
				}
				else if (!hashs.Contains(value.GetHashCode()))
				{
					if(value is string)
					{
						ret.Append($"{name}:{value}");
					}
					else if(value is Enum @enum)
					{
						ret.Append($"{name}:{@enum.ToString()}");
					}
					else if(value is IEnumerable arr)
					{
						ret.Append($"{name}:{ListToString(arr, ref hashs)}");
					}
					else if(value.GetType().IsClass)
					{
						ret.Append($"{name}:{ClassToString(value, ref hashs)}");
					}
					else
					{
						ret.Append($"{name}:{value.ToString()}");
					}
				}
			}
			hashs.Remove(baseHash);
			return $"<{ret}>";
		}
	}
}
