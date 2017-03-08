using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
namespace ORMLite {
	public static class Reflections {

		/* Fields */

		public static List<PropertyInfo> GetDeclaredFields(Type type) {
			return type.GetTypeInfo().DeclaredProperties.ToList();
		}

		public static PropertyInfo GetDeclaredField(String name, Type type) {
			return type.GetTypeInfo().GetDeclaredProperty(name);
		}

		public static List<PropertyInfo> GetDeclaredFieldsRecursively(Type extensionClass, Type extendedClass) {
			List<PropertyInfo> fields = new List<PropertyInfo>();
			fields.AddRange(GetDeclaredFields(extensionClass));
			Type superClass = GetBaseType(extensionClass);
			if (superClass != extendedClass) {
				fields.AddRange(GetDeclaredFieldsRecursively(superClass, extendedClass));
			}
			return fields;
		}

		public static PropertyInfo GetDeclaredFieldRecursively(String fieldName, Type extensionClass, Type extendedClass) {
			PropertyInfo field = GetDeclaredField(fieldName, extensionClass);
			if (field == null) {
				Type superClass = GetBaseType(extensionClass);
				if (superClass != extendedClass) {
					field = GetDeclaredFieldRecursively(fieldName, superClass, extendedClass);
				}
			}
			return field;
		}

		public static Object CopyFieldFromObjectToObject(PropertyInfo field, Object fromObject, Object toObject) {
			field.SetValue(toObject, field.GetValue(fromObject));
			return toObject;
		}

		/* Classes*/

		public static Type GetBaseType(Type type) {
			return type.GetTypeInfo().BaseType;
		}

		public static object GetSuperInstanceFromInstance(object obj) {
			Type superClass = GetBaseType(obj.GetType());
			object superObject = Activator.CreateInstance(superClass);
			foreach (PropertyInfo field in GetDeclaredFieldsRecursively(superClass, typeof(PersistentEntity))) {
				superObject = CopyFieldFromObjectToObject(field, obj, superObject);
			}
			return superObject;
		}

		public static Object SetInstanceFromSuperInstance(Object obj, Object superObject) {
			foreach (PropertyInfo field in GetDeclaredFields(superObject.GetType())) {
				obj = CopyFieldFromObjectToObject(field, superObject, obj);
			}
			return obj;
		}

		public static dynamic CastTo(dynamic source, Type dest) {
  			return Convert.ChangeType(source, dest);
		}

		/* Assembly*/

		public static List<TypeInfo> GetTypesFromAssembly(String assemblyName) {
			Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
			return assembly.DefinedTypes.ToList();
		}

		/* Attributes*/

		public static bool IsAttributePresent(PropertyInfo property, Type attributeType) {
			return property.GetCustomAttributes(attributeType, true).Count() > 0;
		}

		public static Attribute GetAttribute(PropertyInfo property, Type attributeType) {
			return property.GetCustomAttributes(attributeType, true).FirstOrDefault();
		}
	}
}
